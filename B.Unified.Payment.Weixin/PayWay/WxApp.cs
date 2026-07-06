using B.Unified.Payment.Abstract;
using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using Newtonsoft.Json;
using Senparc.Weixin.TenPayV3.Apis;
using Senparc.Weixin.TenPayV3.Apis.BasePay;
using Senparc.Weixin.TenPayV3.Helpers;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>微信 APP 支付 — POST /v3/pay/transactions/app（Senparc SDK）</summary>
    public class WxApp : IWxPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        public async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = WxPayHelper.GetConfig(ctx);
            var reqData = WxPayHelper.BuildReqData(rq, cfg);

            PayLogger.LogRequest("Weixin", "WX_APP", "/v3/pay/transactions/app", reqData);

            var result = await WxPayHelper.BuildApi(cfg).AppAsync(reqData);
            var rs = new Models.WxAppOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
            rs.ChannelOriginResponse = JsonConvert.SerializeObject(result);

            if (result.VerifySignSuccess == true && !string.IsNullOrEmpty(result.prepay_id))
            {
                // APP 调起签名复用 JSAPI 签名包（appId/timeStamp/nonceStr/prepayIdPackage/signature）
                var pkg = TenPaySignHelper.GetJsApiUiPackage(cfg.AppId, result.prepay_id, WxPayHelper.BuildSetting(cfg));
                rs.PayInfo = JsonConvert.SerializeObject(pkg);
                rs.ChannelRetMsg = ChannelRetMsg.Waiting();
            }
            else
            {
                var rc = result.ResultCode;
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmFail(rc?.ErrorCode, rc?.ErrorMessage);
            }
            PayLogger.LogResponse("Weixin", "WX_APP", result, rs.ChannelRetMsg);
            return rs;
        }
    }
}
