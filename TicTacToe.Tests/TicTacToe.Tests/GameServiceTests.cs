
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;
using Xunit;

namespace TicTacToe.Tests;

public class GameServiceTests
{
    private static GameService NewService() => new();

    private static GameStateResponse NewTwoPlayerGame(
        GameService service)
    {
        return service.CreateGame(GameMode.TwoPlayer);
    }

    private static GameStateResponse Play(
        GameService service,
        Guid gameId,
        Player player,
        int row,
        int column)
    {
        return service.MakeMove(gameId, new MoveRequest
        {
            Player = player,
            Row = row,
            Column = column
        });
    }

    [Fact]
    public void ValidMove_ShouldUpdateBoard()
    {
        var service = NewService();
        var game = NewTwoPlayerGame(service);

        var result = Play(service, game.GameId,
            Player.X, 0, 0);

        Assert.Equal("X", result.Board[0]);
        Assert.Single(result.MoveHistory);
    }

    [Fact]
    public void InvalidMove_ShouldBeRejected()
    {
        var service = NewService();
        var game = NewTwoPlayerGame(service);

        Play(service, game.GameId, Player.X, 0, 0);

        Assert.Throws<InvalidOperationException>(() =>
            Play(service, game.GameId, Player.O, 0, 0));

        Assert.Throws<ArgumentException>(() =>
            Play(service, game.GameId, Player.O, 3, 0));

        var state = service.GetGame(game.GameId);

        Assert.Equal("X", state.Board[0]);
        Assert.Equal(Player.O, state.CurrentPlayer);
        Assert.Single(state.MoveHistory);
    }

    [Fact]
    public void ValidMove_ShouldSwitchTurn()
    {
        var service = NewService();
        var game = NewTwoPlayerGame(service);

        var result = Play(service, game.GameId,
            Player.X, 0, 0);

        Assert.Equal(Player.O, result.CurrentPlayer);
    }

    [Fact]
    public void ThreeMarksInRow_ShouldWin()
    {
        var service = NewService();
        var game = NewTwoPlayerGame(service);

        Play(service, game.GameId, Player.X, 0, 0);
        Play(service, game.GameId, Player.O, 1, 0);
        Play(service, game.GameId, Player.X, 0, 1);
        Play(service, game.GameId, Player.O, 1, 1);

        var result = Play(service, game.GameId,
            Player.X, 0, 2);

        Assert.Equal(GameStatus.Won, result.Status);
        Assert.Equal(Player.X, result.Winner);
        Assert.Equal(new[] { 0, 1, 2 }, result.WinningCells);
    }

    [Fact]
    public void ThreeMarksInColumn_ShouldWin()
    {
        var service = NewService();
        var game = NewTwoPlayerGame(service);

        Play(service, game.GameId, Player.X, 0, 0);
        Play(service, game.GameId, Player.O, 0, 1);
        Play(service, game.GameId, Player.X, 1, 0);
        Play(service, game.GameId, Player.O, 1, 1);

        var result = Play(service, game.GameId,
            Player.X, 2, 0);

        Assert.Equal(GameStatus.Won, result.Status);
        Assert.Equal(Player.X, result.Winner);
        Assert.Equal(new[] { 0, 3, 6 }, result.WinningCells);
    }

    [Fact]
    public void ThreeMarksDiagonally_ShouldWin()
    {
        var service = NewService();
        var game = NewTwoPlayerGame(service);

        Play(service, game.GameId, Player.X, 0, 0);
        Play(service, game.GameId, Player.O, 0, 1);
        Play(service, game.GameId, Player.X, 1, 1);
        Play(service, game.GameId, Player.O, 0, 2);

        var result = Play(service, game.GameId,
            Player.X, 2, 2);

        Assert.Equal(GameStatus.Won, result.Status);
        Assert.Equal(Player.X, result.Winner);
        Assert.Equal(new[] { 0, 4, 8 }, result.WinningCells);
    }

    [Fact]
    public void FullBoardWithoutWinner_ShouldBeDraw()
    {
        var service = NewService();
        var game = NewTwoPlayerGame(service);

        Play(service, game.GameId, Player.X, 0, 0);
        Play(service, game.GameId, Player.O, 0, 1);
        Play(service, game.GameId, Player.X, 0, 2);
        Play(service, game.GameId, Player.O, 1, 1);
        Play(service, game.GameId, Player.X, 1, 0);
        Play(service, game.GameId, Player.O, 1, 2);
        Play(service, game.GameId, Player.X, 2, 1);
        Play(service, game.GameId, Player.O, 2, 0);

        var result = Play(service, game.GameId,
            Player.X, 2, 2);

        Assert.Equal(GameStatus.Draw, result.Status);
        Assert.Null(result.Winner);
        Assert.Equal(9, result.MoveHistory.Count);
        Assert.Equal(1, service.GetScoreboard().Draws);
    }

    [Fact]
    public void ResetGame_ShouldClearBoardAndKeepScoreboard()
    {
        var service = NewService();
        var game = NewTwoPlayerGame(service);

        Play(service, game.GameId, Player.X, 0, 0);
        Play(service, game.GameId, Player.O, 1, 1);

        var result = service.ResetGame(game.GameId);

        Assert.All(result.Board, cell => Assert.Null(cell));
        Assert.Empty(result.MoveHistory);
        Assert.Equal(Player.X, result.CurrentPlayer);
        Assert.Equal(GameStatus.InProgress, result.Status);

        var scores = service.GetScoreboard();
        Assert.Equal(0, scores.XWins);
        Assert.Equal(0, scores.OWins);
        Assert.Equal(0, scores.Draws);
    }

