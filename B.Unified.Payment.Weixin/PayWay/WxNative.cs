using B.Unified.Payment.Abstract;
using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using Newtonsoft.Json;
using Senparc.Weixin.TenPayV3.Apis;
using Senparc.Weixin.TenPayV3.Apis.BasePay;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>微信 Native 扫码支付 — POST /v3/pay/transactions/native（Senparc SDK）</summary>
    public class WxNative : IWxPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        public async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = WxPayHelper.GetConfig(ctx);
            var reqData = WxPayHelper.BuildReqData(rq, cfg);

            PayLogger.LogRequest("Weixin", "WX_NATIVE", "/v3/pay/transactions/native", reqData);

            var result = await WxPayHelper.BuildApi(cfg).NativeAsync(reqData);
            var rs = new Models.WxNativeOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
            rs.ChannelOriginResponse = JsonConvert.SerializeObject(result);

            if (result.VerifySignSuccess == true && !string.IsNullOrEmpty(result.code_url))
            {
                rs.CodeUrl = result.code_url;
                rs.ChannelRetMsg = ChannelRetMsg.Waiting();
            }
            else
            {
                var rc = result.ResultCode;
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmFail(rc?.ErrorCode, rc?.ErrorMessage);
            }
            PayLogger.LogResponse("Weixin", "WX_NATIVE", result, rs.ChannelRetMsg);
            return rs;
        }
    }
}
