using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Abstract.Models.Channel;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.YsfPay.Utils;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.YsfPay.Services.Query
{
    /// <summary>云闪付订单查询 — /gateway/api/pay/queryOrder</summary>
    public class YsfpayPayOrderQueryService : IPayOrderQueryService
    {
        public string GetIfCode() => Constants.IfCode.YSFPAY;

        public async Task<ChannelRetMsg> QueryAsync(string payOrderId, MchAppConfigContext ctx)
        {
            var cfg = YsfpayConfigHelper.GetConfig(ctx);

            var reqParams = new JObject
            {
                ["orderNo"] = payOrderId,
                ["orderType"] = YsfHttpUtil.GetOrderType(payOrderId?.Contains("YSF") == true ? "YSF_JSAPI" : "YSF_BAR")
            };

            var resJson = await YsfHttpUtil.PackageParamAndReqAsync("/gateway/api/pay/queryOrder", reqParams, cfg).ConfigureAwait(false);
            if (resJson == null) return ChannelRetMsg.Waiting();

            var respCode = resJson["respCode"]?.ToString();
            if (respCode != "00") return ChannelRetMsg.Waiting();

            var origRespCode = resJson["origRespCode"]?.ToString();
            if (origRespCode == "00")
                return ChannelRetMsg.ConfirmSuccess(resJson["transIndex"]?.ToString());
            // 02(未支付) / 05(用户支付中) → 等待
            return ChannelRetMsg.Waiting();
        }
    }
}