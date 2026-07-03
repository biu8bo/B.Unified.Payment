using Aop.Api.Domain;
using Aop.Api.Request;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Alipay.Models;

namespace B.Unified.Payment.Alipay.PayWay
{
    /// <summary>
    /// 支付宝订单码支付（ALI_OC）— 用户出示订单二维码，商家扫码收款。
    /// <para>API: alipay.trade.precreate（productCode=QR_CODE_OFFLINE）</para>
    /// <para>返回值：AliOcOrderRS.CodeUrl | ChannelState: WAITING / CONFIRM_FAIL</para>
    /// </summary>
    public class AliOc : IAliPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        public AbstractRS Pay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var client = AlipayClientFactory.Build(ctx);

            var req = new AlipayTradePrecreateRequest();
            req.SetBizModel(new AlipayTradePrecreateModel
            {
                OutTradeNo   = rq.PayOrderId,
                Subject      = rq.Subject,
                Body         = rq.Body,
                TotalAmount  = rq.GetAmountYuan(),
                TimeExpire   = rq.ExpiredTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                ProductCode  = "QR_CODE_OFFLINE"   // 订单码专用的产品码
            });
            req.SetNotifyUrl(rq.NotifyUrl);

            var resp = client.Execute(req);
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
                rs.OrderState = 2; // STATE_FAIL
            }
            return rs;
        }
    }
}