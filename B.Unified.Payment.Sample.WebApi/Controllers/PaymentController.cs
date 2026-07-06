using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Sample.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace B.Unified.Payment.Sample.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentServiceFactory _factory;

    public PaymentController(IPaymentServiceFactory factory) => _factory = factory;

    [HttpPost("pay")]
    public async Task<IActionResult> Pay([FromBody] PayRequest req)
    {
        var service = _factory.GetPaymentService(req.WayCode);
        var rq = new UnifiedOrderRQ
        {
            PayOrderId     = $"TEST{DateTime.Now:yyyyMMddHHmmssfff}",
            MchOrderNo     = $"MCH{DateTime.Now:yyyyMMddHHmmss}",
            WayCode        = req.WayCode,
            Amount         = req.Amount,
            Subject        = req.Subject ?? req.WayCode,
            Body           = req.Body ?? req.Subject ?? req.WayCode,
            NotifyUrl      = req.NotifyUrl ?? "https://your-domain.com/api/notify",
            ReturnUrl      = req.ReturnUrl,
            ChannelUserId  = req.ChannelUserId,
            AuthCode       = req.AuthCode,
            ClientIp       = req.ClientIp ?? "127.0.0.1"
        };

        var rs = (UnifiedOrderRS)await service.PayAsync(rq, ConfigHelper.Load(req.IfCode));
        return Ok(rs);
    }
}
