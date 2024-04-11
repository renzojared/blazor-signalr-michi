using Michi.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace Michi.Blazor.Client.Pages;

public partial class Lobby
{
    private HubConnection? _hubConnection;
    private string playerName = string.Empty;
    private string currentRoomName = string.Empty;
    private GameRoom? currentRoom;
    private List<GameRoom> rooms = [];

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
            .Build();

        _hubConnection.On<List<GameRoom>>("Rooms",
            (roomList) =>
            {
                Console.WriteLine($"We got rooms! Count = {roomList.Count}");
                rooms = roomList;
                StateHasChanged();
            });

        await _hubConnection.StartAsync();
    }

    private async Task CreateRoom()
    {
        if (_hubConnection is null)
            return;
        currentRoom = await _hubConnection.InvokeAsync<GameRoom>(
            nameof(CreateRoom),
            currentRoomName,
            playerName);
    }

    private async Task JoinRoom(string roomId)
    {
        if (_hubConnection is null)
            return;

        var joinedRoom = await _hubConnection.InvokeAsync<GameRoom>(
            nameof(JoinRoom),
            roomId,
            playerName);

        if (joinedRoom is not null)
            currentRoom = joinedRoom;
        else
            Console.WriteLine("Room is full or does not exist.");
    }
}