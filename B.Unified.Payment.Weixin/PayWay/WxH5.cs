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
    /// <summary>微信 H5 支付 — POST /v3/pay/transactions/h5（Senparc SDK）</summary>
    public class WxH5 : IWxPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        public async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = WxPayHelper.GetConfig(ctx);
            var reqData = WxPayHelper.BuildReqData(rq, cfg);
            reqData.scene_info = new TransactionsRequestData.Scene_Info
            {
                payer_client_ip = rq.ClientIp ?? "127.0.0.1",
                h5_info = new TransactionsRequestData.Scene_Info.H5_Info { type = "Wap" }
            };

            PayLogger.LogRequest("Weixin", "WX_H5", "/v3/pay/transactions/h5", reqData);

            var result = await WxPayHelper.BuildApi(cfg).H5Async(reqData);
            var rs = new Models.WxH5OrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
            rs.ChannelOriginResponse = JsonConvert.SerializeObject(result);

            if (result.VerifySignSuccess == true && !string.IsNullOrEmpty(result.h5_url))
            {
                rs.PayUrl = result.h5_url;
                rs.ChannelRetMsg = ChannelRetMsg.Waiting();
            }
            else
            {
                var rc = result.ResultCode;
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmFail(rc?.ErrorCode, rc?.ErrorMessage);
            }
            PayLogger.LogResponse("Weixin", "WX_H5", result, rs.ChannelRetMsg);
            return rs;
        }
    }
}
