namespace TicTacToe.Api.Models;

public class GameState
{
    public Guid GameId { get; set; } = Guid.NewGuid();

    public string[,] Board { get; set; } = new string[3, 3];

    public Player CurrentPlayer { get; set; } = Player.X;

    public GameMode Mode { get; set; } = GameMode.TwoPlayer;

    public GameStatus Status { get; set; } = GameStatus.InProgress;

    public Player? Winner { get; set; }

    public List<Move> MoveHistory { get; set; } = new();

    public List<int> WinningCells { get; set; } = new();

    public bool ResultRecorded { get; set; }
    
}