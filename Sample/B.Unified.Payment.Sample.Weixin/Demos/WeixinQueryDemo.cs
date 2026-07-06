using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Weixin;

namespace B.Unified.Payment.Sample.Weixin;

/// <summary>微信支付订单查询 Demo</summary>
public static class WeixinQueryDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   微信查单 Demo                            ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        IPayOrderQueryService queryService = new WeixinPayOrderQueryService();

        Console.Write("请输入要查询的商户订单号 (PayOrderId): ");
        var payOrderId = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(payOrderId))
        {
            Console.WriteLine("  跳过查单（未输入订单号）");
            return;
        }

        Console.WriteLine($"\n  正在查询: {payOrderId}");
        var result = queryService.QueryAsync(payOrderId, WeixinConfig.Context).GetAwaiter().GetResult();

        Console.WriteLine($"  ChannelOrderId: {result.ChannelOrderId}");
        Console.WriteLine($"  State:          {result.State}");
        Console.WriteLine($"  ErrCode:        {result.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg:         {result.ChannelErrMsg}");
    }
}