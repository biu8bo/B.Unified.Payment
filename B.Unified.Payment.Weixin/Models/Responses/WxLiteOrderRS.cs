using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Weixin.Models.Responses
{
    /// <summary>
    /// WX_LITE 小程序支付响应 — 返回小程序调起支付参数 JSON（与 JSAPI 相同）
    /// </summary>
    public class WxLiteOrderRS : UnifiedOrderRS
    {
        /// <summary>小程序调起支付参数字符串</summary>
        public string PayInfo { get; set; }

        public override PayDataTypeCode BuildPayDataType() => PayDataTypeCode.WxApp;

        public override string BuildPayData() => PayInfo ?? "";
    }
}