using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Abstract.Models.Refund;
using B.Unified.Payment.Sample.YsfPay.Config;
using B.Unified.Payment.YsfPay.Services.Refund;

namespace B.Unified.Payment.Sample.YsfPay.Demos;

/// <summary>云闪付退款 Demo</summary>
public static class YsfRefundDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   云闪付退款 Demo                          ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        IRefundService refundService = new YsfpayRefundService();

        Console.Write("请输入原支付订单号 (PayOrderId): ");
        var payOrderId = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(payOrderId)) { Console.WriteLine("  跳过"); return; }

        var refundOrderId = $"RF{DateTime.Now:yyyyMMddHHmmssfff}";

        Console.Write("退款金额(分): ");
        if (!long.TryParse(Console.ReadLine(), out var refundAmount)) { refundAmount = 1; }

        Console.Write("退款原因: ");
        var reason = Console.ReadLine()?.Trim() ?? "测试退款";

        var rq = new RefundOrderRQ
        {
            PayOrderId     = payOrderId,
            RefundOrderId  = refundOrderId,
            RefundAmount   = refundAmount,
            RefundReason   = reason,
        };

        // 发起退款
        Console.WriteLine($"\n═══ 发起退款 ═══");
        var result = refundService.RefundAsync(rq, YsfpayConfig.Context).GetAwaiter().GetResult();
        Console.WriteLine($"  State: {result.State}");
        Console.WriteLine($"  ErrCode: {result.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg: {result.ChannelErrMsg}");

        // 查单
        Console.WriteLine($"\n═══ 退款查单 ═══");
        var queryResult = refundService.QueryAsync(refundOrderId, payOrderId, null, YsfpayConfig.Context).GetAwaiter().GetResult();
        Console.WriteLine($"  State: {queryResult.State}");
        Console.WriteLine($"  ErrCode: {queryResult.ChannelErrCode}");
    }
}