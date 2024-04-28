using Microsoft.AspNetCore.Mvc;
using UserManagement.Data;
using UserManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IConfiguration _config;

        public UsersController(UserContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: /api/users/authenticate
        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Authenticate(User user)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);
            if (dbUser != null)
            {
                var token = GenerateJwtToken(dbUser);
                return Ok(token);
            }
            return Unauthorized();
        }

        // POST: /api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // GET: /api/users
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: /api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // PUT: /api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: /api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private string GenerateJwtToken(User user)
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                    new Claim(JwtRegisteredClaimNames.Email, user.Username), // Assuming username is the email
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(30), // Adjust token lifetime as necessary
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

       [HttpGet("external")]
        public async Task<ActionResult<IEnumerable<string>>> GetInfoFromOtherMicroservices()
        {
            var orderManagementUrl = _configuration["ServiceUrls:OrderManagement"];
            var paymentProcessingUrl = _configuration["ServiceUrls:PaymentProcessing"];
            var productCatalogUrl = _configuration["ServiceUrls:ProductCatalog"];
            var shoppingCartUrl = _configuration["ServiceUrls:ShoppingCart"];

            var tasks = new List<Task<string>>();

            using (var client = new HttpClient())
            {
                tasks.Add(GetContentFromUrl(client, orderManagementUrl));
                tasks.Add(GetContentFromUrl(client, paymentProcessingUrl));
                tasks.Add(GetContentFromUrl(client, productCatalogUrl));
                tasks.Add(GetContentFromUrl(client, shoppingCartUrl));

                await Task.WhenAll(tasks);

                var results = new List<string>();
                foreach (var task in tasks)
                {
                    results.Add(await task);
                }

                return Ok(results);
            }
        }

        private async Task<string> GetContentFromUrl(HttpClient client, string url)
        {
            try
            {
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return content;
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Internal server error: {ex.Message}";
            }
        }
    }
}
