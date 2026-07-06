namespace B.Unified.Payment.Abstract.Models.Payment
{
    /// <summary>统一关单请求参数</summary>
    public class CloseOrderRQ
    {
        /// <summary>支付订单号（对应渠道 out_trade_no / orderNo）</summary>
        public string PayOrderId { get; set; }

        /// <summary>支付方式代码（云闪付关单必填）</summary>
        public string WayCode { get; set; }
    }
}
