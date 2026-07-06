using B.Unified.Payment.Abstract;
using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin.Models;
using Newtonsoft.Json;
using Senparc.Weixin.TenPayV3.Apis;
using Senparc.Weixin.TenPayV3.Apis.BasePay;
using Senparc.Weixin.TenPayV3.Helpers;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>微信 JSAPI 支付 — POST /v3/pay/transactions/jsapi（Senparc SDK）</summary>
    public class WxJsapi : IWxPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.ChannelUserId)) return "JSAPI 支付 openid 不能为空";
            return null;
        }

        public async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = WxPayHelper.GetConfig(ctx);
            var reqData = WxPayHelper.BuildReqData(rq, cfg);
            reqData.payer = new TransactionsRequestData.Payer(openid: rq.ChannelUserId);

            PayLogger.LogRequest("Weixin", "WX_JSAPI", "/v3/pay/transactions/jsapi", reqData);

            var result = await WxPayHelper.BuildApi(cfg).JsApiAsync(reqData);
            var rs = new WxJsapiOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
            rs.ChannelOriginResponse = JsonConvert.SerializeObject(result);

            if (result.VerifySignSuccess == true && !string.IsNullOrEmpty(result.prepay_id))
            {
                var pkg = TenPaySignHelper.GetJsApiUiPackage(cfg.AppId, result.prepay_id, WxPayHelper.BuildSetting(cfg));
                rs.PayInfo = JsonConvert.SerializeObject(pkg);
                rs.ChannelRetMsg = ChannelRetMsg.Waiting();
            }
            else
            {
                var rc = result.ResultCode;
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmFail(rc?.ErrorCode, rc?.ErrorMessage);
            }
            PayLogger.LogResponse("Weixin", "WX_JSAPI", result, rs.ChannelRetMsg);
            return rs;
        }
    }
}
