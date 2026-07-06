using B.Unified.Payment.Abstract.Factory;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Sample.Weixin.Config;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Extensions;

namespace B.Unified.Payment.Sample.Weixin.Demos;

/// <summary>微信支付 Demo — 6 种支付方式示例</summary>
public static class WeixinPayDemo
{
    private static readonly IPaymentServiceFactory _factory =
        PaymentServiceBuilder.Create().AddWeixin().Build();

    public static async Task RunAsync()
    {
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║   微信支付 Demo                            ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");
        Console.WriteLine("  ⚠ 请先在 WeixinConfig.cs 中替换为真实商户参数");

        await PayAsync("WX_JSAPI — 公众号支付", WxPayWay.JSAPI, rq =>
            rq.ChannelUserId = "oUpF8uMuAJO_M2pxb1Q9zNjWeS6o");

        await PayAsync("WX_NATIVE — 扫码支付", WxPayWay.NATIVE);

        await PayAsync("WX_H5 — H5支付", WxPayWay.H5, rq =>
            rq.ClientIp = "8.8.8.8");

        await PayAsync("WX_APP — APP支付", WxPayWay.APP);

        await PayAsync("WX_LITE — 小程序支付", WxPayWay.LITE, rq =>
            rq.ChannelUserId = "oUpF8uMuAJO_M2pxb1Q9zNjWeS6o");

        await PayAsync("WX_BAR — 付款码支付", WxPayWay.BAR, rq =>
            rq.AuthCode = "130000000000000000");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n═══════════════════════════════════");
        Console.WriteLine("  支付 Demo 执行完毕。");
        Console.WriteLine("═══════════════════════════════════");
        Console.ResetColor();
    }

    private static async Task PayAsync(string title, Abstract.Models.PayWayCode wayCode, Action<UnifiedOrderRQ>? setup = null)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n═══ {title} ═══");
        Console.ResetColor();

        var rq = new UnifiedOrderRQ
        {
            PayOrderId  = $"WX{DateTime.Now:yyyyMMddHHmmssfff}",
            MchOrderNo  = $"MCH{DateTime.Now:yyyyMMddHHmmss}",
            WayCode     = wayCode,
            Amount      = 100,
            Body        = title,
            Subject     = title,
            NotifyUrl   = "https://your-domain.com/api/notify/weixin",
            ClientIp    = "127.0.0.1"
        };
        setup?.Invoke(rq);

        Console.WriteLine($"  请求: PayOrderId={rq.PayOrderId} Amount={rq.Amount / 100m:F2}元");
        var service = _factory.GetPaymentService(wayCode);
        var rs = (UnifiedOrderRS)await service.PayAsync(rq, WeixinConfig.Context);

        Console.ForegroundColor = rs.ErrCode == null ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"  响应: ErrCode={rs.ErrCode} State={rs.ChannelRetMsg?.State} PayDataType={rs.PayDataType}");
        Console.ResetColor();
    }
}
