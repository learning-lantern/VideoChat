
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using webrtc_dotnetcore.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endponts =>
{
    endponts.MapControllers();
    endponts.MapHub<WebRTCHub>("/WebRTCHub");
});

app.Run();