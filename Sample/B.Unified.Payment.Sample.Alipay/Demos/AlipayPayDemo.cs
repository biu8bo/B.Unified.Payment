using B.Unified.Payment.Abstract.Factory;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Alipay.Constants;
using B.Unified.Payment.Alipay.Extensions;
using B.Unified.Payment.Sample.Alipay.Config;

namespace B.Unified.Payment.Sample.Alipay.Demos;

/// <summary>支付宝支付 Demo — 8 种支付方式示例</summary>
public static class AlipayPayDemo
{
    private static readonly IPaymentServiceFactory _factory =
        PaymentServiceBuilder.Create().AddAlipay().Build();

    public static async Task RunAsync()
    {
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║   支付宝支付 Demo                          ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        await PayAsync("ALI_QR — 扫码支付", AlipayPayWay.QR);
        await PayAsync("ALI_PC — PC网站支付", AlipayPayWay.PC);
        await PayAsync("ALI_WAP — 手机网站支付", AlipayPayWay.WAP);
        await PayAsync("ALI_APP — APP支付", AlipayPayWay.APP);
        await PayAsync("ALI_OC — 订单码支付", AlipayPayWay.OC);

        await PayAsync("ALI_LITE — 小程序支付", AlipayPayWay.LITE, rq =>
            rq.ChannelUserId = "2088000000000000");

        await PayAsync("ALI_BAR — 条码支付", AlipayPayWay.BAR, rq =>
            rq.AuthCode = "289625961689639166");

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
            PayOrderId  = $"TEST{DateTime.Now:yyyyMMddHHmmssfff}",
            MchOrderNo  = $"MCH{DateTime.Now:yyyyMMddHHmmss}",
            WayCode     = wayCode,
            Amount      = 100,
            Subject     = title,
            Body        = title,
            NotifyUrl   = "https://your-domain.com/api/notify/alipay",
            ReturnUrl   = "https://your-domain.com/return",
            ClientIp    = "127.0.0.1"
        };
        setup?.Invoke(rq);

        Console.WriteLine($"  请求: PayOrderId={rq.PayOrderId} Amount={rq.Amount / 100m:F2}元");
        var service = _factory.GetPaymentService(wayCode);
        var rs = (UnifiedOrderRS)await service.PayAsync(rq, AlipayConfig.Context);

        Console.ForegroundColor = rs.ErrCode == null ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"  响应: ErrCode={rs.ErrCode} State={rs.ChannelRetMsg?.State} PayDataType={rs.PayDataType}");
        Console.ResetColor();
    }
}
