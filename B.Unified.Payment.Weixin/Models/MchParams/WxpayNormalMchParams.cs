using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Weixin.Models.Mch;

namespace B.Unified.Payment.Weixin.Models.MchParams
{
    /// <summary>
    /// 微信支付商户配置参数（普通商户）
    /// </summary>
    public class WxpayNormalMchParams : NormalMchParams
    {
        /// <summary>微信公众号 / 小程序 AppId</summary>
        public string AppId { get; set; }

        /// <summary>公众号 AppSecret（获取 openid 等场景使用）</summary>
        public string AppSecret { get; set; }

        /// <summary>微信支付商户号</summary>
        public string MchId { get; set; }

        /// <summary>API V2 密钥（MD5 签名，V2 接口需要）</summary>
        public string Key { get; set; }

        /// <summary>密钥模式: 0-公钥模式, 1-证书模式（默认按配置字段自动推断）</summary>
        public CertMode? UseCert { get; set; }

        /// <summary>API 版本: V2 / V3</summary>
        public string ApiVersion { get; set; }

        /// <summary>API V3 密钥（32 位，用于回调通知解密和平台证书下载）</summary>
        public string ApiV3Key { get; set; }

        /// <summary>商户 API 证书序列号</summary>
        public string SerialNo { get; set; }

        /// <summary>商户 API 证书私钥（PKCS8 PEM 格式内容，用于请求签名）</summary>
        public string PrivateKey { get; set; }

        /// <summary>商户 API 证书（PEM 格式内容）</summary>
        public string Certificate { get; set; }

        /// <summary>微信支付公钥 ID（验签使用）</summary>
        public string WxpayPublicKeyId { get; set; }

        /// <summary>微信支付公钥证书（验签使用）</summary>
        public string WxpayPublicKey { get; set; }
    }
}