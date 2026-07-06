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
    /// <summary>支付宝条码支付（ALI_BAR）</summary>
    public class AliBar : AlipayPayServiceBase
    {
        public override bool IsSupport(string wayCode) => wayCode == AlipayPayWay.BAR;

        protected override string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.AuthCode)) return "条码支付 authCode 不能为空";
            return null;
        }

        protected override Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var client = AlipayClientFactory.Build(ctx);
            var model = new AlipayTradePayModel
            {
                OutTradeNo = rq.PayOrderId, Scene = "bar_code", AuthCode = rq.AuthCode?.Trim(),
                Subject = rq.Subject, Body = rq.Body, TotalAmount = rq.GetAmountYuan(),
                TimeExpire = rq.ExpiredTime?.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var req = new AlipayTradePayRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(rq.NotifyUrl);

            PayLogger.LogRequest("Alipay", "ALI_BAR", "alipay.trade.pay", new { model.OutTradeNo, model.Scene, model.TotalAmount });

            var resp = client.Execute(req);
            var rs = new AliBarOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
            rs.ChannelOriginResponse = resp.Body;
            var ret = new ChannelRetMsg
            {
                ChannelAttach = resp.Body, ChannelOrderId = resp.TradeNo, ChannelUserId = resp.BuyerUserId
            };

            if ("10000".Equals(resp.Code) && !resp.IsError)
                ret.State = ChannelState.CONFIRM_SUCCESS;
            else if ("10003".Equals(resp.Code))
                ret.State = ChannelState.WAITING;
            else
            {
                ret.State = ChannelState.CONFIRM_FAIL;
                ret.ChannelErrCode = resp.SubCode ?? resp.Code;
                ret.ChannelErrMsg = resp.SubMsg ?? resp.Msg;
            }
            rs.ChannelRetMsg = ret;
            PayLogger.LogResponse("Alipay", "ALI_BAR", new { resp.Code, resp.TradeNo, resp.BuyerUserId }, ret);
            return Task.FromResult<AbstractRS>(rs);
        }
    }
}
