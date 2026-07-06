using System.Threading.Tasks;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Models;
using B.Unified.Payment.Weixin.PayWay;
using Senparc.Weixin.TenPayV3.Apis;

namespace B.Unified.Payment.Weixin
{
    /// <summary>
    /// 微信支付订单查询 — Senparc SDK OrderQueryByOutTradeNoAsync。
    /// <para>状态映射: SUCCESS → CONFIRM_SUCCESS, USERPAYING → WAITING, CLOSED/REVOKED/PAYERROR → CONFIRM_FAIL, 其他 → UNKNOWN</para>
    /// </summary>
    public class WeixinPayOrderQueryService : IPayOrderQueryService
    {
        public string GetIfCode() => IfCode.WXPAY;

        public async Task<ChannelRetMsg> QueryAsync(string payOrderId, MchAppConfigContext ctx)
        {
            var cfg = ctx.GetNormalMchParams<WxpayNormalMchParams>(IfCode.WXPAY)
                       ?? throw new BizException("未找到微信支付商户配置");

            try
            {
                var result = await WxPayHelper.BuildApi(cfg)
                    .OrderQueryByOutTradeNoAsync(payOrderId, cfg.MchId);

                var tradeState = result?.trade_state;
                if (tradeState == "SUCCESS")
                    return ChannelRetMsg.ConfirmSuccess(result.transaction_id);
                if (tradeState == "USERPAYING")
                    return ChannelRetMsg.Waiting();
                if (tradeState == "CLOSED" || tradeState == "REVOKED" || tradeState == "PAYERROR")
                    return ChannelRetMsg.ConfirmFail();

                return ChannelRetMsg.Unknown();
            }
            catch (System.Exception ex)
            {
                return ChannelRetMsg.SysError(ex.Message);
            }
        }
    }
}
