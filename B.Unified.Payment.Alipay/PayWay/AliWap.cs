using Aop.Api.Domain;
using Aop.Api.Request;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Alipay.PayWay
{
    /// <summary>支付宝手机网站支付（ALI_WAP）</summary>
    public class AliWap : IAliPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        public AbstractRS Pay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var client = AlipayClientFactory.Build(ctx);
            var model = new AlipayTradeWapPayModel
            {
                OutTradeNo = rq.PayOrderId, Subject = rq.Subject, Body = rq.Body,
                TotalAmount = rq.GetAmountYuan(), ProductCode = "QUICK_WAP_PAY",
                TimeExpire = rq.ExpiredTime?.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var req = new AlipayTradeWapPayRequest(); req.SetBizModel(model); req.SetNotifyUrl(rq.NotifyUrl); req.SetReturnUrl(rq.ReturnUrl);

            PayLogger.LogRequest("Alipay", "ALI_WAP", "alipay.trade.wap.pay", new { model.OutTradeNo, model.Subject, model.TotalAmount });

            var rs = new Models.AliWapOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
            rs.FormContent = client.pageExecute(req).Body;
            rs.ChannelOriginResponse = rs.FormContent;
            rs.ChannelRetMsg = ChannelRetMsg.Waiting();
            PayLogger.LogResponse("Alipay", "ALI_WAP", new { FormLen = rs.FormContent?.Length }, rs.ChannelRetMsg);
            return rs;
        }
    }
}