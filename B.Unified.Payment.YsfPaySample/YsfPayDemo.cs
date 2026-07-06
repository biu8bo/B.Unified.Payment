using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.YsfPay;

namespace B.Unified.Payment.YsfPaySample;

/// <summary>云闪付支付 Demo — 条码支付 + JSAPI 支付</summary>
public static class YsfPayDemo
{
    public static void Run()
    {
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║   云闪付支付 Demo                          ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");
        Console.WriteLine("  ⚠ 请先在 YsfpayConfig.cs 中替换为真实商户参数");

        var service = new YsfpayPaymentService();

        // 1) JSAPI 支付
        Console.WriteLine("\n═══ YSF_JSAPI — JSAPI 支付 ═══");
        var jsapiRq = new UnifiedOrderRQ
        {
            PayOrderId  = $"YSF{DateTime.Now:yyyyMMddHHmmssfff}",
            MchOrderNo  = $"MCH{DateTime.Now:yyyyMMddHHmmss}",
            WayCode     = PayWayCode.Of("YSF_JSAPI"),
            Amount      = 100,
            Body        = "云闪付JSAPI测试",
            Subject     = "云闪付JSAPI测试",
            NotifyUrl   = "https://your-domain.com/api/notify/ysf",
            ReturnUrl   = "https://your-domain.com/return",
            ChannelUserId = "user_id",
            ClientIp    = "127.0.0.1"
        };

        Console.WriteLine($"  请求: PayOrderId={jsapiRq.PayOrderId} Amount={jsapiRq.Amount / 100m:F2}元");
        var jsapiRs = (UnifiedOrderRS)service.PayAsync(jsapiRq, YsfpayConfig.Context).GetAwaiter().GetResult();
        Console.WriteLine($"  响应: ErrCode={jsapiRs.ErrCode} State={jsapiRs.ChannelRetMsg?.State}");
        Console.WriteLine($"  PayDataType={jsapiRs.PayDataType} PayData={jsapiRs.PayData?.Truncate(100)}");

        // 2) 条码支付
        Console.WriteLine("\n═══ YSF_BAR — 条码支付 ═══");
        var barRq = new UnifiedOrderRQ
        {
            PayOrderId  = $"YSF{DateTime.Now:yyyyMMddHHmmssfff}",
            MchOrderNo  = $"MCH{DateTime.Now:yyyyMMddHHmmss}",
            WayCode     = PayWayCode.Of("YSF_BAR"),
            Amount      = 100,
            Body        = "云闪付条码测试",
            Subject     = "云闪付条码测试",
            NotifyUrl   = "https://your-domain.com/api/notify/ysf",
            AuthCode    = "6200000000000000000",
            ClientIp    = "127.0.0.1"
        };

        Console.WriteLine($"  请求: PayOrderId={barRq.PayOrderId} Amount={barRq.Amount / 100m:F2}元");
        var barRs = (UnifiedOrderRS)service.PayAsync(barRq, YsfpayConfig.Context).GetAwaiter().GetResult();
        Console.WriteLine($"  响应: ErrCode={barRs.ErrCode} State={barRs.ChannelRetMsg?.State}");
        Console.WriteLine($"  IsNeedQuery={barRs.ChannelRetMsg?.IsNeedQuery}");

        Console.WriteLine("\n═══════════════════════════════════");
        Console.WriteLine("  支付 Demo 执行完毕。");
        Console.WriteLine("═══════════════════════════════════");
    }
}

internal static class StringExtensions
{
    public static string Truncate(this string? s, int max)
        => string.IsNullOrEmpty(s) ? "" : s.Length <= max ? s : s[..max] + "...";
}