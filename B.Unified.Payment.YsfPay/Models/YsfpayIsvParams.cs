using B.Unified.Payment.Abstract.Models;

namespace B.Unified.Payment.YsfPay.Models
{
    /// <summary>
    /// 云闪付商户配置参数。
    /// <para>云闪付接口要求服务商标识 (serProvId) 和商户编号 (merId)，此处统一管理。</para>
    /// </summary>
    public class YsfpayIsvParams : NormalMchParams
    {
        /// <summary>是否沙箱环境: 0-生产, 1-沙箱</summary>
        public byte? Sandbox { get; set; }

        /// <summary>云闪付服务商标识（serProvId）</summary>
        public string SerProvId { get; set; }

        /// <summary>子商户编号（merId）</summary>
        public string MerId { get; set; }

        /// <summary>服务商私钥证书内容（PKCS12 Base64 或文件路径）</summary>
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
        public string GetServerUrl() => Sandbox == 1 ? SandboxServerUrl : ProdServerUrl;
    }
}