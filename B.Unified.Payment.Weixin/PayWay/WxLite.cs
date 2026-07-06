using B.Unified.Payment.Abstract;
using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin.Models;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>
    /// 微信小程序支付（WX_LITE）— 用户在微信小程序中完成支付。
    /// <para>与 JSAPI 完全一致，复用 POST /v3/pay/transactions/jsapi。</para>
    /// <para>返回值：WxLiteOrderRS.PayInfo (小程序调起参数 JSON) | ChannelState: WAITING</para>
    /// </summary>
    public class WxLite : IWxPayWay
    {
        private readonly WxJsapi _jsapi = new WxJsapi();

        /// <summary>前置校验：复用 WxJsapi 的校验逻辑</summary>
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
            => _jsapi.PreCheck(rq, ctx);

        /// <summary>执行小程序支付 — 复用 JSAPI 支付逻辑</summary>
        public async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var result = await _jsapi.PayAsync(rq, ctx);
            // 如果有 prepay_id 返回，包装为 WxLiteOrderRS
            if (result is WxJsapiOrderRS jsapiRs && jsapiRs.PayInfo != null)
            {
                return new WxLiteOrderRS
                {
                    PayOrderId = rq.PayOrderId,
                    MchOrderNo = rq.MchOrderNo,
                    PayInfo = jsapiRs.PayInfo,
                    ChannelRetMsg = jsapiRs.ChannelRetMsg
                };
            }
            return result;
        }
    }
}