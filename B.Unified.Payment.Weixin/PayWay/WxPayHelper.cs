using System;
using System.Net.Http;
using System.Text;
using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>
    /// 微信支付 PayWay 共享工具 — 微信支付 API V3 原生 HTTP 调用（兼容 Senparc.Weixin.TenPayV3 依赖）
    /// </summary>
    internal static class WxPayHelper
    {
        private static readonly HttpClient _http = new HttpClient();
        private const string BaseUrl = "https://api.mch.weixin.qq.com";

        /// <summary>获取商户配置</summary>
        public static WxpayNormalMchParams GetConfig(MchAppConfigContext ctx)
            => ctx.GetNormalMchParams<WxpayNormalMchParams>(IfCode.WXPAY)
               ?? throw new BizException("未找到微信支付商户配置");

        /// <summary>HTTP POST JSON 调用微信支付 V3 API</summary>
        public static JObject PostJson(WxpayNormalMchParams cfg, string path, object body)
        {
            var json = JsonConvert.SerializeObject(body,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var req = new HttpRequestMessage(HttpMethod.Post, BaseUrl + path)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            req.Headers.Add("Authorization", SignAuth(cfg, "POST", path, json));
            req.Headers.Add("Accept", "application/json");

            try
            {
                var resp = _http.SendAsync(req).GetAwaiter().GetResult();
                var respBody = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JObject.Parse(respBody);
            }
            catch { return new JObject(); }
        }

        private static string SignAuth(WxpayNormalMchParams cfg, string method, string path, string body)
        {
            var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var nonce = Guid.NewGuid().ToString("N");
            var signStr = $"{method}\n{path}\n{ts}\n{nonce}\n{body}\n";
            var sig = RsaSign(cfg.PrivateKey, signStr);
            return $"WECHATPAY2-SHA256-RSA2048 mchid=\"{cfg.MchId}\"," +
                   $"nonce_str=\"{nonce}\",timestamp=\"{ts}\"," +
                   $"serial_no=\"{cfg.SerialNo}\",signature=\"{sig}\"";
        }

        /// <summary>RSA-SHA256 签名</summary>
        public static string RsaSign(string privateKeyPem, string data)
        {
            using (var rsa = System.Security.Cryptography.RSA.Create())
            {
                rsa.ImportPkcs8PrivateKey(ParsePem(privateKeyPem), out _);
                var sig = rsa.SignData(Encoding.UTF8.GetBytes(data),
                    System.Security.Cryptography.HashAlgorithmName.SHA256,
                    System.Security.Cryptography.RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(sig);
            }
        }

        private static byte[] ParsePem(string pem)
        {
            var b64 = pem.Replace("-----BEGIN PRIVATE KEY-----", "").Replace("-----END PRIVATE KEY-----", "")
                         .Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "")
                         .Replace("\r", "").Replace("\n", "").Trim();
            return Convert.FromBase64String(b64);
        }

        /// <summary>生成 JSAPI/小程序调起支付签名 JSON</summary>
        public static string GenJsApiSign(WxpayNormalMchParams cfg, string prepayId)
        {
            var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var nonce = Guid.NewGuid().ToString("N")[..32];
            var pkg = $"prepay_id={prepayId}";
            var sign = RsaSign(cfg.PrivateKey, $"{cfg.AppId}\n{ts}\n{nonce}\n{pkg}\n");
            return JsonConvert.SerializeObject(new { appId = cfg.AppId, timeStamp = ts, nonceStr = nonce, package = pkg, signType = "RSA", paySign = sign });
        }

        /// <summary>生成 APP 调起支付签名 JSON</summary>
        public static string GenAppPaySign(WxpayNormalMchParams cfg, string prepayId)
        {
            var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var nonce = Guid.NewGuid().ToString("N")[..32];
            var sign = RsaSign(cfg.PrivateKey, $"{cfg.AppId}\n{ts}\n{nonce}\nprepay_id={prepayId}\n");
            return JsonConvert.SerializeObject(new { appid = cfg.AppId, partnerid = cfg.MchId, prepayid = prepayId, package = "Sign=WXPay", noncestr = nonce, timestamp = ts, sign });
        }

        /// <summary>JObject → JSON 字符串</summary>
        public static string ToJsonString(JObject obj) => obj?.ToString(Formatting.Indented) ?? "";
    }
}