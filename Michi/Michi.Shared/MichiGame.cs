using System.Data;

namespace Michi.Shared;

public class MichiGame
{
    public string? PlayerXId { get; set; }
    public string? PlayerOId { get; set; }
    public string? CurrentPlayerId { get; set; }
    public string CurrentPlayerSymbol => CurrentPlayerId == PlayerXId ? "X" : "O";
    public bool GameStarted { get; set; } = false;
    public bool GameOver { get; set; } = false;
    public bool IsDraw { get; set; } = false;

    public string Winner { get; set; } = string.Empty;

    //TODO: See all games wins
    public List<List<string>> Board { get; set; } = new(3);

    public MichiGame()
    {
        InitializedBoard();
    }

    public void StartGame()
    {
        CurrentPlayerId = PlayerXId;
        GameStarted = true;
        GameOver = false;
        Winner = string.Empty;
        IsDraw = false;
        InitializedBoard();
    }

    private void InitializedBoard()
    {
        Board.Clear();
        for (short i = 0; i < 3; i++)
        {
            List<string> row = new(3);
            for (short j = 0; j < 3; j++)
            {
                row.Add(string.Empty);
            }

            Board.Add(row);
        }
    }

    public void TogglePlayer()
    {
        CurrentPlayerId = CurrentPlayerId == PlayerXId ? PlayerOId : PlayerXId;
    }

    public bool MakeMove(int row, int col, string playerId)
    {
        if (playerId != CurrentPlayerId || row < 0 || row >= 3 || col < 0 || col >= 3 ||
            Board[row][col] != string.Empty)
            return false;

        Board[row][col] = CurrentPlayerSymbol;
        TogglePlayer();
        return true;
    }

    public string CheckWinner()
    {
        //Rows and columns
        for (short i = 0; i < 3; i++)
        {
            if (!string.IsNullOrEmpty(Board[i][0]) && Board[i][0] == Board[i][1] && Board[i][1] == Board[i][2])
                return Board[i][0];

            if (!string.IsNullOrEmpty(Board[0][i]) && Board[0][i] == Board[1][i] && Board[1][i] == Board[2][i])
                return Board[0][i];
        }

        //Diagonals
        if (!string.IsNullOrEmpty(Board[0][0]) && Board[0][0] == Board[1][1] && Board[1][1] == Board[2][2])
            return Board[0][0];

        if (!string.IsNullOrEmpty(Board[0][2]) && Board[0][2] == Board[1][1] && Board[1][1] == Board[2][0])
            return Board[0][2];
        return string.Empty;
    }

    public bool CheckDraw()
    {
        return IsDraw = Board.All(row => row.All(cell => !string.IsNullOrEmpty(cell)));
    }

}