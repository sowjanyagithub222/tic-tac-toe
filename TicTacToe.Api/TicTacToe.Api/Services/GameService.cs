using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services;

public class GameService
{
    private readonly Dictionary<Guid, GameState> _games = new();
    private readonly Scoreboard _scoreboard = new();

    private readonly object _lock = new();

    private static readonly int[][] WinningLines =
    {
        new[] { 0, 1, 2 },
        new[] { 3, 4, 5 },
        new[] { 6, 7, 8 },
        new[] { 0, 3, 6 },
        new[] { 1, 4, 7 },
        new[] { 2, 5, 8 },
        new[] { 0, 4, 8 },
        new[] { 2, 4, 6 }
    };

    public GameStateResponse CreateGame(GameMode mode)
    {
        if (!Enum.IsDefined(typeof(GameMode), mode))
            throw new ArgumentException("Invalid game mode.");

        lock (_lock)
        {
            var game = new GameState { Mode = mode };
            _games[game.GameId] = game;

            return ToResponse(game);
        }
    }

    public GameStateResponse GetGame(Guid id)
    {
        lock (_lock)
        {
            return ToResponse(GetGameOrThrow(id));
        }
    }

    public GameStateResponse MakeMove(Guid id, MoveRequest request)
    {
        lock (_lock)
        {
            var game = GetGameOrThrow(id);

            if (game.Status != GameStatus.InProgress)
                throw new InvalidOperationException(
                    "The game is already completed.");

            if (request.Row < 0 || request.Row > 2 ||
                request.Column < 0 || request.Column > 2)
                throw new ArgumentException(
                    "Row and column must be between 0 and 2.");

            if (!Enum.IsDefined(typeof(Player), request.Player))
                throw new ArgumentException("Invalid player.");

            if (request.Player != game.CurrentPlayer)
                throw new InvalidOperationException(
                    $"It is player {game.CurrentPlayer}'s turn.");

            if (game.Mode == GameMode.Computer &&
                request.Player == Player.O)
                throw new InvalidOperationException(
                    "The computer controls player O.");

            if (game.Board[request.Row, request.Column] != null)
                throw new InvalidOperationException(
                    "That cell is already occupied.");

            ApplyMove(game, request.Row, request.Column,
                request.Player);

            if (game.Status == GameStatus.InProgress &&
                game.Mode == GameMode.Computer)
            {
                MakeComputerMove(game);
            }

            return ToResponse(game);
        }
    }

    private void ApplyMove(
        GameState game, int row, int column, Player player)
    {
        game.Board[row, column] = player.ToString();

        game.MoveHistory.Add(new Move
        {
            MoveNumber = game.MoveHistory.Count + 1,
            Player = player,
            Row = row,
            Column = column
        });

        CheckGameStatus(game);

        if (game.Status == GameStatus.InProgress)
        {
            game.CurrentPlayer =
                player == Player.X ? Player.O : Player.X;
        }
    }

    private void CheckGameStatus(GameState game)
    {
        game.WinningCells.Clear();

        foreach (var line in WinningLines)
        {
            int a = line[0];
            int b = line[1];
            int c = line[2];

            string? first = GetCell(game, a);
            string? second = GetCell(game, b);
            string? third = GetCell(game, c);

            if (first != null &&
                first == second &&
                second == third)
            {
                game.Status = GameStatus.Won;
                game.Winner = Enum.Parse<Player>(first);
                game.WinningCells = line.ToList();

                RecordResult(game);
                return;
            }
        }

        bool boardFull = game.MoveHistory.Count == 9;

        if (boardFull)
        {
            game.Status = GameStatus.Draw;
            RecordResult(game);
        }
    }

    private string? GetCell(GameState game, int index)
    {
        return game.Board[index / 3, index % 3];
    }

    private void RecordResult(GameState game)
    {
        if (game.ResultRecorded)
            return;

        if (game.Status == GameStatus.Won)
        {
            if (game.Winner == Player.X)
                _scoreboard.XWins++;
            else if (game.Winner == Player.O)
                _scoreboard.OWins++;
        }
        else if (game.Status == GameStatus.Draw)
        {
            _scoreboard.Draws++;
        }

        game.ResultRecorded = true;
    }

