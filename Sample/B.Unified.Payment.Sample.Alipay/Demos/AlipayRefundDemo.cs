using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Abstract.Models.Refund;
using B.Unified.Payment.Alipay.Services.Refund;
using B.Unified.Payment.Sample.Alipay.Config;

namespace B.Unified.Payment.Sample.Alipay.Demos;

/// <summary>支付宝退款 Demo — 发起退款 + 查单</summary>
public static class AlipayRefundDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   支付宝退款 Demo                          ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        IRefundService refundService = new AlipayRefundService();

        Console.Write("请输入原支付订单号 (PayOrderId): ");
        var payOrderId = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(payOrderId)) { Console.WriteLine("  跳过（未输入订单号）"); return; }

        Console.Write("请输入渠道订单号 (ChannelOrderNo / TradeNo): ");
        var channelOrderNo = Console.ReadLine()?.Trim();

        var refundOrderId = $"RF{DateTime.Now:yyyyMMddHHmmssfff}";

        Console.Write("退款金额(分): ");
        if (!long.TryParse(Console.ReadLine(), out var refundAmount) || refundAmount <= 0)
        { refundAmount = 1; Console.WriteLine($"  使用默认值: {refundAmount}分"); }

        Console.Write("退款原因: ");
        var reason = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(reason)) reason = "测试退款";

        var rq = new RefundOrderRQ
        {
            PayOrderId     = payOrderId,
            ChannelOrderNo = channelOrderNo,
            RefundOrderId  = refundOrderId,
            RefundAmount   = refundAmount,
            RefundReason   = reason,
            NotifyUrl      = "https://your-domain.com/api/refund/notify/alipay"
        };

        Console.WriteLine($"\n═══ 发起退款 ═══");
        Console.WriteLine($"  退款单号: {refundOrderId}");
        Console.WriteLine($"  退款金额: {refundAmount}分 ({refundAmount / 100m:F2}元)");

        var result = await refundService.RefundAsync(rq, AlipayConfig.Context);
        Console.WriteLine($"  State: {result.State}");
        Console.WriteLine($"  ChannelOrderId: {result.ChannelOrderId}");
        Console.WriteLine($"  ErrCode: {result.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg: {result.ChannelErrMsg}");

        Console.WriteLine($"\n═══ 退款查单 ═══");
        var queryResult = await refundService.QueryAsync(refundOrderId, payOrderId, channelOrderNo, AlipayConfig.Context);
        Console.WriteLine($"  State: {queryResult.State}");
        Console.WriteLine($"  ErrCode: {queryResult.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg: {queryResult.ChannelErrMsg}");

        Console.WriteLine("\n  退款 Demo 执行完毕。");
    }
}
