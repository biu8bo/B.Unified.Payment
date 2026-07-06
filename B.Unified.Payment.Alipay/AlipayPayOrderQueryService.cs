using Aop.Api.Domain;
using Aop.Api.Request;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Alipay.PayWay;

namespace B.Unified.Payment.Alipay
{
    /// <summary>
    /// 支付宝订单查询 — 调用 alipay.trade.query 查询订单状态。
    /// <para>状态映射: TRADE_SUCCESS → CONFIRM_SUCCESS, WAIT_BUYER_PAY → WAITING, 其他 → WAITING</para>
    /// </summary>
    public class AlipayPayOrderQueryService : IPayOrderQueryService
    {
        public string GetIfCode() => Constants.IfCode.ALIPAY;

        public ChannelRetMsg Query(string payOrderId, MchAppConfigContext ctx)
        {
            var client = AlipayClientFactory.Build(ctx);

            var req = new AlipayTradeQueryRequest();
            req.SetBizModel(new AlipayTradeQueryModel { OutTradeNo = payOrderId });

            var resp = client.Execute(req);

            if ("TRADE_SUCCESS".Equals(resp.TradeStatus))
                return ChannelRetMsg.ConfirmSuccess(resp.TradeNo);

            // WAIT_BUYER_PAY 或其他状态 → 等待中
            return ChannelRetMsg.Waiting();
        }
    }
}