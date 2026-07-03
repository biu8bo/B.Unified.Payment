using Aop.Api.Domain;
using Aop.Api.Request;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Alipay.PayWay
{
    /// <summary>支付宝 APP 支付（ALI_APP）— 返回 SDK 调起参数字符串</summary>
    public class AliApp : IAliPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        public AbstractRS Pay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var client = AlipayClientFactory.Build(ctx);
            var model = new AlipayTradeAppPayModel
            {
                OutTradeNo = rq.PayOrderId, Subject = rq.Subject, Body = rq.Body,
                TotalAmount = rq.GetAmountYuan(), ProductCode = "QUICK_MSECURITY_PAY",
                TimeExpire = rq.ExpiredTime?.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var req = new AlipayTradeAppPayRequest(); req.SetBizModel(model); req.SetNotifyUrl(rq.NotifyUrl);

            PayLogger.LogRequest("Alipay", "ALI_APP", "alipay.trade.app.pay", new { model.OutTradeNo, model.Subject, model.TotalAmount });

            var payData = client.SdkExecute(req).Body;
            var rs = new Models.AliAppOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo, PayData = payData };
            var ret = ChannelRetMsg.Waiting();
            ret.ChannelAttach = payData;
            rs.ChannelRetMsg = ret;
            PayLogger.LogResponse("Alipay", "ALI_APP", new { PayDataLen = payData?.Length }, ret);
            return rs;
        }
    }
}