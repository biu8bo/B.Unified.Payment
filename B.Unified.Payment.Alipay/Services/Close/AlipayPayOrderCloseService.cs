using System.Threading.Tasks;
using Aop.Api.Domain;
using Aop.Api.Request;
using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Abstract.Models.Channel;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Alipay.Infrastructure;

namespace B.Unified.Payment.Alipay.Services.Close
{
    /// <summary>支付宝关单 — 调用 alipay.trade.close</summary>
    public class AlipayPayOrderCloseService : IPayOrderCloseService
    {
        public string GetIfCode() => Constants.IfCode.ALIPAY;

        public Task<ChannelRetMsg> CloseAsync(CloseOrderRQ rq, MchAppConfigContext ctx)
        {
            var holder = AlipayClientFactory.Build(ctx);

            var req = new AlipayTradeCloseRequest();
            req.SetBizModel(new AlipayTradeCloseModel { OutTradeNo = rq.PayOrderId });

            var resp = holder.Execute(req);

            if (!resp.IsError)
                return Task.FromResult(ChannelRetMsg.ConfirmSuccess(resp.TradeNo));

            return Task.FromResult(ChannelRetMsg.SysError(resp.SubMsg ?? resp.Msg));
        }
    }
}
