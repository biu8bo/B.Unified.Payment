using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models.Refund;
using B.Unified.Payment.Alipay;

namespace B.Unified.Payment.AlipaySample;

/// <summary>支付宝退款 Demo — 发起退款 + 查单</summary>
public static class AlipayRefundDemo
{
    public static void Run()
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

        // 生成退款单号
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

        // 1) 发起退款
        Console.WriteLine($"\n═══ 发起退款 ═══");
        Console.WriteLine($"  退款单号: {refundOrderId}");
        Console.WriteLine($"  退款金额: {refundAmount}分 ({refundAmount / 100m:F2}元)");

        var result = refundService.Refund(rq, AlipayConfig.Context);
        Console.WriteLine($"  State: {result.State}");
        Console.WriteLine($"  ChannelOrderId: {result.ChannelOrderId}");
        Console.WriteLine($"  ErrCode: {result.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg: {result.ChannelErrMsg}");

        // 2) 查单
        Console.WriteLine($"\n═══ 退款查单 ═══");
        var queryResult = refundService.Query(refundOrderId, payOrderId, channelOrderNo, AlipayConfig.Context);
        Console.WriteLine($"  State: {queryResult.State}");
        Console.WriteLine($"  ErrCode: {queryResult.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg: {queryResult.ChannelErrMsg}");

        Console.WriteLine("\n  退款 Demo 执行完毕。");
    }
}