using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Base;
using B.Unified.Payment.Abstract.Models.Channel;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Infrastructure;
using B.Unified.Payment.Weixin.Models;
using B.Unified.Payment.Weixin.Models.MchParams;
using B.Unified.Payment.Weixin.Models.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>微信付款码支付（WX_BAR）</summary>
    public class WxBar : WxPayServiceBase
    {
        private const string BaseUrl = "https://api.mch.weixin.qq.com";
        private static readonly HttpClient _http = new HttpClient();

        public override bool IsSupport(string wayCode) => wayCode == WxPayWay.BAR;

        protected override string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.AuthCode)) return "条码支付 AuthCode 不能为空";
            return null;
        }

        protected override async Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = WxPayHelper.GetConfig(ctx);
            const string path = "/v3/pay/transactions/micropay";
            var body = new
            {
                appid = cfg.AppId, mchid = cfg.MchId, description = rq.Body,
                out_trade_no = rq.PayOrderId, auth_code = rq.AuthCode?.Trim(),
                amount = new { total = rq.GetAmountFen(), currency = rq.Currency ?? "CNY" },
                scene_info = new { payer_client_ip = rq.ClientIp ?? "127.0.0.1" }, notify_url = rq.NotifyUrl
            };
            var json = JsonConvert.SerializeObject(body,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            PayLogger.LogRequest("Weixin", "WX_BAR", path, body);

            var resp = await PostJsonSignedAsync(cfg, path, json);
            var rs = new WxBarOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
            rs.ChannelOriginResponse = resp.ToString();

            var ret = new ChannelRetMsg();
            var tradeState = resp["trade_state"]?.ToString();
            if (tradeState != null)
            {
                rs.TransactionId = resp["transaction_id"]?.ToString();
                rs.TradeType = resp["trade_type"]?.ToString();
                ret.ChannelOrderId = rs.TransactionId;
                ret.ChannelUserId = resp["openid"]?.ToString();
                switch (tradeState)
                {
                    case "SUCCESS": ret.State = ChannelState.CONFIRM_SUCCESS; break;
                    case "USERPAYING": ret.State = ChannelState.WAITING; ret.IsNeedQuery = true; break;
                    case "PAYERROR": ret.State = ChannelState.CONFIRM_FAIL; break;
                    default: ret.State = ChannelState.WAITING; ret.IsNeedQuery = true; break;
                }
            }
            else
            {
                ret = ChannelRetMsg.ConfirmFail(resp["code"]?.ToString(), resp["message"]?.ToString());
            }

            rs.ChannelRetMsg = ret;
            PayLogger.LogResponse("Weixin", "WX_BAR", resp, ret);
            return rs;
        }

        private static async Task<JObject> PostJsonSignedAsync(WxpayNormalMchParams cfg, string path, string json)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, BaseUrl + path)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            req.Headers.Add("Accept", "application/json");
            SignAsV3(req, cfg, "POST", path, json);

            if (WxPayHelper.IsPublicKeyMode(cfg))
                req.Headers.Add("Wechatpay-Serial", cfg.WxpayPublicKeyId);

            var httpResp = await _http.SendAsync(req);
            var body = await httpResp.Content.ReadAsStringAsync();
            return JObject.Parse(body);
        }

        private static void SignAsV3(HttpRequestMessage req, WxpayNormalMchParams cfg, string method, string path, string body)
        {
            var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var nonce = Guid.NewGuid().ToString("N");
            var signStr = $"{method}\n{path}\n{ts}\n{nonce}\n{body}\n";
            var sig = RsaSign(cfg.PrivateKey, signStr);
            req.Headers.Add("Authorization",
                $"WECHATPAY2-SHA256-RSA2048 mchid=\"{cfg.MchId}\"," +
                $"nonce_str=\"{nonce}\",timestamp=\"{ts}\"," +
                $"serial_no=\"{cfg.SerialNo}\",signature=\"{sig}\"");
        }

        private static string RsaSign(string privateKeyPem, string data)
        {
            using (var rsa = System.Security.Cryptography.RSA.Create())
            {
                var b64 = privateKeyPem
                    .Replace("-----BEGIN PRIVATE KEY-----", "").Replace("-----END PRIVATE KEY-----", "")
                    .Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "")
                    .Replace("\r", "").Replace("\n", "").Trim();
                rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(b64), out _);
                var sig = rsa.SignData(Encoding.UTF8.GetBytes(data),
                    System.Security.Cryptography.HashAlgorithmName.SHA256,
                    System.Security.Cryptography.RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(sig);
            }
        }
    }
}
