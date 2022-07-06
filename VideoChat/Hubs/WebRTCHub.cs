using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using webrtc_dotnetcore.Models;

namespace webrtc_dotnetcore.Hubs
{
    public class WebRTCHub : Hub
    {
        private static readonly RoomManager roomManager = new();

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            roomManager.DeleteRoom(Context.ConnectionId);
            await NotifyRoomInfoAsync(true);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task CreateRoom(string name)
        {
            RoomInfo roomInfo = roomManager.CreateRoom(Context.ConnectionId, name);
            if (roomInfo != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomInfo.RoomId);
                await Clients.Caller.SendAsync("created", roomInfo.RoomId);
                await NotifyRoomInfoAsync(false);
            }
            else
            {
                await Clients.Caller.SendAsync("error", "error occurred when creating a new room.");
            }
        }

        public async Task Join(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Caller.SendAsync("joined", roomId);
            await Clients.Group(roomId).SendAsync("ready");

            if (int.TryParse(roomId, out int id))
            {
                roomManager.DeleteRoom(id);
                await NotifyRoomInfoAsync(false);
            }
        }

        public async Task LeaveRoom(string roomId)
        {
            await Clients.Group(roomId).SendAsync("bye");
        }

        public async Task GetRoomInfo()
        {
            await NotifyRoomInfoAsync(true);
        }

        public async Task SendMessage(string roomId, object message)
        {
            await Clients.OthersInGroup(roomId).SendAsync("message", message);
        }

        public async Task NotifyRoomInfoAsync(bool notifyOnlyCaller)
        {
            List<RoomInfo> roomInfos = roomManager.GetAllRoomInfo();
            var list = from room in roomInfos
                       select new
                       {
                           room.RoomId,
                           room.Name,
                           Button = "<button class=\"joinButton\">Join!</button>"
                       };
            var data = JsonConvert.SerializeObject(list);

            if (notifyOnlyCaller)
            {
                await Clients.Caller.SendAsync("updateRoom", data);
            }
            else
            {
                await Clients.All.SendAsync("updateRoom", data);
            }
        }
    }
}
