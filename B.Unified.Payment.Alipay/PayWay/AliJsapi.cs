using System.Threading.Tasks;
using Aop.Api.Domain;
using Aop.Api.Request;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Alipay.Constants;
using B.Unified.Payment.Alipay.Models;

namespace B.Unified.Payment.Alipay.PayWay
{
    /// <summary>支付宝生活号 / JSAPI 支付（ALI_JSAPI）</summary>
    public class AliJsapi : AlipayPayServiceBase
    {
        public override bool IsSupport(string wayCode) => wayCode == AlipayPayWay.JSAPI;

        protected override string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.ChannelUserId))
                return "JSAPI 支付 ChannelUserId(buyer_id) 不能为空";
            return null;
        }

        protected override Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
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
                TimeExpire  = rq.ExpiredTime?.ToString("yyyy-MM-dd HH:mm:ss")
            });
            req.SetNotifyUrl(rq.NotifyUrl);

            var resp = client.Execute(req);
            var rs = new AliJsapiOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
            var ret = new ChannelRetMsg
            {
                ChannelAttach  = resp.Body,
                ChannelOrderId = resp.TradeNo
            };

            if (resp.IsError)
            {
                ret.State = ChannelState.CONFIRM_FAIL;
                ret.ChannelErrCode = resp.SubCode ?? resp.Code;
                ret.ChannelErrMsg  = resp.SubMsg  ?? resp.Msg;
            }
            else
            {
                ret.State = ChannelState.WAITING;
                rs.AlipayTradeNo = resp.TradeNo;
            }
            rs.ChannelRetMsg = ret;
            return Task.FromResult<AbstractRS>(rs);
        }
    }
}
