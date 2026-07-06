using B.Unified.Payment.Abstract.Models;

namespace B.Unified.Payment.Abstract.Models.Refund
{
    /// <summary>
    /// 退款请求参数
    /// </summary>
    public class RefundOrderRQ
    {
        /// <summary>原支付订单号（商户侧 out_trade_no）</summary>
        public string PayOrderId { get; set; }

        /// <summary>原支付金额，单位：分</summary>
        public long? PayOrderAmount { get; set; }

        /// <summary>渠道侧支付订单号（tradeNo / transactionId）</summary>
        public string ChannelOrderNo { get; set; }

        /// <summary>退款订单号（商户侧，唯一）</summary>
        public string RefundOrderId { get; set; }

        /// <summary>退款金额，单位：分</summary>
        public long? RefundAmount { get; set; }

        /// <summary>退款原因</summary>
        public string RefundReason { get; set; }

        /// <summary>货币代码，默认 CNY</summary>
        public string Currency { get; set; } = CurrencyCode.CNY;

        /// <summary>退款异步通知地址</summary>
        public string NotifyUrl { get; set; }
    }
}