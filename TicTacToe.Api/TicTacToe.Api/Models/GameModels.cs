namespace TicTacToe.Api.Models;

public enum Player
{
    X,
    O
}

public enum GameMode
{
    TwoPlayer,
    Computer
}

public enum GameStatus
{
    InProgress,
    Won,
    Draw
}

public class MoveRequest
{
    public int Row { get; set; }
    public int Column { get; set; }
    public Player Player { get; set; }
}

public class Move
{
    public int MoveNumber { get; set; }
    public Player Player { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}

public class Scoreboard
{
    public int XWins { get; set; }
    public int OWins { get; set; }
    public int Draws { get; set; }
}

public class CreateGameRequest
{
    public GameMode GameMode { get; set; } = GameMode.TwoPlayer;
}