using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Alipay.Services.Query;
using B.Unified.Payment.Sample.Alipay.Config;

namespace B.Unified.Payment.Sample.Alipay.Demos;

/// <summary>支付宝订单查询 Demo</summary>
public static class AlipayQueryDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   支付宝查单 Demo                          ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        IPayOrderQueryService queryService = new AlipayPayOrderQueryService();

        Console.Write("请输入要查询的商户订单号 (PayOrderId): ");
        var payOrderId = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(payOrderId))
        {
            Console.WriteLine("  跳过查单（未输入订单号）");
            return;
        }

        Console.WriteLine($"\n  正在查询: {payOrderId}");
        var result = queryService.QueryAsync(payOrderId, AlipayConfig.Context).GetAwaiter().GetResult();

        Console.WriteLine($"  ChannelOrderId: {result.ChannelOrderId}");
        Console.WriteLine($"  State:          {result.State}");
        Console.WriteLine($"  ErrCode:        {result.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg:         {result.ChannelErrMsg}");
    }
}