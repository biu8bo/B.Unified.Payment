using System;
using System.Net.Http;
using System.Text;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Refund;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.Weixin
{
    /// <summary>
    /// 微信支付退款服务 — V3 API。
    /// <para>发起: POST /v3/refund/domestic/refunds, 查单: GET /v3/refund/domestic/refunds/{out_refund_no}</para>
    /// </summary>
    public class WeixinRefundService : IRefundService
    {
        private static readonly HttpClient _http = new HttpClient();
        private const string BaseUrl = "https://api.mch.weixin.qq.com";

        public string GetIfCode() => IfCode.WXPAY;

        public string PreCheck(RefundOrderRQ bizRQ, MchAppConfigContext ctx)
        {
            if (bizRQ == null) return "退款请求参数不能为空";
            if (string.IsNullOrEmpty(bizRQ.RefundOrderId)) return "退款单号不能为空";
            if (bizRQ.RefundAmount == null || bizRQ.RefundAmount <= 0) return "退款金额无效";
            if (string.IsNullOrEmpty(bizRQ.PayOrderId)) return "原支付订单号不能为空";
            if (bizRQ.PayOrderAmount == null || bizRQ.PayOrderAmount <= 0) return "原支付金额无效";
            return null;
        }

        public ChannelRetMsg Refund(RefundOrderRQ bizRQ, MchAppConfigContext ctx)
        {
            try
            {
                var cfg = GetConfig(ctx);
                var path = "/v3/refund/domestic/refunds";
                var body = new
                {
                    out_trade_no  = bizRQ.PayOrderId,
                    out_refund_no = bizRQ.RefundOrderId,
                    notify_url    = bizRQ.NotifyUrl,
                    reason        = bizRQ.RefundReason,
                    amount        = new { refund = bizRQ.RefundAmount, total = bizRQ.PayOrderAmount, currency = "CNY" }
                };
                var json = JsonConvert.SerializeObject(body,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                var req = new HttpRequestMessage(HttpMethod.Post, BaseUrl + path)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                req.Headers.Add("Accept", "application/json");
                SignAsV3(req, cfg, "POST", path, json);

                var httpResp = _http.SendAsync(req).GetAwaiter().GetResult();
                var respJson = JObject.Parse(httpResp.Content.ReadAsStringAsync().GetAwaiter().GetResult());

                var status = respJson["status"]?.ToString();
                var refundId = respJson["refund_id"]?.ToString();

                if (status == "SUCCESS")
                    return ChannelRetMsg.ConfirmSuccess(refundId);
                if (status == "PROCESSING")
                    return ChannelRetMsg.Waiting(refundId);

                return ChannelRetMsg.ConfirmFail(status, status);
            }
            catch (Exception ex)
            {
                return ChannelRetMsg.SysError(ex.Message);
            }
        }

        public ChannelRetMsg Query(string refundOrderId, string payOrderId, string channelOrderNo, MchAppConfigContext ctx)
        {
            try
            {
                var cfg = GetConfig(ctx);
                var path = $"/v3/refund/domestic/refunds/{refundOrderId}";
                var query = $"?mchid={cfg.MchId}";
                var fullUrl = BaseUrl + path + query;

                var req = new HttpRequestMessage(HttpMethod.Get, fullUrl);
                req.Headers.Add("Accept", "application/json");
                SignAsV3(req, cfg, "GET", path + query, "");

                var httpResp = _http.SendAsync(req).GetAwaiter().GetResult();
                var respJson = JObject.Parse(httpResp.Content.ReadAsStringAsync().GetAwaiter().GetResult());

                var status = respJson["status"]?.ToString();

                if (status == "SUCCESS")
                    return ChannelRetMsg.ConfirmSuccess();
                if (status == "CLOSED" || status == "ABNORMAL")
                    return ChannelRetMsg.ConfirmFail(status, status);

                return ChannelRetMsg.Waiting();
            }
            catch (Exception ex)
            {
                return ChannelRetMsg.SysError(ex.Message);
            }
        }

        private static WxpayNormalMchParams GetConfig(MchAppConfigContext ctx)
            => ctx.GetNormalMchParams<WxpayNormalMchParams>(IfCode.WXPAY)
               ?? throw new BizException("未找到微信支付商户配置");

        /// <summary>V3 签名并添加到请求头</summary>
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