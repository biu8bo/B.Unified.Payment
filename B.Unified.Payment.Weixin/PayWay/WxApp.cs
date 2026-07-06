using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>微信 APP 支付 — POST /v3/pay/transactions/app</summary>
    public class WxApp : IWxPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        public AbstractRS Pay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = WxPayHelper.GetConfig(ctx);
            var body = new
            {
                appid = cfg.AppId, mchid = cfg.MchId, description = rq.Body,
                out_trade_no = rq.PayOrderId, time_expire = rq.ExpiredTime?.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                notify_url = rq.NotifyUrl, amount = new { total = rq.GetAmountFen(), currency = rq.Currency ?? "CNY" },
                settle_info = Division(rq)
            };

            PayLogger.LogRequest("Weixin", "WX_APP", "/v3/pay/transactions/app", body);

            var resp = WxPayHelper.PostJson(cfg, "/v3/pay/transactions/app", body);
            var rs = new Models.WxAppOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
            rs.ChannelOriginResponse = WxPayHelper.ToJsonString(resp);

            if (resp["prepay_id"] != null)
            {
                rs.PayInfo = WxPayHelper.GenAppPaySign(cfg, resp["prepay_id"].ToString());
                rs.ChannelRetMsg = ChannelRetMsg.Waiting();
            }
            else
            {
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmFail(resp["code"]?.ToString(), resp["message"]?.ToString());
            }
            PayLogger.LogResponse("Weixin", "WX_APP", resp, rs.ChannelRetMsg);
            return rs;
        }

        private static object Division(UnifiedOrderRQ rq) =>
            (rq.DivisionMode == 1 || rq.DivisionMode == 2) ? new { profit_sharing = true } : null;
    }
}