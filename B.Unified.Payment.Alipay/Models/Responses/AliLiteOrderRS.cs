using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Alipay.Models.Responses
{
    /// <summary>
    /// ALI_LITE 小程序支付响应 — 预创建订单，包含支付宝交易号
    /// </summary>
    public class AliLiteOrderRS : UnifiedOrderRS
    {
        /// <summary>支付宝交易号</summary>
        public string AlipayTradeNo { get; set; }

        public override PayDataTypeCode BuildPayDataType() => PayDataTypeCode.None;
    }
}