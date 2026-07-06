using System.Threading.Tasks;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Models;
using Newtonsoft.Json;
using Senparc.Weixin.TenPayV3.Apis.BasePay;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>微信 H5 支付（WX_H5）</summary>
    public class WxH5 : WxPayServiceBase
    {
        public override bool IsSupport(string wayCode) => wayCode == WxPayWay.H5;

        protected override string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        protected override async Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
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
            var rs = new WxH5OrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
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
