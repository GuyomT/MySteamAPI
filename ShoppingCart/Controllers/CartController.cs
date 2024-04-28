using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.Controllers
{
    [ApiController]
    [Route("/api/cart")]
    public class CartController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CartController(IConfiguration config)
        {
            _config = config;
        }

    }
}