namespace Michi.Shared;

public class Player(string connectionId, string name)
{
    public string ConnectionId => connectionId;
    public string Name => name;
}