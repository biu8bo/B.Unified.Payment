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
    /// <summary>支付宝订单码支付（ALI_OC）</summary>
    public class AliOc : AlipayPayServiceBase
    {
        public override bool IsSupport(string wayCode) => wayCode == AlipayPayWay.OC;

        protected override string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        protected override Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var holder = AlipayClientFactory.Build(ctx);
            var req = new AlipayTradePrecreateRequest();
            req.SetBizModel(new AlipayTradePrecreateModel
            {
                OutTradeNo   = rq.PayOrderId,
                Subject      = rq.Subject,
                Body         = rq.Body,
                TotalAmount  = rq.GetAmountYuan(),
                TimeExpire   = rq.ExpiredTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                ProductCode  = "QR_CODE_OFFLINE"
            });
            req.SetNotifyUrl(rq.NotifyUrl);

            var resp = holder.Execute(req);
            var rs = new AliOcOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };

            if (!resp.IsError)
            {
                rs.CodeUrl = resp.QrCode;
                var ret = ChannelRetMsg.Waiting();
                ret.ChannelAttach = resp.Body;
                rs.ChannelRetMsg = ret;
            }
            else
            {
                var ret = new ChannelRetMsg
                {
                    ChannelErrCode = resp.SubCode ?? resp.Code,
                    ChannelErrMsg  = resp.SubMsg  ?? resp.Msg,
                    ChannelAttach  = resp.Body
                };
                rs.ChannelRetMsg = ret;
                rs.OrderState = PayOrderState.Failed;
            }
            return Task.FromResult<AbstractRS>(rs);
        }
    }
}
