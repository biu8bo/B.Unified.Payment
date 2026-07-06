using B.Unified.Payment.Abstract;
using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.YsfPay.PayWay
{
    /// <summary>云闪付条码支付（YSF_BAR）— 用户出示付款码，商家扫码收款</summary>
    public class YsfBar : IYsfPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.AuthCode)) return "条码支付 authCode 不能为空";
            return null;
        }

        public async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
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

            var resJson = YsfHttpUtil.PackageParamAndReq("/gateway/api/pay/micropay", reqParams, cfg);
            var respCode = resJson["respCode"]?.ToString();
            var rs = new Models.YsfBarOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };

            if (respCode == "00")
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmSuccess(resJson["transIndex"]?.ToString());
            else if (respCode == "02" || respCode == "12" || respCode == "99")
                rs.ChannelRetMsg = new ChannelRetMsg { State = ChannelRetMsg.ChannelState.WAITING, IsNeedQuery = true };
            else
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmFail(respCode, resJson["respMsg"]?.ToString());

            return rs;
        }
    }
}