using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using B.Unified.Payment.YsfPay.Models;

namespace B.Unified.Payment.YsfPay
{
    /// <summary>
    /// 云闪付公共工具 — HTTP 请求 + RSA SHA256withRSA 签名
    /// </summary>
    internal static class YsfHttpUtil
    {
        private static readonly HttpClient _http = new HttpClient();

        /// <summary>
        /// 封装公共参数 + 签名 + 发送 HTTP POST，返回 JSONObject
        /// </summary>
        public static JObject PackageParamAndReq(string apiUri, JObject reqParams, YsfpayIsvParams cfg)
        {
            // 注入服务商/子商户信息
            reqParams["serProvId"] = cfg.SerProvId;
            reqParams["merId"] = cfg.MerId;

            // RSA SHA256withRSA 签名
            var signature = Sign(reqParams, cfg.PrivateCert, cfg.PrivateCertPwd);
            reqParams["signature"] = signature;

            // HTTP POST
            var url = cfg.GetServerUrl() + apiUri;
            var json = reqParams.ToString(Formatting.None);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = _http.PostAsync(url, content).GetAwaiter().GetResult();
            var respBody = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JObject.Parse(respBody);
        }

        /// <summary>
        /// RSA SHA256withRSA 签名 — 参数按 key 排序 → 拼成 key=value&... → SHA256 摘要 → RSA 签名 → Base64
        /// </summary>
        private static string Sign(JObject reqParams, string certContent, string certPwd)
        {
            // 1. 排序并拼接签名字符串
            var sorted = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var prop in reqParams.Properties())
            {
                var val = prop.Value?.ToString();
                if (val != null) sorted[prop.Name] = val;
            }
            var signStr = string.Join("&", sorted.Select(kv => $"{kv.Key}={kv.Value}"));

            // 2. SHA256 摘要
            var sha256Data = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(signStr));

            // 3. 加载 PKCS12 证书
            var cert = string.IsNullOrEmpty(certPwd)
                ? new X509Certificate2(Convert.FromBase64String(certContent))
                : new X509Certificate2(Convert.FromBase64String(certContent), certPwd,
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

            // 4. RSA 签名
            using (var rsa = cert.GetRSAPrivateKey())
            {
                var sig = rsa.SignData(sha256Data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(sig);
            }
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