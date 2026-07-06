using System;
using System.Net.Http;
using System.Text;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Models;

namespace B.Unified.Payment.Weixin
{
    /// <summary>
    /// 微信支付订单查询 — 调用 V3 API GET /v3/pay/transactions/out-trade-no/{out_trade_no}?mchid=xxx。
    /// <para>状态映射: SUCCESS → CONFIRM_SUCCESS, USERPAYING → WAITING, CLOSED/REVOKED/PAYERROR → CONFIRM_FAIL, 其他 → UNKNOWN</para>
    /// </summary>
    public class WeixinPayOrderQueryService : IPayOrderQueryService
    {
        public string GetIfCode() => IfCode.WXPAY;

        public ChannelRetMsg Query(string payOrderId, MchAppConfigContext ctx)
        {
            var cfg = ctx.GetNormalMchParams<WxpayNormalMchParams>(IfCode.WXPAY)
                       ?? throw new BizException("未找到微信支付商户配置");

            var path = $"/v3/pay/transactions/out-trade-no/{payOrderId}";
            var query = $"?mchid={cfg.MchId}";
            var url = $"https://api.mch.weixin.qq.com{path}{query}";

            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, url);
                // V3 签名
                var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                var nonce = Guid.NewGuid().ToString("N");
                var signStr = $"GET\n{path}{query}\n{ts}\n{nonce}\n\n";
                var sig = RsaSign(cfg.PrivateKey, signStr);
                req.Headers.Add("Authorization",
                    $"WECHATPAY2-SHA256-RSA2048 mchid=\"{cfg.MchId}\"," +
                    $"nonce_str=\"{nonce}\",timestamp=\"{ts}\"," +
                    $"serial_no=\"{cfg.SerialNo}\",signature=\"{sig}\"");
                req.Headers.Add("Accept", "application/json");

                var resp = new HttpClient().SendAsync(req).GetAwaiter().GetResult();
                var body = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var json = Newtonsoft.Json.Linq.JObject.Parse(body);
                var tradeState = json["trade_state"]?.ToString();

                if (tradeState == "SUCCESS")
                    return ChannelRetMsg.ConfirmSuccess(json["transaction_id"]?.ToString());
                if (tradeState == "USERPAYING")
                    return ChannelRetMsg.Waiting();
                if (tradeState == "CLOSED" || tradeState == "REVOKED" || tradeState == "PAYERROR")
                    return ChannelRetMsg.ConfirmFail();

                return ChannelRetMsg.Unknown();
            }
            catch (Exception ex)
            {
                return ChannelRetMsg.SysError(ex.Message);
            }
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