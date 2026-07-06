using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Abstract.Models.Refund;
using B.Unified.Payment.Sample.Weixin.Config;
using B.Unified.Payment.Weixin.Services.Refund;

namespace B.Unified.Payment.Sample.Weixin.Demos;

/// <summary>微信退款 Demo — 发起退款 + 查单</summary>
public static class WeixinRefundDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   微信退款 Demo                            ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        IRefundService refundService = new WeixinRefundService();

        Console.Write("请输入原支付订单号 (PayOrderId): ");
        var payOrderId = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(payOrderId)) { Console.WriteLine("  跳过（未输入订单号）"); return; }

        Console.Write("原支付金额(分): ");
        if (!long.TryParse(Console.ReadLine(), out var payAmount) || payAmount <= 0)
        { Console.WriteLine("  跳过（金额无效）"); return; }

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
            PayOrderAmount = payAmount,
            RefundOrderId  = refundOrderId,
            RefundAmount   = refundAmount,
            RefundReason   = reason,
            NotifyUrl      = "https://your-domain.com/api/refund/notify/weixin"
        };

        // 1) 发起退款
        Console.WriteLine($"\n═══ 发起退款 ═══");
        Console.WriteLine($"  退款单号: {refundOrderId}");
        Console.WriteLine($"  原金额: {payAmount}分  退款: {refundAmount}分");

        var result = refundService.RefundAsync(rq, WeixinConfig.Context).GetAwaiter().GetResult();
        Console.WriteLine($"  State: {result.State}");
        Console.WriteLine($"  ChannelOrderId: {result.ChannelOrderId}");
        Console.WriteLine($"  ErrCode: {result.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg: {result.ChannelErrMsg}");

        // 2) 查单
        Console.WriteLine($"\n═══ 退款查单 ═══");
        var queryResult = refundService.QueryAsync(refundOrderId, payOrderId, null, WeixinConfig.Context).GetAwaiter().GetResult();
        Console.WriteLine($"  State: {queryResult.State}");
        Console.WriteLine($"  ErrCode: {queryResult.ChannelErrCode}");
        Console.WriteLine($"  ErrMsg: {queryResult.ChannelErrMsg}");

        Console.WriteLine("\n  退款 Demo 执行完毕。");
    }
}