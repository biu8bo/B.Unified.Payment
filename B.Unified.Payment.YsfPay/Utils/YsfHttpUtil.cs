using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.YsfPay.Models.Mch;
using B.Unified.Payment.YsfPay.Models.MchParams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.YsfPay.Utils
{
    /// <summary>
    /// 云闪付公共工具 — HTTP 请求 + RSA SHA256withRSA 签名。
    /// <para>支持证书模式（PKCS12）与公钥模式（RSA 私钥）。</para>
    /// </summary>
    internal static class YsfHttpUtil
    {
        private static readonly HttpClient _http = new HttpClient();

        /// <summary>
        /// 封装公共参数 + 签名 + 发送 HTTP POST，返回 JSONObject
        /// </summary>
        public static JObject PackageParamAndReq(string apiUri, JObject reqParams, YsfpayIsvParams cfg)
        {
            reqParams["serProvId"] = cfg.SerProvId;
            reqParams["merId"] = cfg.MerId;

            var signature = Sign(reqParams, cfg);
            reqParams["signature"] = signature;

            var url = cfg.GetServerUrl() + apiUri;
            var json = reqParams.ToString(Formatting.None);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = _http.PostAsync(url, content).GetAwaiter().GetResult();
            var respBody = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JObject.Parse(respBody);
        }

        private static CertMode ResolveMode(YsfpayIsvParams cfg)
        {
            if (cfg.UseCert.HasValue)
                return cfg.UseCert.Value;

            if (!string.IsNullOrEmpty(cfg.PrivateCert))
                return CertMode.Certificate;

            if (!string.IsNullOrEmpty(cfg.PrivateKey))
                return CertMode.PublicKey;

            return CertMode.Certificate;
        }

        private static string Sign(JObject reqParams, YsfpayIsvParams cfg)
        {
            var signStr = BuildSignString(reqParams);
            var sha256Data = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(signStr));

            return ResolveMode(cfg) == CertMode.PublicKey
                ? SignWithPrivateKey(cfg.PrivateKey, sha256Data)
                : SignWithCertificate(cfg.PrivateCert, cfg.PrivateCertPwd, sha256Data);
        }

        private static string BuildSignString(JObject reqParams)
        {
            var sorted = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var prop in reqParams.Properties())
            {
                var val = prop.Value?.ToString();
                if (val != null) sorted[prop.Name] = val;
            }

            return string.Join("&", sorted.Select(kv => $"{kv.Key}={kv.Value}"));
        }

        private static string SignWithCertificate(string certContent, string certPwd, byte[] sha256Data)
        {
            if (string.IsNullOrEmpty(certContent))
                throw new BizException("证书模式下 PrivateCert 不能为空");

            var cert = string.IsNullOrEmpty(certPwd)
                ? new X509Certificate2(Convert.FromBase64String(certContent))
                : new X509Certificate2(Convert.FromBase64String(certContent), certPwd,
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

            using (var rsa = cert.GetRSAPrivateKey())
            {
                var sig = rsa.SignData(sha256Data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(sig);
            }
        }

        private static string SignWithPrivateKey(string privateKey, byte[] sha256Data)
        {
            if (string.IsNullOrEmpty(privateKey))
                throw new BizException("公钥模式下 PrivateKey 不能为空");

            using (var rsa = RSA.Create())
            {
                rsa.ImportPkcs8PrivateKey(ExtractPrivateKeyBytes(privateKey), out _);
                var sig = rsa.SignData(sha256Data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(sig);
            }
        }

        private static byte[] ExtractPrivateKeyBytes(string privateKey)
        {
            if (privateKey.Contains("BEGIN"))
            {
                var b64 = privateKey
                    .Replace("-----BEGIN PRIVATE KEY-----", "").Replace("-----END PRIVATE KEY-----", "")
                    .Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "")
                    .Replace("\r", "").Replace("\n", "").Trim();
                return Convert.FromBase64String(b64);
            }

            return Convert.FromBase64String(privateKey.Trim());
        }

        /// <summary>获取 orderType（用于查单/退款/关单）</summary>
        public static string GetOrderType(string wayCode) => wayCode switch
        {
            "YSF_BAR" or "YSF_JSAPI" => "unionpay",
            "ALI_BAR" or "ALI_JSAPI" => "alipay",
            "WX_BAR" or "WX_JSAPI" => "wechat",
            _ => "unionpay"
        };

        /// <summary>获取支付 orderType（条码 vs JSAPI 不同）</summary>
        public static string GetPayOrderType(string wayCode) => wayCode switch
        {
            "YSF_BAR" => "unionpay",
            "YSF_JSAPI" => "upJs",
            "ALI_BAR" => "alipay",
            "ALI_JSAPI" => "alipayJs",
            "WX_BAR" => "wechat",
            "WX_JSAPI" => "wechatJs",
            _ => "unionpay"
        };
    }
}
