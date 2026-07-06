using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Abstract.Models.Channel;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Abstract.Models.Refund;
using B.Unified.Payment.YsfPay.Utils;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.YsfPay.Services.Refund
{
    /// <summary>云闪付退款服务 — 发起退款 + 退款查单</summary>
    public class YsfpayRefundService : IRefundService
    {
        public string GetIfCode() => Constants.IfCode.YSFPAY;

        public string PreCheck(RefundOrderRQ bizRQ, MchAppConfigContext ctx)
        {
            if (bizRQ == null) return "退款请求参数不能为空";
            if (string.IsNullOrEmpty(bizRQ.RefundOrderId)) return "退款单号不能为空";
            if (bizRQ.RefundAmount == null || bizRQ.RefundAmount <= 0) return "退款金额无效";
            if (string.IsNullOrEmpty(bizRQ.PayOrderId)) return "原支付订单号不能为空";
            return null;
        }

        public async Task<ChannelRetMsg> RefundAsync(RefundOrderRQ bizRQ, MchAppConfigContext ctx)
        {
            var cfg = YsfpayConfigHelper.GetConfig(ctx);

            var reqParams = new JObject
            {
                ["origOrderNo"] = bizRQ.PayOrderId,
                ["origTxnAmt"] = bizRQ.PayOrderAmount ?? 0,
                ["orderNo"] = bizRQ.RefundOrderId,
                ["txnAmt"] = bizRQ.RefundAmount ?? 0,
                ["orderType"] = YsfHttpUtil.GetOrderType("YSF_BAR")
            };

            var resJson = await YsfHttpUtil.PackageParamAndReqAsync("/gateway/api/pay/refund", reqParams, cfg).ConfigureAwait(false);
            if (resJson == null) return new ChannelRetMsg { State = ChannelState.UNKNOWN };

            var respCode = resJson["respCode"]?.ToString();
            if (respCode == "00")
                return ChannelRetMsg.ConfirmSuccess(bizRQ.RefundOrderId);

            return ChannelRetMsg.ConfirmFail(respCode, resJson["respMsg"]?.ToString());
        }

        public async Task<ChannelRetMsg> QueryAsync(string refundOrderId, string payOrderId, string channelOrderNo, MchAppConfigContext ctx)
        {
            var cfg = YsfpayConfigHelper.GetConfig(ctx);

            var reqParams = new JObject
            {
                ["orderNo"] = refundOrderId,
                ["origOrderNo"] = payOrderId,
                ["orderType"] = YsfHttpUtil.GetOrderType("YSF_BAR")
            };

            var resJson = await YsfHttpUtil.PackageParamAndReqAsync("/gateway/api/pay/refundQuery", reqParams, cfg).ConfigureAwait(false);
            if (resJson == null) return new ChannelRetMsg { State = ChannelState.UNKNOWN };

            var respCode = resJson["respCode"]?.ToString();
            if (respCode != "00") return new ChannelRetMsg { State = ChannelState.UNKNOWN };

            var origRespCode = resJson["origRespCode"]?.ToString();
            return origRespCode switch
            {
                "00" => ChannelRetMsg.ConfirmSuccess(),
                "01" => ChannelRetMsg.ConfirmFail(respCode, resJson["respMsg"]?.ToString()),
                "02" => ChannelRetMsg.Waiting(),
                _    => new ChannelRetMsg { State = ChannelState.UNKNOWN }
            };
        }
    }
}