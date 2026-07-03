using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>微信付款码支付（WX_BAR）— 用户出示付款码，商家扫码收款</summary>
    public class WxBar : IWxPayWay
    {
        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.AuthCode)) return "条码支付 AuthCode 不能为空";
            return null;
        }

        public AbstractRS Pay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = WxPayHelper.GetConfig(ctx);
            var body = new { appid = cfg.AppId, mchid = cfg.MchId, description = rq.Body,
                out_trade_no = rq.PayOrderId, auth_code = rq.AuthCode?.Trim(),
                amount = new { total = rq.GetAmountFen(), currency = rq.Currency ?? "CNY" },
                scene_info = new { payer_client_ip = rq.ClientIp ?? "127.0.0.1" }, notify_url = rq.NotifyUrl };

            PayLogger.LogRequest("Weixin", "WX_BAR", "/v3/pay/transactions/micropay", new { body.appid, body.mchid, body.out_trade_no, body.amount });

            var resp = WxPayHelper.PostJson(cfg, "/v3/pay/transactions/micropay", body);
            var rs = new Models.WxBarOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
            var ret = new ChannelRetMsg();
            var tradeState = resp["trade_state"]?.ToString();

            if (tradeState != null)
            {
                rs.TransactionId = resp["transaction_id"]?.ToString();
                rs.TradeType     = resp["trade_type"]?.ToString();
                ret.ChannelOrderId = rs.TransactionId;
                ret.ChannelUserId  = resp["openid"]?.ToString(); // 与 Java 一致: getOpenid()

                switch (tradeState)
                {
                    case "SUCCESS":
                        ret.State = ChannelRetMsg.ChannelState.CONFIRM_SUCCESS; break;
                    case "USERPAYING":
                        ret.State = ChannelRetMsg.ChannelState.WAITING; ret.IsNeedQuery = true; break;
                    case "PAYERROR":
                        ret.State = ChannelRetMsg.ChannelState.CONFIRM_FAIL; break;
                    default:
                        ret.State = ChannelRetMsg.ChannelState.WAITING; ret.IsNeedQuery = true; break;
                }
            }
            else
            {
                ret = Fail(resp);
            }

            rs.ChannelRetMsg = ret;
            PayLogger.LogResponse("Weixin", "WX_BAR", new { trade_state = tradeState, transaction_id = rs.TransactionId, openid = ret.ChannelUserId }, ret);
            return rs;
        }

        private static ChannelRetMsg Fail(JObject r) =>
            ChannelRetMsg.ConfirmFail(r["code"]?.ToString(), r["message"]?.ToString());
    }
}