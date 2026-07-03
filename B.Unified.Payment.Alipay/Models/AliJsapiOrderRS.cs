using B.Unified.Payment.Abstract.Models.Payment;
using Newtonsoft.Json;

namespace B.Unified.Payment.Alipay.Models
{
    /// <summary>
    /// ALI_JSAPI 生活号/JSAPI支付响应 — 预创建订单，包含支付宝交易号。
    /// <para>前端通过 alipayTradeNo 调起 JSSDK 完成支付。</para>
    /// </summary>
    public class AliJsapiOrderRS : UnifiedOrderRS
    {
        /// <summary>支付宝交易号（tradeNo），前端 JSSDK 调起支付需使用</summary>
        public string AlipayTradeNo { get; set; }

        public override string BuildPayDataType() => "aliapp";

        public override string BuildPayData()
            => string.IsNullOrEmpty(AlipayTradeNo) ? "" : JsonConvert.SerializeObject(new { alipayTradeNo = AlipayTradeNo });
    }
}