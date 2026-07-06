using System;
using System.Net.Http;
using System.Text;
using B.Unified.Payment.Abstract;
using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>
    /// 微信付款码支付 — POST /v3/pay/transactions/micropay
    /// <para>Senparc SDK 未提供 MicroPayAsync，此处保留原始 HTTP + V3 签名实现。</para>
    /// </summary>
    public class WxBar : IWxPayWay
    {
        private const string BaseUrl = "https://api.mch.weixin.qq.com";
        private static readonly HttpClient _http = new HttpClient();

        public string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.AuthCode)) return "条码支付 AuthCode 不能为空";
            return null;
        }

        public async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var cfg = WxPayHelper.GetConfig(ctx);
            var path = "/v3/pay/transactions/micropay";
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

            var resp = await PostJsonSigned(cfg, path, json);
            var rs = new Models.WxBarOrderRS { PayOrderId = rq.PayOrderId, MchOrderNo = rq.MchOrderNo };
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
                    case "SUCCESS": ret.State = ChannelRetMsg.ChannelState.CONFIRM_SUCCESS; break;
                    case "USERPAYING": ret.State = ChannelRetMsg.ChannelState.WAITING; ret.IsNeedQuery = true; break;
                    case "PAYERROR": ret.State = ChannelRetMsg.ChannelState.CONFIRM_FAIL; break;
                    default: ret.State = ChannelRetMsg.ChannelState.WAITING; ret.IsNeedQuery = true; break;
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

        /// <summary>发起 V3 签名的 POST 请求并解析返回 JSON</summary>
        private static async Task<JObject> PostJsonSigned(Models.WxpayNormalMchParams cfg, string path, string json)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, BaseUrl + path)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            req.Headers.Add("Accept", "application/json");
            SignAsV3(req, cfg, "POST", path, json);

            var httpResp = await _http.SendAsync(req);
            var body = await httpResp.Content.ReadAsStringAsync();
            return JObject.Parse(body);
        }

        /// <summary>V3 签名并添加到请求头</summary>
        private static void SignAsV3(HttpRequestMessage req, Models.WxpayNormalMchParams cfg, string method, string path, string body)
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
