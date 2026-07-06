using System.Threading.Tasks;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Models;
using Newtonsoft.Json;
using Senparc.Weixin.TenPayV3.Apis.BasePay;
using Senparc.Weixin.TenPayV3.Helpers;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>微信 APP 支付（WX_APP）</summary>
    public class WxApp : WxPayServiceBase
    {
        public override bool IsSupport(string wayCode) => wayCode == WxPayWay.APP;

        protected override string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        protected override async Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = WxPayHelper.GetConfig(ctx);
            var reqData = WxPayHelper.BuildReqData(rq, cfg);

            PayLogger.LogRequest("Weixin", "WX_APP", "/v3/pay/transactions/app", reqData);

            var result = await WxPayHelper.BuildApi(cfg).AppAsync(reqData);
            var rs = new WxAppOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
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
            PayLogger.LogResponse("Weixin", "WX_APP", result, rs.ChannelRetMsg);
            return rs;
        }
    }
}
