using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>微信 H5 支付（WX_H5）</summary>
    public class WxH5 : IWxPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        public AbstractRS Pay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = WxPayHelper.GetConfig(ctx);
            var body = new { appid = cfg.AppId, mchid = cfg.MchId, description = rq.Body,
                out_trade_no = rq.PayOrderId, time_expire = rq.ExpiredTime?.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                notify_url = rq.NotifyUrl, amount = new { total = rq.GetAmountFen(), currency = rq.Currency ?? "CNY" },
                scene_info = new { payer_client_ip = rq.ClientIp ?? "127.0.0.1", h5_info = new { type = "Wap" } },
                settle_info = Division(rq) };

            PayLogger.LogRequest("Weixin", "WX_H5", "/v3/pay/transactions/h5", new { body.appid, body.mchid, body.out_trade_no, body.amount });

            var resp = WxPayHelper.PostJson(cfg, "/v3/pay/transactions/h5", body);
            var rs = new Models.WxH5OrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };

            if (resp["h5_url"] != null)
            {
                rs.PayUrl = resp["h5_url"].ToString();
                rs.ChannelRetMsg = ChannelRetMsg.Waiting();
                PayLogger.LogResponse("Weixin", "WX_H5", new { h5_url = rs.PayUrl }, rs.ChannelRetMsg);
            }
            else
            {
                rs.ChannelRetMsg = Fail(resp);
                PayLogger.LogResponse("Weixin", "WX_H5", resp, rs.ChannelRetMsg);
            }
            return rs;
        }

        private static object Division(UnifiedOrderRQ rq) =>
            (rq.DivisionMode == 1 || rq.DivisionMode == 2) ? new { profit_sharing = true } : null;

        private static ChannelRetMsg Fail(JObject r) =>
            ChannelRetMsg.ConfirmFail(r["code"]?.ToString(), r["message"]?.ToString());
    }
}