using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;

namespace ShoppingCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly HttpClient _httpClient;

        public ShoppingCartController(UserContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: api/ShoppingCart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetShoppingCartItems()
        {
            try
            {
                // Fetch shopping cart items from the database
                var items = await _context.ShoppingCartItems.ToListAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/ShoppingCart/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetShoppingCartItem(int id)
        {
            try
            {
                var item = await _context.ShoppingCartItems.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/ShoppingCart
        [HttpPost]
        public async Task<ActionResult<User>> AddToShoppingCart(User user)
        {
            try
            {
                _context.ShoppingCartItems.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetShoppingCartItem), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/ShoppingCart/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShoppingCartItem(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShoppingCartItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/ShoppingCart/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromShoppingCart(int id)
        {
            try
            {
                var item = await _context.ShoppingCartItems.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }

                _context.ShoppingCartItems.Remove(item);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool ShoppingCartItemExists(int id)
        {
            return _context.ShoppingCartItems.Any(e => e.Id == id);
        }

                // POST: api/ShoppingCart
        [HttpPost]
        public async Task<ActionResult<User>> AddToShoppingCart(User user)
        {
            try
            {
                // Ajouter un nouvel élément au panier d'achat
                _context.ShoppingCartItems.Add(user);
                await _context.SaveChangesAsync();

                // Appel vers le service de payment processing pour effectuer le paiement
                var paymentRequest = new PaymentRequest { Amount = user.Amount, Token = user.Token };
                var paymentResponse = await _httpClient.PostAsJsonAsync(_paymentProcessingUrl, paymentRequest);
                if (!paymentResponse.IsSuccessStatusCode)
                {
                    // Gérer l'échec du paiement
                    return StatusCode((int)paymentResponse.StatusCode, "Payment processing failed");
                }

                // Appel vers le service de user management pour créer l'utilisateur
                var userResponse = await _httpClient.PostAsJsonAsync(_userManagementUrl, user);
                if (!userResponse.IsSuccessStatusCode)
                {
                    // Gérer l'échec de la création de l'utilisateur
                    return StatusCode((int)userResponse.StatusCode, "User creation failed");
                }

                return CreatedAtAction(nameof(GetShoppingCartItem), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

