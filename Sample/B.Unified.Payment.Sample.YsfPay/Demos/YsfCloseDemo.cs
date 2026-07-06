using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Sample.YsfPay.Config;
using B.Unified.Payment.YsfPay.Constants;
using B.Unified.Payment.YsfPay.Services.Close;

namespace B.Unified.Payment.Sample.YsfPay.Demos;

/// <summary>云闪付关单 Demo</summary>
public static class YsfCloseDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   云闪付关单 Demo                          ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        IPayOrderCloseService closeService = new YsfpayPayOrderCloseService();

        Console.Write("请输入要关闭的商户订单号 (PayOrderId): ");
        var payOrderId = Console.ReadLine()?.Trim();
        Console.Write($"请输入支付方式 (默认 {YsfPayWay.BAR}): ");
        var wayCode = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(payOrderId))
        {
            Console.WriteLine("  跳过关单（未输入订单号）");
            return;
        }

        if (string.IsNullOrEmpty(wayCode))
            wayCode = YsfPayWay.BAR;

        Console.WriteLine($"\n  正在关单: {payOrderId} ({wayCode})");
        var result = closeService.CloseAsync(new CloseOrderRQ
        {
            PayOrderId = payOrderId,
            WayCode    = wayCode
        }, YsfpayConfig.Context).GetAwaiter().GetResult();

        Console.WriteLine($"  ChannelOrderId: {result.ChannelOrderId}");
        Console.WriteLine($"  State:          {result.State}");
        Console.WriteLine($"  ErrCode:        {result.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg:         {result.ChannelErrMsg}");
    }
}
