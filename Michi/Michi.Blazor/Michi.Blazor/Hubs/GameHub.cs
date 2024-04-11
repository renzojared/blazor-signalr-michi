using Michi.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Michi.Blazor.Hubs;

public class GameHub : Hub
{
    private static readonly List<GameRoom> _rooms = [];

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Player with Id '{Context.ConnectionId}' connected");

        await Clients.Caller
            .SendAsync("Rooms", _rooms.OrderBy(s => s.RoomName));
    }

    public async Task<GameRoom> CreateRoom(string name, string playerName)
    {
        var roomId = Guid.NewGuid().ToString();
        GameRoom room = new(roomId, name);
        _rooms.Add(room);

        Player newPLayer = new(Context.ConnectionId, playerName);
        room.TryAddPlayer(newPLayer);

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.All.SendAsync("Rooms", _rooms.OrderBy(s => s.RoomName));
        return room;
    }

    public async Task<GameRoom?> JoinRoom(string roomId, string playerName)
    {
        var room = _rooms.FirstOrDefault(s => s.RoomId == roomId);
        if (room is not null)
        {
            var newPlayer = new Player(Context.ConnectionId, playerName);
            if (room.TryAddPlayer(newPlayer))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

                await Clients.Group(roomId).SendAsync("PlayerJoined", newPlayer);

                return room;
            }
        }

        return null;
    }

    public async Task StartGame(string roomId)
    {
        var room = _rooms.FirstOrDefault(s => s.RoomId == roomId);
        if (room is not null)
        {
            room.Game.StartGame();
            await Clients.Group(roomId).SendAsync("UpdateGame", room);
        }
    }

    public async Task MakeMove(string roomId, int row, int col, string playerId)
    {
        var room = _rooms.FirstOrDefault(s => roomId == s.RoomId);
        if (room != null && room.Game.MakeMove(row, col, playerId))
        {
            room.Game.Winner = room.Game.CheckWinner();
            room.Game.IsDraw = room.Game.CheckDraw() && string.IsNullOrEmpty(room.Game.Winner);

            if (!string.IsNullOrEmpty(room.Game.Winner) || room.Game.IsDraw)
                room.Game.GameOver = true;

            await Clients.Group(roomId).SendAsync("UpdateGame", room);
        }
    }
}