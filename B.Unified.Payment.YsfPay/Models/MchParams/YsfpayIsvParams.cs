using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.YsfPay.Models.Mch;

namespace B.Unified.Payment.YsfPay.Models.MchParams
{
    /// <summary>
    /// 云闪付商户配置参数。
    /// <para>云闪付接口要求服务商标识 (serProvId) 和商户编号 (merId)，此处统一管理。</para>
    /// </summary>
    public class YsfpayIsvParams : NormalMchParams
    {
        /// <summary>是否沙箱环境</summary>
        public EnvFlag? Sandbox { get; set; }

        /// <summary>云闪付服务商标识（serProvId）</summary>
        public string SerProvId { get; set; }

        /// <summary>子商户编号（merId）</summary>
        public string MerId { get; set; }

        /// <summary>密钥模式: 0-公钥模式, 1-证书模式（默认按配置字段自动推断）</summary>
        public CertMode? UseCert { get; set; }

        /// <summary>服务商私钥（公钥模式下使用，PKCS8 PEM 或 Base64）</summary>
        public string PrivateKey { get; set; }

        /// <summary>服务商私钥证书内容（证书模式下使用，PKCS12 Base64）</summary>
        public string PrivateCert { get; set; }

        /// <summary>私钥证书密码</summary>
        public string PrivateCertPwd { get; set; }

        /// <summary>云闪付公钥（用于回调验签）</summary>
        public string YsfpayPublicKey { get; set; }

        /// <summary>支付机构号（acqOrgCode）</summary>
        public string AcqOrgCode { get; set; }

        /// <summary>网关地址常量</summary>
        public const string ProdServerUrl = "https://partner.95516.com";
        public const string SandboxServerUrl = "http://ysf.bcbip.cn:10240";

        /// <summary>获取当前环境网关地址</summary>
        public string GetServerUrl() => Sandbox == EnvFlag.Sandbox ? SandboxServerUrl : ProdServerUrl;
    }
}