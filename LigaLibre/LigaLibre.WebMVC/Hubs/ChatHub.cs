using Microsoft.AspNetCore.SignalR;

namespace LigaLibre.WebMVC.Hubs;

public class ChatHub : Hub
{
    private static readonly Dictionary<string, string> connectedUsers = new();
    private static readonly Dictionary<string, string> userRooms = new();

    public async Task SendMessage (string user, string message)
    {
        await Clients.Others.SendAsync("ReceiveMessage", user, message, DateTime.Now);
    }

    public async Task JoinRoom(string roomName, string userName)
    {
        // Remover de la sala anterior si existe
        if (userRooms.TryGetValue(Context.ConnectionId, out var oldRoom))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, oldRoom);
        }
        
        // Agregar a la nueva sala
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        userRooms[Context.ConnectionId] = roomName;
        
        await Clients.OthersInGroup(roomName).SendAsync("UserJoined", userName, roomName);
        await Clients.Caller.SendAsync("UserJoined", userName, roomName);
    }

    public async Task SendMessageToRoom(string roomName, string user, string message)
    {
        await Clients.OthersInGroup(roomName).SendAsync("ReceiveMessage", user, message, DateTime.Now);
    }

    public async Task NotifyTyping(string roomName, string user)
    {
        await Clients.OthersInGroup(roomName).SendAsync("UserTyping", user);
    }

    public async Task CanceledNotifyTyping(string roomName, string user)
    {
        await Clients.OthersInGroup(roomName).SendAsync("UserCanceledTyping", user);
    }

    public override async Task OnConnectedAsync()
    {
        var userName = Context.GetHttpContext()?.Request.Query["user"].ToString() ?? "Anonimo";
        connectedUsers[Context.ConnectionId] = userName;
        await Clients.All.SendAsync("UserConnected", userName, Context.ConnectionId);
        await Clients.Caller.SendAsync("ConnectedUsers", connectedUsers);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync (Exception? exception)
    {
        if(connectedUsers.TryGetValue(Context.ConnectionId, out var userName))
        {
            connectedUsers.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("UserDisconnected", userName);
        }

        await base.OnDisconnectedAsync((exception));
    }
}

