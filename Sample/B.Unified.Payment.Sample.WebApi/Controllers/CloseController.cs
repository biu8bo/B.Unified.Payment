using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Factory;
using B.Unified.Payment.Abstract.Models.Payment;
using Microsoft.AspNetCore.Mvc;

namespace B.Unified.Payment.Sample.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CloseController : ControllerBase
{
    private readonly IPaymentServiceFactory _factory;

    public CloseController(IPaymentServiceFactory factory) => _factory = factory;

    [HttpPost("close")]
    public async Task<IActionResult> Close([FromBody] CloseRequest req)
    {
        var service = _factory.GetCloseService(req.IfCode);
        var result = await service.CloseAsync(new CloseOrderRQ
        {
            PayOrderId = req.PayOrderId,
            WayCode    = req.WayCode
        }, ConfigHelper.Load(req.IfCode));

        return Ok(new { result.State, result.ChannelOrderId, result.ChannelErrCode, result.ChannelErrMsg });
    }

    public class CloseRequest
    {
        public string IfCode { get; set; }
        public string PayOrderId { get; set; }
        public string WayCode { get; set; }
    }
}
