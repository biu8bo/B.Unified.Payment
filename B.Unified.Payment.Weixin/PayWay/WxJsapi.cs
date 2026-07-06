using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin.Models;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>微信 JSAPI 支付 — POST /v3/pay/transactions/jsapi</summary>
    public class WxJsapi : IWxPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.ChannelUserId)) return "JSAPI 支付 openid 不能为空";
            return null;
        }

        public AbstractRS Pay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = WxPayHelper.GetConfig(ctx);
            var body = new
            {
                appid = cfg.AppId, mchid = cfg.MchId, description = rq.Body,
                out_trade_no = rq.PayOrderId, time_expire = rq.ExpiredTime?.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                notify_url = rq.NotifyUrl, amount = new { total = rq.GetAmountFen(), currency = rq.Currency ?? "CNY" },
                payer = new { openid = rq.ChannelUserId },
                scene_info = string.IsNullOrEmpty(rq.ClientIp) ? null : new { payer_client_ip = rq.ClientIp },
                settle_info = Division(rq)
            };

            PayLogger.LogRequest("Weixin", "WX_JSAPI", "/v3/pay/transactions/jsapi", body);

            var resp = WxPayHelper.PostJson(cfg, "/v3/pay/transactions/jsapi", body);
            var rs = new WxJsapiOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
            rs.ChannelOriginResponse = WxPayHelper.ToJsonString(resp);

            if (resp["prepay_id"] != null)
            {
                rs.PayInfo = WxPayHelper.GenJsApiSign(cfg, resp["prepay_id"].ToString());
                rs.ChannelRetMsg = ChannelRetMsg.Waiting();
            }
            else
            {
                rs.ChannelRetMsg = ChannelRetMsg.ConfirmFail(resp["code"]?.ToString(), resp["message"]?.ToString());
            }
            PayLogger.LogResponse("Weixin", "WX_JSAPI", resp, rs.ChannelRetMsg);
            return rs;
        }

        private static object Division(UnifiedOrderRQ rq) =>
            (rq.DivisionMode == 1 || rq.DivisionMode == 2) ? new { profit_sharing = true } : null;
    }
}