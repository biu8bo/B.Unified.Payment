using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Alipay.Models
{
    /// <summary>
    /// ALI_APP APP支付响应 — 返回 APP 调起支付参数字符串（SdkExecute 结果）
    /// </summary>
    public class AliAppOrderRS : UnifiedOrderRS
    {
        public override string BuildPayDataType() => "aliapp";

        public override string BuildPayData() => PayData ?? "";
    }
}