    private void MakeComputerMove(GameState game)
    {
        // Priority 1: O wins if possible.
        int cell = FindWinningMove(game, Player.O);

        // Priority 2: Block X from winning.
        if (cell == -1)
            cell = FindWinningMove(game, Player.X);

        // Priority 3: Take the center.
        if (cell == -1 && GetCell(game, 4) == null)
            cell = 4;

        // Priority 4: Take a corner.
        if (cell == -1)
        {
            foreach (int corner in new[] { 0, 2, 6, 8 })
            {
                if (GetCell(game, corner) == null)
                {
                    cell = corner;
                    break;
                }
            }
        }

        // Priority 5: Take any available cell.
        if (cell == -1)
        {
            for (int i = 0; i < 9; i++)
            {
                if (GetCell(game, i) == null)
                {
                    cell = i;
                    break;
                }
            }
        }

        if (cell != -1)
        {
            ApplyMove(game, cell / 3, cell % 3, Player.O);
        }
    }

    private int FindWinningMove(GameState game, Player player)
    {
        for (int i = 0; i < 9; i++)
        {
            int row = i / 3;
            int column = i % 3;

            if (game.Board[row, column] != null)
                continue;

            game.Board[row, column] = player.ToString();

            bool wins = WinningLines.Any(line =>
                line.All(index =>
                    GetCell(game, index) == player.ToString()));

            game.Board[row, column] = null;

            if (wins)
                return i;
        }

        return -1;
    }

    public GameStateResponse Undo(Guid id)
    {
        lock (_lock)
        {
            var game = GetGameOrThrow(id);

            // Decision: Option A from the assignment.
            // Completed games cannot be undone.
            if (game.Status != GameStatus.InProgress)
                throw new InvalidOperationException(
                    "Undo is disabled after game completion.");

            if (game.MoveHistory.Count == 0)
                throw new InvalidOperationException(
                    "There are no moves to undo.");

            int movesToRemove =
                game.Mode == GameMode.Computer ? 2 : 1;

            int actualMoves = Math.Min(
                movesToRemove, game.MoveHistory.Count);

            for (int i = 0; i < actualMoves; i++)
            {
                int lastIndex = game.MoveHistory.Count - 1;
                Move lastMove = game.MoveHistory[lastIndex];

                game.Board[lastMove.Row, lastMove.Column] = null;
                game.MoveHistory.RemoveAt(lastIndex);
            }

            for (int i = 0; i < game.MoveHistory.Count; i++)
            {
                game.MoveHistory[i].MoveNumber = i + 1;
            }

            game.CurrentPlayer = game.MoveHistory.Count % 2 == 0
                ? Player.X
                : Player.O;

            game.Status = GameStatus.InProgress;
            game.Winner = null;
            game.WinningCells.Clear();

            return ToResponse(game);
        }
    }

    public GameStateResponse ResetGame(Guid id)
    {
        lock (_lock)
        {
            var game = GetGameOrThrow(id);

            Array.Clear(game.Board, 0, game.Board.Length);
            game.MoveHistory.Clear();
            game.WinningCells.Clear();

            game.CurrentPlayer = Player.X;
            game.Status = GameStatus.InProgress;
            game.Winner = null;
            game.ResultRecorded = false;

            return ToResponse(game);
        }
    }

    public Scoreboard GetScoreboard()
    {
        lock (_lock)
        {
            return new Scoreboard
            {
                XWins = _scoreboard.XWins,
                OWins = _scoreboard.OWins,
                Draws = _scoreboard.Draws
            };
        }
    }

    public Scoreboard ResetScoreboard()
    {
        lock (_lock)
        {
            _scoreboard.XWins = 0;
            _scoreboard.OWins = 0;
            _scoreboard.Draws = 0;

            // Existing games are not changed.
            // Their completed results are not recorded again.
            return GetScoreboard();
        }
    }

    private GameState GetGameOrThrow(Guid id)
    {
        if (!_games.TryGetValue(id, out var game))
            throw new KeyNotFoundException("Game not found.");

        return game;
    }

    private GameStateResponse ToResponse(GameState game)
    {
        var board = new string?[9];

        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                board[row * 3 + column] =
                    game.Board[row, column];
            }
        }

        return new GameStateResponse
        {
            GameId = game.GameId,
            Board = board,
            CurrentPlayer = game.CurrentPlayer,
            Mode = game.Mode,
            Status = game.Status,
            Winner = game.Winner,
            MoveHistory = game.MoveHistory
                .Select(m => new Move
                {
                    MoveNumber = m.MoveNumber,
                    Player = m.Player,
                    Row = m.Row,
                    Column = m.Column
                })
                .ToList(),
            WinningCells = game.WinningCells.ToList()
        };
    }

}