using System.Threading.Tasks;
using Aop.Api.Domain;
using Aop.Api.Request;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
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
    /// <summary>支付宝手机网站支付（ALI_WAP）</summary>
    public class AliWap : AlipayPayServiceBase
    {
        public override bool IsSupport(string wayCode) => wayCode == AlipayPayWay.WAP;

        protected override string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        protected override Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var holder = AlipayClientFactory.Build(ctx);
            var model = new AlipayTradeWapPayModel
            {
                OutTradeNo = rq.PayOrderId, Subject = rq.Subject, Body = rq.Body,
                TotalAmount = rq.GetAmountYuan(), ProductCode = "QUICK_WAP_PAY",
                TimeExpire = rq.ExpiredTime?.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var req = new AlipayTradeWapPayRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(rq.NotifyUrl);
            req.SetReturnUrl(rq.ReturnUrl);

            PayLogger.LogRequest("Alipay", "ALI_WAP", "alipay.trade.wap.pay", new { model.OutTradeNo, model.Subject, model.TotalAmount });

            var rs = new AliWapOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
            rs.FormContent = holder.Client.pageExecute(req).Body;
            rs.ChannelOriginResponse = rs.FormContent;
            rs.ChannelRetMsg = ChannelRetMsg.Waiting();
            PayLogger.LogResponse("Alipay", "ALI_WAP", new { FormLen = rs.FormContent?.Length }, rs.ChannelRetMsg);
            return Task.FromResult<AbstractRS>(rs);
        }
    }
}
