using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly GameService _gameService;

    public GamesController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    public IActionResult CreateGame(
        [FromBody] CreateGameRequest request)
    {
        try
        {
            return Ok(_gameService.CreateGame(request.GameMode));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{Id:guid}")]
    public IActionResult GetGame(Guid id)
    {
        try
        {
            return Ok(_gameService.GetGame(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/moves")]
    public IActionResult MakeMove(
        Guid id, [FromBody] MoveRequest request)
    {
        try
        {
            return Ok(_gameService.MakeMove(id, request));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/undo")]
    public IActionResult Undo(Guid id)
    {
        try
        {
            return Ok(_gameService.Undo(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/reset")]
    public IActionResult ResetGame(Guid id)
    {
        try
        {
            return Ok(_gameService.ResetGame(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}