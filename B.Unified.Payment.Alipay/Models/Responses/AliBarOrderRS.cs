using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Alipay.Models.Responses
{
    /// <summary>
    /// ALI_BAR 条码支付响应 — 被扫支付即时返回结果，无需返回支付数据给客户端
    /// </summary>
    public class AliBarOrderRS : UnifiedOrderRS
    {
        public override PayDataTypeCode BuildPayDataType() => PayDataTypeCode.None;
    }
}