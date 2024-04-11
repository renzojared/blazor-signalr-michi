namespace Michi.Shared;

public class GameRoom(string roomId, string roomName)
{
    public string RoomId => roomId;
    public string RoomName => roomName;
    public List<Player> Players { get; set; } = [];
    public MichiGame Game { get; set; } = new();

    //TODO: User readonly and refact code
    public bool TryAddPlayer(Player newPlayer)
    {
        if (Players.Count < 2 && !Players.Any(s => s.ConnectionId == newPlayer.ConnectionId))
        {
            Players.Add(newPlayer);
            if (Players.Count == 1)
                Game.PlayerXId = newPlayer.ConnectionId;
            else if (Players.Count == 2)
                Game.PlayerOId = newPlayer.ConnectionId;
            
            return true;
        }

        return false;
    }
}