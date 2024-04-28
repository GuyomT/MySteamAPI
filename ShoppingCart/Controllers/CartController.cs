// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.Collections.Generic;
// using System.Net.Http;
// using System.Text;
// using System.Text.Json;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using ShoppingCart.Models;
// using ShoppingCart.Data;

// namespace ShoppingCart.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class CartController : ControllerBase
//     {
//         private readonly CartContext _context;
//         private readonly HttpClient _httpClient;
//         private readonly string _paymentProcessingUrl;
//         private readonly string _userManagementUrl;

//         public CartController(CartContext context, IHttpClientFactory httpClientFactory)
//         {
//             _context = context;
//             _httpClient = httpClientFactory.CreateClient();
//             _paymentProcessingUrl = "http://localhost:5249/payment/charge";
//             _userManagementUrl = "http://localhost:5238/api/users";
//         }

//         // GET: api/Cart
//         [HttpGet]
//         public async Task<ActionResult<IEnumerable<User>>> GetCartItems()
//         {
//             try
//             {
//                 // Fetch cart items from the database
//                 var items = await _context.CartItems.ToListAsync();
//                 return Ok(items);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, $"Internal server error: {ex.Message}");
//             }
//         }

//         // GET: api/Cart/{id}
//         [HttpGet("{id}")]
//         public async Task<ActionResult<User>> GetCartItem(int id)
//         {
//             try
//             {
//                 var item = await _context.CartItems.FindAsync(id);
//                 if (item == null)
//                 {
//                     return NotFound();
//                 }
//                 return Ok(item);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, $"Internal server error: {ex.Message}");
//             }
//         }

//         // POST: api/Cart
//         [HttpPost]
//         public async Task<ActionResult<User>> AddToCart(User user)
//         {
//             try
//             {
//                 _context.CartItems.Add(user);
//                 await _context.SaveChangesAsync();

//                 // Call to payment processing service to process payment
//                 var paymentRequest = new PaymentRequest { Amount = user.Amount, Token = user.Token };
//                 var paymentResponse = await _httpClient.PostAsJsonAsync(_paymentProcessingUrl, paymentRequest);
//                 if (!paymentResponse.IsSuccessStatusCode)
//                 {
//                     // Handle payment failure
//                     return StatusCode((int)paymentResponse.StatusCode, "Payment processing failed");
//                 }

//                 // Call to user management service to create user
//                 var userResponse = await _httpClient.PostAsJsonAsync(_userManagementUrl, user);
//                 if (!userResponse.IsSuccessStatusCode)
//                 {
//                     // Handle user creation failure
//                     return StatusCode((int)userResponse.StatusCode, "User creation failed");
//                 }

//                 return CreatedAtAction(nameof(GetCartItem), new { id = user.Id }, user);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, $"Internal server error: {ex.Message}");
//             }
//         }

//         // PUT: api/Cart/{id}
//         [HttpPut("{id}")]
//         public async Task<IActionResult> UpdateCartItem(int id, User user)
//         {
//             if (id != user.Id)
//             {
//                 return BadRequest();
//             }

//             try
//             {
//                 _context.Entry(user).State = EntityState.Modified;
//                 await _context.SaveChangesAsync();
//                 return NoContent();
//             }
//             catch (DbUpdateConcurrencyException)
//             {
//                 if (!CartItemExists(id))
//                 {
//                     return NotFound();
//                 }
//                 else
//                 {
//                     throw;
//                 }
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, $"Internal server error: {ex.Message}");
//             }
//         }

//         // DELETE: api/Cart/{id}
//         [HttpDelete("{id}")]
//         public async Task<IActionResult> RemoveFromCart(int id)
//         {
//             try
//             {
//                 var item = await _context.CartItems.FindAsync(id);
//                 if (item == null)
//                 {
//                     return NotFound();
//                 }

//                 _context.CartItems.Remove(item);
//                 await _context.SaveChangesAsync();
//                 return NoContent();
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, $"Internal server error: {ex.Message}");
//             }
//         }

//         private bool CartItemExists(int id)
//         {
//             return _context.CartItems.Any(e => e.Id == id);
//         }
//     }
// }
