using B.Unified.Payment.Abstract;
using B.Unified.Payment.YsfPay;

namespace B.Unified.Payment.Sample.YsfPay;

/// <summary>云闪付查单 Demo</summary>
public static class YsfQueryDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   云闪付查单 Demo                          ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        IPayOrderQueryService queryService = new YsfpayPayOrderQueryService();

        Console.Write("请输入商户订单号 (PayOrderId): ");
        var payOrderId = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(payOrderId)) { Console.WriteLine("  跳过"); return; }

        var result = queryService.QueryAsync(payOrderId, YsfpayConfig.Context).GetAwaiter().GetResult();
        Console.WriteLine($"  ChannelOrderId: {result.ChannelOrderId}");
        Console.WriteLine($"  State: {result.State}");
        Console.WriteLine($"  ErrCode: {result.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg: {result.ChannelErrMsg}");
    }
}