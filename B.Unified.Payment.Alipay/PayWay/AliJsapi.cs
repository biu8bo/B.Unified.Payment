using Aop.Api.Domain;
using System.Threading.Tasks;
using Aop.Api.Request;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Alipay.Models;

namespace B.Unified.Payment.Alipay.PayWay
{
    /// <summary>
    /// 支付宝生活号 / JSAPI 支付（ALI_JSAPI）— 用户在支付宝内嵌 H5 中完成支付。
    /// <para>返回值：AliJsapiOrderRS.AlipayTradeNo | ChannelState: WAITING</para>
    /// </summary>
    public class AliJsapi : IAliPayWay
    {
        /// <summary>前置校验：ChannelUserId(buyerId) 不能为空</summary>
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.ChannelUserId))
                return "JSAPI 支付 ChannelUserId(buyer_id) 不能为空";
            return null;
        }

        /// <summary>执行生活号支付 — 调用 alipay.trade.create 预创建订单</summary>
        public async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
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
            return rs;
        }
    }
}