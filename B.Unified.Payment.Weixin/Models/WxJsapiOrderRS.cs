using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Weixin.Models
{
    /// <summary>
    /// WX_JSAPI 公众号支付响应 — 返回 JSAPI 调起支付参数 JSON
    /// </summary>
    public class WxJsapiOrderRS : UnifiedOrderRS
    {
        /// <summary>JSAPI 调起支付参数字符串（appId/timeStamp/nonceStr/package/paySign）</summary>
        public string PayInfo { get; set; }

        public override string BuildPayDataType() => "wxapp";

        public override string BuildPayData() => PayInfo ?? "";
    }
}