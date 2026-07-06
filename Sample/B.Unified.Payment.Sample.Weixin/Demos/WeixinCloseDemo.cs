using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Sample.Weixin.Config;
using B.Unified.Payment.Weixin.Services.Close;

namespace B.Unified.Payment.Sample.Weixin.Demos;

/// <summary>微信关单 Demo</summary>
public static class WeixinCloseDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   微信关单 Demo                            ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        IPayOrderCloseService closeService = new WeixinPayOrderCloseService();

        Console.Write("请输入要关闭的商户订单号 (PayOrderId): ");
        var payOrderId = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(payOrderId))
        {
            Console.WriteLine("  跳过关单（未输入订单号）");
            return;
        }

        Console.WriteLine($"\n  正在关单: {payOrderId}");
        var result = closeService.CloseAsync(new CloseOrderRQ { PayOrderId = payOrderId }, WeixinConfig.Context)
            .GetAwaiter().GetResult();

        Console.WriteLine($"  ChannelOrderId: {result.ChannelOrderId}");
        Console.WriteLine($"  State:          {result.State}");
        Console.WriteLine($"  ErrCode:        {result.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg:         {result.ChannelErrMsg}");
    }
}
