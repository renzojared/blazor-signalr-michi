using Michi.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Michi.Blazor.Client.Components;

public partial class Room : ComponentBase
{
    private string? MyPlayerId;
    [CascadingParameter] public HubConnection HubConnection { get; set; }
    [Parameter] public GameRoom? CurrentRoom { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (CurrentRoom is null || HubConnection is null || HubConnection.ConnectionId is null)
            return;

        MyPlayerId = HubConnection.ConnectionId;
        
        HubConnection.On<Player>("PlayerJoined", player =>
        {
            CurrentRoom.Players.Add(player);
            StateHasChanged();
        });
        HubConnection.On<GameRoom>("UpdateGame", serverRoom =>
        {
            CurrentRoom = serverRoom;
            StateHasChanged();
        });
    }
    //TODO: Abandon room
    //TODO: Go to waiting room
    //TODO: Recents room

    async Task StartGame()
    {
        if (HubConnection is null || CurrentRoom is null)
            return;
        await HubConnection.InvokeAsync(nameof(StartGame), CurrentRoom.RoomId);
    }

    private async Task MakeMove(int row, int col)
    {
        if (IsMyTurn() && CurrentRoom is not null && CurrentRoom.Game.GameStarted && !CurrentRoom.Game.GameOver &&
            HubConnection is not null)
        {
            await HubConnection.InvokeAsync(
                nameof(MakeMove),
                CurrentRoom.RoomId,
                row,
                col,
                MyPlayerId);
        }
    }

    private bool IsMyTurn()
    {
        if (CurrentRoom is not null)
        {
            return MyPlayerId == CurrentRoom.Game.CurrentPlayerId;
        }

        return false;
    }
}