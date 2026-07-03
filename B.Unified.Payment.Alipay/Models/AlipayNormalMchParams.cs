using B.Unified.Payment.Abstract.Models;

namespace B.Unified.Payment.Alipay.Models
{
    /// <summary>
    /// 支付宝商户配置参数（普通商户）
    /// </summary>
    public class AlipayNormalMchParams : NormalMchParams
    {
        /// <summary>是否沙箱环境: 0-生产, 1-沙箱</summary>
        public byte? Sandbox { get; set; }

        /// <summary>支付宝分配给开发者的应用 ID</summary>
        public string AppId { get; set; }

        /// <summary>商户应用私钥（PKCS8 格式）</summary>
        public string PrivateKey { get; set; }

        /// <summary>支付宝公钥</summary>
        public string AlipayPublicKey { get; set; }

        /// <summary>签名算法类型（推荐 RSA2）</summary>
        public string SignType { get; set; }

        /// <summary>是否使用证书模式: 0-公钥模式, 1-证书模式</summary>
        public byte? UseCert { get; set; }

        /// <summary>应用公钥证书内容（证书模式下必填）</summary>
        public string AppPublicCert { get; set; }

        /// <summary>支付宝公钥证书内容（证书模式下必填）</summary>
        public string AlipayPublicCert { get; set; }

        /// <summary>支付宝根证书内容（证书模式下必填）</summary>
        public string AlipayRootCert { get; set; }

        // 网关常量
        public const string Format = "json";
        public const string Charset = "utf-8";

        /// <summary>支付宝生产环境网关</summary>
        public static readonly string ProdServerUrl = "https://openapi.alipay.com/gateway.do";

        /// <summary>支付宝沙箱环境网关</summary>
        public static readonly string SandboxServerUrl = "https://openapi-sandbox.dl.alipaydev.com/gateway.do";

        /// <summary>根据 Sandbox 标记返回对应网关地址</summary>
        public string GetServerUrl() => Sandbox == 1 ? SandboxServerUrl : ProdServerUrl;
    }
}