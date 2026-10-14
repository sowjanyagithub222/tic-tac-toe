namespace TicTacToe.Api.Models;

public class GameStateResponse
{
    public Guid GameId { get; set; }

    public string?[] Board { get; set; } = new string?[9];

    public Player CurrentPlayer { get; set; }

    public GameMode Mode { get; set; }

    public GameStatus Status { get; set; }

    public Player? Winner { get; set; }

    public List<Move> MoveHistory { get; set; } = new();

    public List<int> WinningCells { get; set; } = new();
}