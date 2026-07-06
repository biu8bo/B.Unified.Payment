using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models.Refund;
using Microsoft.AspNetCore.Mvc;

namespace B.Unified.Payment.Sample.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefundController : ControllerBase
{
    private readonly IPaymentServiceFactory _factory;

    public RefundController(IPaymentServiceFactory factory) => _factory = factory;

    [HttpPost("refund")]
    public async Task<IActionResult> Refund([FromBody] RefundRequest req)
    {
        var service = _factory.GetRefundService(req.IfCode);
        var rq = new RefundOrderRQ
        {
            PayOrderId     = req.PayOrderId,
            ChannelOrderNo = req.ChannelOrderNo,
            RefundOrderId  = $"RF{DateTime.Now:yyyyMMddHHmmssfff}",
            PayOrderAmount = req.PayAmount,
            RefundAmount   = req.RefundAmount,
            RefundReason   = req.Reason ?? "API退款"
        };
        var result = await service.RefundAsync(rq, ConfigHelper.Load(req.IfCode));
        return Ok(new { result.State, result.ChannelOrderId, result.ChannelErrCode, result.ChannelErrMsg });
    }

    [HttpGet("refund-query")]
    public async Task<IActionResult> RefundQuery(
        [FromQuery] string ifCode, [FromQuery] string refundOrderId,
        [FromQuery] string payOrderId, [FromQuery] string? channelOrderNo)
    {
        var service = _factory.GetRefundService(ifCode);
        var result = await service.QueryAsync(refundOrderId, payOrderId, channelOrderNo, ConfigHelper.Load(ifCode));
        return Ok(new { result.State, result.ChannelErrCode, result.ChannelErrMsg });
    }
}

public class RefundRequest
{
    public string IfCode { get; set; } = "alipay";
    public string PayOrderId { get; set; } = "";
    public string? ChannelOrderNo { get; set; }
    public long? PayAmount { get; set; }
    public long RefundAmount { get; set; } = 1;
    public string? Reason { get; set; }
}