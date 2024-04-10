using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _userService.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            await _userService.Create(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            await _userService.Update(user);
            return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.Delete(id);
            return NoContent();
        }
    }
}