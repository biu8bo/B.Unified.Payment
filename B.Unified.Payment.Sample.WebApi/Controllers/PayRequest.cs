using System.Text.Json.Serialization;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Sample.WebApi.Json;

namespace B.Unified.Payment.Sample.WebApi.Controllers;

public class PayRequest
{
    public string IfCode { get; set; } = "alipay";

    [JsonConverter(typeof(PayWayCodeJsonConverter))]
    public PayWayCode WayCode { get; set; } = PayWayCode.Of("ALI_QR");

    public long Amount { get; set; } = 100;
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? NotifyUrl { get; set; }
    public string? ReturnUrl { get; set; }
    public string? ChannelUserId { get; set; }
    public string? AuthCode { get; set; }
    public string? ClientIp { get; set; }
}
