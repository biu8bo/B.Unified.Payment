using Aop.Api.Domain;
using Aop.Api.Request;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Alipay.Models;

namespace B.Unified.Payment.Alipay.PayWay
{
    /// <summary>
    /// 支付宝小程序支付（ALI_LITE）— 用户在支付宝小程序中完成支付。
    /// <para>同 ALI_JSAPI，调用 alipay.trade.create 预创建订单。</para>
    /// <para>返回值：AliLiteOrderRS.AlipayTradeNo | ChannelState: WAITING</para>
    /// </summary>
    public class AliLite : IAliPayWay
    {
        /// <summary>前置校验：ChannelUserId(buyerId) 不能为空</summary>
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.ChannelUserId))
                return "小程序支付 ChannelUserId(buyer_id) 不能为空";
            return null;
        }

        /// <summary>执行小程序支付 — 调用 alipay.trade.create</summary>
        public AbstractRS Pay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var client = AlipayClientFactory.Build(ctx);

            var req = new AlipayTradeCreateRequest();
            req.SetBizModel(new AlipayTradeCreateModel
            {
                OutTradeNo  = rq.PayOrderId,
                Subject     = rq.Subject,
                Body        = rq.Body,
                TotalAmount = rq.GetAmountYuan(),
                BuyerId     = rq.ChannelUserId,
                TimeExpire  = rq.ExpiredTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                OpAppId     = ctx.AppId             // 小程序 appId
            });
            req.SetNotifyUrl(rq.NotifyUrl);

            var resp = client.Execute(req);
            var rs = new AliLiteOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };

            if (!resp.IsError)
            {
                rs.AlipayTradeNo = resp.TradeNo;
                rs.ChannelRetMsg = ChannelRetMsg.Waiting();
            }
            else
            {
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmFail(
                    resp.SubCode ?? resp.Code, resp.SubMsg ?? resp.Msg);
            }
            return rs;
        }
    }
}