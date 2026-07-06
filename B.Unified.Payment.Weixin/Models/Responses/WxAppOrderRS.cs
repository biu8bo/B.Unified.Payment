using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Weixin.Models
{
    /// <summary>
    /// WX_APP APP支付响应 — 返回 APP 调起支付参数 JSON
    /// </summary>
    public class WxAppOrderRS : UnifiedOrderRS
    {
        /// <summary>APP 调起支付参数字符串（appid/partnerid/prepayid/package/noncestr/timestamp/sign）</summary>
        public string PayInfo { get; set; }

        public override PayDataTypeCode BuildPayDataType() => PayDataTypeCode.WxApp;

        public override string BuildPayData() => PayInfo ?? "";
    }
}