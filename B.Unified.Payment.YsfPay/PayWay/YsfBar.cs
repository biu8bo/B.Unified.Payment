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
    /// <summary>云闪付条码支付（YSF_BAR）</summary>
    public class YsfBar : YsfPayServiceBase
    {
        public override bool IsSupport(string wayCode) => wayCode == YsfPayWay.BAR;

        protected override string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.AuthCode)) return "条码支付 authCode 不能为空";
            return null;
        }

        protected override async Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = YsfpayConfigHelper.GetConfig(ctx);
            var orderType = YsfHttpUtil.GetPayOrderType("YSF_BAR");

            var reqParams = new JObject
            {
                ["orderNo"] = rq.PayOrderId,
                ["orderType"] = orderType,
                ["txnAmt"] = rq.Amount ?? 0,
                ["authCode"] = rq.AuthCode?.Trim(),
                ["termInfo"] = new JObject { ["longitude"] = "", ["latitude"] = "", ["clientIp"] = rq.ClientIp ?? "127.0.0.1" }.ToString(),
                ["termId"] = "01727367"
            };

            var resJson = await YsfHttpUtil.PackageParamAndReqAsync("/gateway/api/pay/micropay", reqParams, cfg).ConfigureAwait(false);
            var respCode = resJson["respCode"]?.ToString();
            var rs = new YsfBarOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };

            if (respCode == "00")
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmSuccess(resJson["transIndex"]?.ToString());
            else if (respCode == "02" || respCode == "12" || respCode == "99")
                rs.ChannelRetMsg = new ChannelRetMsg { State = ChannelState.WAITING, IsNeedQuery = true };
            else
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmFail(respCode, resJson["respMsg"]?.ToString());

            return rs;
        }
    }
}
