using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace webrtc_dotnetcore.Models
{
    public class RoomManager
    {
        private int roomId;
        private readonly ConcurrentDictionary<int, RoomInfo> rooms;

        public RoomManager()
        {
            roomId = 1;
            rooms = new ConcurrentDictionary<int, RoomInfo>();
        }

        public RoomInfo CreateRoom(string connectionId, string name)
        {
            rooms.TryRemove(roomId, out _);

            var roomInfo = new RoomInfo
            {
                RoomId = roomId.ToString(),
                Name = name,
                HostConnectionId = connectionId
            };

            bool result = rooms.TryAdd(roomId, roomInfo);

            if (result)
            {
                roomId++;
                return roomInfo;
            }
            else
            {
                return null;
            }
        }

        public void DeleteRoom(int roomId)
        {
            rooms.TryRemove(roomId, out _);
        }

        public void DeleteRoom(string connectionId)
        {
            int? correspondingRoomId = null;

            foreach (var pair in rooms)
            {
                if (pair.Value.HostConnectionId.Equals(connectionId))
                {
                    correspondingRoomId = pair.Key;
                }
            }

            if (correspondingRoomId.HasValue)
            {
                rooms.TryRemove(correspondingRoomId.Value, out _);
            }
        }

        public List<RoomInfo> GetAllRoomInfo()
        {
            return rooms.Values.ToList();
        }
    }
}