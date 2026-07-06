using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Weixin.Models.Responses
{
    /// <summary>
    /// WX_BAR 付款码支付响应 — 被扫支付即时返回交易状态
    /// </summary>
    public class WxBarOrderRS : UnifiedOrderRS
    {
        /// <summary>交易类型</summary>
        public string TradeType { get; set; }

        /// <summary>微信支付订单号</summary>
        public string TransactionId { get; set; }

        public override PayDataTypeCode BuildPayDataType() => PayDataTypeCode.None;
    }
}