using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin.Models;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>微信 JSAPI 支付（WX_JSAPI）</summary>
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
            var body = new { appid = cfg.AppId, mchid = cfg.MchId, description = rq.Body,
                out_trade_no = rq.PayOrderId, time_expire = rq.ExpiredTime?.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                notify_url = rq.NotifyUrl, amount = new { total = rq.GetAmountFen(), currency = rq.Currency ?? "CNY" },
                payer = new { openid = rq.ChannelUserId },
                scene_info = string.IsNullOrEmpty(rq.ClientIp) ? null : new { payer_client_ip = rq.ClientIp },
                settle_info = Division(rq) };

            PayLogger.LogRequest("Weixin", "WX_JSAPI", "/v3/pay/transactions/jsapi", new { body.appid, body.mchid, body.out_trade_no, body.amount });

            var resp = WxPayHelper.PostJson(cfg, "/v3/pay/transactions/jsapi", body);
            var rs = new WxJsapiOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };

            if (resp["prepay_id"] != null)
            {
                var prepayId = resp["prepay_id"].ToString();
                rs.PayInfo = WxPayHelper.GenJsApiSign(cfg, prepayId);
                rs.ChannelRetMsg = ChannelRetMsg.Waiting();
                PayLogger.LogResponse("Weixin", "WX_JSAPI", new { prepay_id = prepayId }, rs.ChannelRetMsg);
            }
            else
            {
                rs.ChannelRetMsg = Fail(resp);
                PayLogger.LogResponse("Weixin", "WX_JSAPI", resp, rs.ChannelRetMsg);
            }
            return rs;
        }

        private static object Division(UnifiedOrderRQ rq) =>
            (rq.DivisionMode == 1 || rq.DivisionMode == 2) ? new { profit_sharing = true } : null;

        private static ChannelRetMsg Fail(JObject r) =>
            ChannelRetMsg.ConfirmFail(r["code"]?.ToString(), r["message"]?.ToString());
    }
}