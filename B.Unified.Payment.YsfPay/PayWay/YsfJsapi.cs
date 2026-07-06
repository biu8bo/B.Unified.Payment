using B.Unified.Payment.Abstract;
using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.YsfPay.PayWay
{
    /// <summary>云闪付 JSAPI 支付（YSF_JSAPI）— 用户在云闪付 APP 内完成支付</summary>
    public class YsfJsapi : IYsfPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.ChannelUserId)) return "JSAPI 支付 ChannelUserId 不能为空";
            return null;
        }

        public async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = YsfpayConfigHelper.GetConfig(ctx);
            var orderType = YsfHttpUtil.GetPayOrderType("YSF_JSAPI");

            var reqParams = new JObject
            {
                ["orderNo"] = rq.PayOrderId,
                ["orderType"] = orderType,
                ["txnAmt"] = rq.Amount ?? 0,
                ["termInfo"] = new JObject { ["clientIp"] = rq.ClientIp ?? "127.0.0.1" }.ToString(),
                ["backUrl"] = rq.NotifyUrl,
                ["frontUrl"] = rq.ReturnUrl
            };

            var resJson = YsfHttpUtil.PackageParamAndReq("/gateway/api/pay/unifiedorder", reqParams, cfg);
            var respCode = resJson["respCode"]?.ToString();
            var rs = new Models.YsfJsapiOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };

            if (respCode == "00")
            {
                rs.RedirectUrl = resJson["redirectUrl"]?.ToString();
                rs.ChannelRetMsg = ChannelRetMsg.Waiting();
            }
            else
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmFail(respCode, resJson["respMsg"]?.ToString());

            return rs;
        }
    }
}