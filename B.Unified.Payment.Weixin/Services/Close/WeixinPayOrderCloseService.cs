using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Abstract.Models.Channel;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Infrastructure;
using B.Unified.Payment.Weixin.Models.MchParams;
using Senparc.Weixin.TenPayV3.Apis.BasePay;

namespace B.Unified.Payment.Weixin.Services.Close
{
    /// <summary>微信关单 — V3 POST /v3/pay/transactions/out-trade-no/{out_trade_no}/close</summary>
    public class WeixinPayOrderCloseService : IPayOrderCloseService
    {
        public string GetIfCode() => IfCode.WXPAY;

        public async Task<ChannelRetMsg> CloseAsync(CloseOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = ctx.GetNormalMchParams<WxpayNormalMchParams>(IfCode.WXPAY)
                       ?? throw new BizException("未找到微信支付商户配置");

            try
            {
                var data = new CloseRequestData(cfg.MchId, rq.PayOrderId);
                await WxPayHelper.BuildApi(cfg).CloseOrderAsync(data);
                return ChannelRetMsg.ConfirmSuccess();
            }
            catch (System.Exception ex)
            {
                return ChannelRetMsg.SysError(ex.Message);
            }
        }
    }
}
