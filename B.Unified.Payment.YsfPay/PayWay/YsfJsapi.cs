using System.Threading.Tasks;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Base;
using B.Unified.Payment.Abstract.Models.Channel;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.YsfPay.Constants;
using B.Unified.Payment.YsfPay.Models;
using B.Unified.Payment.YsfPay.Models.Responses;
using B.Unified.Payment.YsfPay.Utils;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.YsfPay.PayWay
{
    /// <summary>云闪付 JSAPI 支付（YSF_JSAPI）</summary>
    public class YsfJsapi : YsfPayServiceBase
    {
        public override bool IsSupport(string wayCode) => wayCode == YsfPayWay.JSAPI;

        protected override string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.ChannelUserId)) return "JSAPI 支付 ChannelUserId 不能为空";
            return null;
        }

        protected override Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
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
            var rs = new YsfJsapiOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };

            if (respCode == "00")
            {
                rs.RedirectUrl = resJson["redirectUrl"]?.ToString();
                rs.ChannelRetMsg = ChannelRetMsg.Waiting();
            }
            else
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmFail(respCode, resJson["respMsg"]?.ToString());

            return Task.FromResult<AbstractRS>(rs);
        }
    }
}
