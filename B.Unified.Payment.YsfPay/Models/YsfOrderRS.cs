using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.YsfPay.Models
{
    /// <summary>云闪付 JSAPI 支付响应 — 返回 redirectUrl 用于调起云闪付插件</summary>
    public class YsfJsapiOrderRS : UnifiedOrderRS
    {
        /// <summary>调起云闪付插件的 URL</summary>
        public string RedirectUrl { get; set; }

        public override string BuildPayDataType() => "ysfapp";

        public override string BuildPayData()
            => string.IsNullOrEmpty(RedirectUrl) ? "" : Newtonsoft.Json.JsonConvert.SerializeObject(new { redirectUrl = RedirectUrl });
    }

    /// <summary>云闪付条码支付响应 — 即时返回结果，无需支付数据</summary>
    public class YsfBarOrderRS : UnifiedOrderRS
    {
        public override string BuildPayDataType() => "none";
    }
}