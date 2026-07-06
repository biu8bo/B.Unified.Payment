using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Refund;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.YsfPay
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

        public ChannelRetMsg Refund(RefundOrderRQ bizRQ, MchAppConfigContext ctx)
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

            var resJson = YsfHttpUtil.PackageParamAndReq("/gateway/api/pay/refund", reqParams, cfg);
            if (resJson == null) return new ChannelRetMsg { State = ChannelRetMsg.ChannelState.UNKNOWN };

            var respCode = resJson["respCode"]?.ToString();
            if (respCode == "00")
                return ChannelRetMsg.ConfirmSuccess(bizRQ.RefundOrderId);

            return ChannelRetMsg.ConfirmFail(respCode, resJson["respMsg"]?.ToString());
        }

        public ChannelRetMsg Query(string refundOrderId, string payOrderId, string channelOrderNo, MchAppConfigContext ctx)
        {
            var cfg = YsfpayConfigHelper.GetConfig(ctx);

            var reqParams = new JObject
            {
                ["orderNo"] = refundOrderId,
                ["origOrderNo"] = payOrderId,
                ["orderType"] = YsfHttpUtil.GetOrderType("YSF_BAR")
            };

            var resJson = YsfHttpUtil.PackageParamAndReq("/gateway/api/pay/refundQuery", reqParams, cfg);
            if (resJson == null) return new ChannelRetMsg { State = ChannelRetMsg.ChannelState.UNKNOWN };

            var respCode = resJson["respCode"]?.ToString();
            if (respCode != "00") return new ChannelRetMsg { State = ChannelRetMsg.ChannelState.UNKNOWN };

            var origRespCode = resJson["origRespCode"]?.ToString();
            return origRespCode switch
            {
                "00" => ChannelRetMsg.ConfirmSuccess(),
                "01" => ChannelRetMsg.ConfirmFail(respCode, resJson["respMsg"]?.ToString()),
                "02" => ChannelRetMsg.Waiting(),
                _    => new ChannelRetMsg { State = ChannelRetMsg.ChannelState.UNKNOWN }
            };
        }
    }
}