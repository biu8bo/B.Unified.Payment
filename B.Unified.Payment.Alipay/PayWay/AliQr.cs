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
    /// <summary>支付宝扫码支付（ALI_QR）</summary>
    public class AliQr : AlipayPayServiceBase
    {
        public override bool IsSupport(string wayCode) => wayCode == AlipayPayWay.QR;

        protected override string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        protected override Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var holder = AlipayClientFactory.Build(ctx);
            var model = new AlipayTradePrecreateModel
            {
                OutTradeNo  = rq.PayOrderId,
                Subject     = rq.Subject,
                Body        = rq.Body,
                TotalAmount = rq.GetAmountYuan(),
                TimeExpire  = rq.ExpiredTime?.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var req = new AlipayTradePrecreateRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(rq.NotifyUrl);

            PayLogger.LogRequest("Alipay", "ALI_QR", "alipay.trade.precreate", new { model.OutTradeNo, model.Subject, model.TotalAmount });

            var resp = holder.Execute(req);
            var rs = new AliQrOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };

            if (!resp.IsError)
            {
                rs.CodeUrl = resp.QrCode;
                var ret = ChannelRetMsg.Waiting();
                ret.ChannelAttach = resp.Body;
                rs.ChannelRetMsg = ret;
                PayLogger.LogResponse("Alipay", "ALI_QR", new { resp.QrCode, resp.Code, resp.Msg }, ret);
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
                PayLogger.LogResponse("Alipay", "ALI_QR", new { resp.Code, resp.SubCode, resp.Msg, resp.SubMsg }, ret);
            }
            return Task.FromResult<AbstractRS>(rs);
        }
    }
}
