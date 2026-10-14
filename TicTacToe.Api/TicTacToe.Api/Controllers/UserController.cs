using Microsoft.AspNetCore.Mvc;

namespace TicTacToe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // GET api/user
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(new List<UserDto>
            {
                new UserDto { Name = "Alice", Email = "alice@example.com" },
                new UserDto { Name = "Bob", Email = "bob@example.com" }
            });
        }

        // GET api/user/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(new UserDto { Name = $"User {id}", Email = $"user{id}@example.com" });
        }

        // POST api/user
        [HttpPost]
        public IActionResult Create([FromBody] UserDto user)
        {
            return CreatedAtAction(nameof(GetById), new { id = 1 }, user);
        }

        // PUT api/user/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UserDto user)
        {
            return NoContent();
        }

        // DELETE api/user/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return NoContent();
        }
    }

    public class UserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}