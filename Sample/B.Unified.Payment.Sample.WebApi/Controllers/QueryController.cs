using B.Unified.Payment.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace B.Unified.Payment.Sample.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueryController : ControllerBase
{
    private readonly IPaymentServiceFactory _factory;

    public QueryController(IPaymentServiceFactory factory) => _factory = factory;

    [HttpGet("query")]
    public async Task<IActionResult> Query([FromQuery] string ifCode, [FromQuery] string payOrderId)
    {
        var service = _factory.GetQueryService(ifCode);
        var result = await service.QueryAsync(payOrderId, ConfigHelper.Load(ifCode));
        return Ok(new { result.State, result.ChannelOrderId, result.ChannelErrCode, result.ChannelErrMsg });
    }
}