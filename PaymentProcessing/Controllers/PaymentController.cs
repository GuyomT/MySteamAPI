using Microsoft.AspNetCore.Mvc;
using Stripe;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    [HttpPost("charge")]
    public ActionResult CreateCharge([FromBody] PaymentRequest request)
    {
        var options = new ChargeCreateOptions
        {
            Amount = request.Amount,
            Currency = "usd",
            Source = request.Token,
            Description = "Description for the charge"
        };

        var service = new ChargeService();
        try
        {
            Charge charge = service.Create(options);
            return Ok(charge);
        }
        catch (StripeException e)
        {
            return BadRequest(new { error = e.StripeError.Message });
        }
    }
}

public class PaymentRequest
{
    public long Amount { get; set; }
    public string Token { get; set; }
}
