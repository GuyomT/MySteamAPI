

namespace ProductCatalog.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string BirthDate { get; set; }
        public string City { get; set; }
        public int UserBalance { get; set; } = 1000;
        // public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        // public List<Invoice> Invoices { get; set; }
    }

}
