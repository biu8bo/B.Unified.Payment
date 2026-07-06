using System.Threading.Tasks;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Base;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Models;
using B.Unified.Payment.Weixin.Models.Responses;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>微信小程序支付（WX_LITE）</summary>
    public class WxLite : WxPayServiceBase
    {
        public override bool IsSupport(string wayCode) => wayCode == WxPayWay.LITE;

        protected override string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.ChannelUserId)) return "JSAPI 支付 openid 不能为空";
            return null;
        }

        protected override async Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var result = await new WxJsapi().PayAsync(rq, ctx);
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
