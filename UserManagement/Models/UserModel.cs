

namespace UserManagement.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string City { get; set; }
        public decimal UserBalance { get; set; } = 1000;
        // public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        // public List<Invoice> Invoices { get; set; }
    }

}
