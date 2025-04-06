using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private static Dictionary<string, List<string>> RoomUsers = new Dictionary<string, List<string>>();

    public async Task JoinRoom(string userName, string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

        if (!RoomUsers.ContainsKey(roomName))
        {
            RoomUsers[roomName] = new List<string>();
        }

        if (!RoomUsers[roomName].Contains(userName))
        {
            RoomUsers[roomName].Add(userName);
        }

        // ✅ Send Updated Active Users List
        await Clients.Group(roomName).SendAsync("UpdateActiveUsers", RoomUsers[roomName]);

        await Clients.Group(roomName).SendAsync("UserJoined", userName);
    }

    public async Task SendMessage(string userName, string roomName, string message)
    {
        await Clients.Group(roomName).SendAsync("ReceiveMessage", userName, message);
    }

    public async Task LeaveRoom(string userName, string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

        if (RoomUsers.ContainsKey(roomName))
        {
            RoomUsers[roomName].Remove(userName);

            // ✅ Send Updated Active Users List
            await Clients.Group(roomName).SendAsync("UpdateActiveUsers", RoomUsers[roomName]);

            // ✅ Notify all users in the room that this user left
            await Clients.Group(roomName).SendAsync("UserLeft", userName);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var room in RoomUsers.Keys)
        {
            if (RoomUsers[room].Contains(Context.ConnectionId))
            {
                RoomUsers[room].Remove(Context.ConnectionId);

                // ✅ Update Active Users List
                await Clients.Group(room).SendAsync("UpdateActiveUsers", RoomUsers[room]);

                await Clients.Group(room).SendAsync("UserLeft", Context.ConnectionId);
                break;
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}
