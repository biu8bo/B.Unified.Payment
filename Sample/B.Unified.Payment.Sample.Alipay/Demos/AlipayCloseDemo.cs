using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Alipay.Services.Close;
using B.Unified.Payment.Sample.Alipay.Config;

namespace B.Unified.Payment.Sample.Alipay.Demos;

/// <summary>支付宝关单 Demo</summary>
public static class AlipayCloseDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   支付宝关单 Demo                          ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        IPayOrderCloseService closeService = new AlipayPayOrderCloseService();

        Console.Write("请输入要关闭的商户订单号 (PayOrderId): ");
        var payOrderId = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(payOrderId))
        {
            Console.WriteLine("  跳过关单（未输入订单号）");
            return;
        }

        Console.WriteLine($"\n  正在关单: {payOrderId}");
        var result = closeService.CloseAsync(new CloseOrderRQ { PayOrderId = payOrderId }, AlipayConfig.Context)
            .GetAwaiter().GetResult();

        Console.WriteLine($"  ChannelOrderId: {result.ChannelOrderId}");
        Console.WriteLine($"  State:          {result.State}");
        Console.WriteLine($"  ErrCode:        {result.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg:         {result.ChannelErrMsg}");
    }
}
