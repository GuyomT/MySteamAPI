using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;

namespace ShoppingCart.Data
{
    public class ShoppingCartContext : DbContext
    {
        public ShoppingCartContext(DbContextOptions<ShoppingCartContext> options) : base(options)
        {
        }

        public DbSet<CartItem> CartItems { get; set; }
    }
}
