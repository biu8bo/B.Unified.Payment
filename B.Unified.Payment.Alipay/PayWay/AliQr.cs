using Aop.Api.Domain;
using System.Threading.Tasks;
using Aop.Api.Request;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Alipay.Models;
using Newtonsoft.Json;

namespace B.Unified.Payment.Alipay.PayWay
{
    /// <summary>
    /// 支付宝扫码支付（ALI_QR）— 商家展示二维码，用户扫码支付。
    /// <para>返回值：AliQrOrderRS.CodeUrl | ChannelState: WAITING</para>
    /// </summary>
    public class AliQr : IAliPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        public async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var client = AlipayClientFactory.Build(ctx);
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

            var resp = client.Execute(req);
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
            return rs;
        }
    }
}