using System.Threading.Tasks;
using Aop.Api.Domain;
using Aop.Api.Request;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Base;
using B.Unified.Payment.Abstract.Models.Channel;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Alipay.Constants;
using B.Unified.Payment.Alipay.Infrastructure;
using B.Unified.Payment.Alipay.Models;
using B.Unified.Payment.Alipay.Models.Responses;

namespace B.Unified.Payment.Alipay.PayWay
{
    /// <summary>支付宝小程序支付（ALI_LITE）</summary>
    public class AliLite : AlipayPayServiceBase
    {
        public override bool IsSupport(string wayCode) => wayCode == AlipayPayWay.LITE;

        protected override string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.ChannelUserId))
                return "小程序支付 ChannelUserId(buyer_id) 不能为空";
            return null;
        }

        protected override Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var holder = AlipayClientFactory.Build(ctx);
            var req = new AlipayTradeCreateRequest();
            req.SetBizModel(new AlipayTradeCreateModel
            {
                OutTradeNo  = rq.PayOrderId,
                Subject     = rq.Subject,
                Body        = rq.Body,
                TotalAmount = rq.GetAmountYuan(),
                BuyerId     = rq.ChannelUserId,
                TimeExpire  = rq.ExpiredTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                OpAppId     = ctx.AppId
            });
            req.SetNotifyUrl(rq.NotifyUrl);

            var resp = holder.Execute(req);
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
            return Task.FromResult<AbstractRS>(rs);
        }
    }
}
