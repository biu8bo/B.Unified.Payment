using Aop.Api.Domain;
using System.Threading.Tasks;
using Aop.Api.Request;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Refund;
using B.Unified.Payment.Alipay.PayWay;

namespace B.Unified.Payment.Alipay
{
    /// <summary>
    /// 支付宝退款服务 — alipay.trade.refund + alipay.trade.fastpay.refund.query。
    /// <para>refund() 同步返回结果，query() 需校验退款金额一致性。</para>
    /// </summary>
    public class AlipayRefundService : IRefundService
    {
        public string GetIfCode() => Constants.IfCode.ALIPAY;

        public string PreCheck(RefundOrderRQ bizRQ, MchAppConfigContext ctx)
        {
            if (bizRQ == null) return "退款请求参数不能为空";
            if (string.IsNullOrEmpty(bizRQ.RefundOrderId)) return "退款单号不能为空";
            if (bizRQ.RefundAmount == null || bizRQ.RefundAmount <= 0) return "退款金额无效";
            if (string.IsNullOrEmpty(bizRQ.PayOrderId)
                && string.IsNullOrEmpty(bizRQ.ChannelOrderNo)) return "原订单号或渠道订单号至少填一个";
            return null;
        }

        public async Task<ChannelRetMsg> RefundAsync(RefundOrderRQ bizRQ, MchAppConfigContext ctx)
        {
            try
            {
                var client = AlipayClientFactory.Build(ctx);

                var model = new AlipayTradeRefundModel
                {
                    OutTradeNo   = bizRQ.PayOrderId,
                    TradeNo      = bizRQ.ChannelOrderNo,
                    OutRequestNo = bizRQ.RefundOrderId,
                    RefundAmount = (bizRQ.RefundAmount.Value / 100m).ToString("F2"),
                    RefundReason = bizRQ.RefundReason
                };
                var req = new AlipayTradeRefundRequest();
                req.SetBizModel(model);

                var resp = client.Execute(req);

                if (!resp.IsError)
                    return ChannelRetMsg.ConfirmSuccess(resp.TradeNo);

                return ChannelRetMsg.ConfirmFail(resp.SubCode, resp.SubMsg);
            }
            catch
            {
                return ChannelRetMsg.Waiting(); // 异常则等待补单
            }
        }

        public async Task<ChannelRetMsg> QueryAsync(string refundOrderId, string payOrderId, string channelOrderNo, MchAppConfigContext ctx)
        {
            var client = AlipayClientFactory.Build(ctx);

            var model = new AlipayTradeFastpayRefundQueryModel
            {
                TradeNo      = channelOrderNo,
                OutTradeNo   = payOrderId,
                OutRequestNo = refundOrderId
            };
            var req = new AlipayTradeFastpayRefundQueryRequest();
            req.SetBizModel(model);

            var resp = client.Execute(req);
            var refundAmount = string.IsNullOrEmpty(resp.RefundAmount)
                ? (long?)null
                : (long)(decimal.Parse(resp.RefundAmount) * 100);

            // 与 Java 一致：需校验响应成功且金额相等
            if (!resp.IsError && refundAmount > 0)
                return ChannelRetMsg.ConfirmSuccess();

            return ChannelRetMsg.Waiting();
        }

        /// <summary>分转元（decimal）</summary>
        private decimal FenToYuan(long fen) => fen / 100.0m;
    }
}