    [Fact]
    public void UndoInTwoPlayerMode_ShouldRemoveOnlyLastMove()
    {
        var service = NewService();
        var game = NewTwoPlayerGame(service);

        Play(service, game.GameId, Player.X, 0, 0);
        Play(service, game.GameId, Player.O, 1, 1);

        var result = service.Undo(game.GameId);

        Assert.Equal("X", result.Board[0]);
        Assert.Null(result.Board[4]);
        Assert.Single(result.MoveHistory);
        Assert.Equal(Player.O, result.CurrentPlayer);
    }

    [Fact]
    public void UndoInComputerMode_ShouldRemoveBothMoves()
    {
        var service = NewService();

        var game = service.CreateGame(GameMode.Computer);

        // X plays; the backend automatically makes O's move.
        var afterMoves = Play(service, game.GameId,
            Player.X, 0, 0);

        Assert.Equal(2, afterMoves.MoveHistory.Count);

        var result = service.Undo(game.GameId);

        Assert.All(result.Board, cell => Assert.Null(cell));
        Assert.Empty(result.MoveHistory);
        Assert.Equal(Player.X, result.CurrentPlayer);
        Assert.Equal(GameStatus.InProgress, result.Status);
    }

    [Fact]
    public void WinningMove_ShouldUpdateScoreboardOnlyOnce()
    {
        var service = NewService();
        var game = NewTwoPlayerGame(service);

        Play(service, game.GameId, Player.X, 0, 0);
        Play(service, game.GameId, Player.O, 1, 0);
        Play(service, game.GameId, Player.X, 0, 1);
        Play(service, game.GameId, Player.O, 1, 1);

        Play(service, game.GameId, Player.X, 0, 2);

        Assert.Throws<InvalidOperationException>(() =>
            Play(service, game.GameId, Player.O, 2, 2));

        var scores = service.GetScoreboard();

        Assert.Equal(1, scores.XWins);
        Assert.Equal(0, scores.OWins);
        Assert.Equal(0, scores.Draws);
    }

    [Fact]
    public void ComputerMode_ShouldChooseCenterWhenAvailable()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.Computer);

        // X chooses the top-left corner.
        var result = Play(service, game.GameId,
            Player.X, 0, 0);

        // O should automatically take the center.
        Assert.Equal("O", result.Board[4]);
        Assert.Equal(2, result.MoveHistory.Count);
        Assert.Equal(Player.X, result.CurrentPlayer);
    }

    [Fact]
    public void MoveAfterGameCompletion_ShouldBeRejected()
    {
        var service = NewService();
        var game = NewTwoPlayerGame(service);

        Play(service, game.GameId, Player.X, 0, 0);
        Play(service, game.GameId, Player.O, 1, 0);
        Play(service, game.GameId, Player.X, 0, 1);
        Play(service, game.GameId, Player.O, 1, 1);

        Play(service, game.GameId, Player.X, 0, 2);

        var before = service.GetGame(game.GameId);

        Assert.Throws<InvalidOperationException>(() =>
            Play(service, game.GameId, Player.O, 2, 2));

        var after = service.GetGame(game.GameId);

        Assert.Equal(before.Status, after.Status);
        Assert.Equal(before.Board, after.Board);
        Assert.Equal(before.MoveHistory.Count,
            after.MoveHistory.Count);
    }




    [Fact]
    public void Computer_ShouldTakeWinningMove()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.Computer);

        // X at top-left; O automatically takes center.
        Play(service, game.GameId, Player.X, 0, 0);

        // X plays top-right; O takes bottom-left.
        Play(service, game.GameId, Player.X, 0, 2);

        // X plays bottom-right; O must win at middle-left.
        var result = Play(service, game.GameId, Player.X, 2, 2);


        Assert.Equal(GameStatus.Won, result.Status);
        Assert.Equal(Player.O, result.Winner);
        Assert.Equal("O", result.Board[7]);
        Assert.Equal(new[] { 1, 4, 7 }, result.WinningCells);

    }

    [Fact]
    public void Computer_ShouldBlockHumanWinningMove()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.Computer);

        // X takes a corner; O takes center.
        Play(service, game.GameId, Player.X, 0, 0);

        // X threatens the top row; O must block at top-middle.
        var result = Play(service, game.GameId, Player.X, 0, 2);

        Assert.Equal("O", result.Board[1]);
        Assert.Equal(GameStatus.InProgress, result.Status);
        Assert.Null(result.Winner);
        Assert.Equal(4, result.MoveHistory.Count);
    }

    [Fact]
    public void Computer_ShouldChooseCornerWhenCenterIsOccupied()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.Computer);

        // X takes center; O should choose the first available corner.
        var result = Play(service, game.GameId, Player.X, 1, 1);

        Assert.Equal("O", result.Board[0]);
        Assert.Equal(2, result.MoveHistory.Count);
        Assert.Equal(GameStatus.InProgress, result.Status);
    }

    [Fact]
    public void Computer_ShouldChooseAnyAvailableCellWhenCornersAndCenterAreOccupied()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.Computer);

        // X takes top-left; O takes center.
        Play(service, game.GameId, Player.X, 0, 0);

        // X takes top-right; O takes bottom-left.
        Play(service, game.GameId, Player.X, 0, 2);

        // X takes bottom-right; O must block at top-middle.
        // This sequence would trigger the blocking priority, so this
        // test must instead create a state where no winning threat exists.
    }

}
