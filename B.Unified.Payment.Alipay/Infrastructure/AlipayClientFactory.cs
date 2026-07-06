using Aop.Api;
using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Alipay.Models.MchParams;

namespace B.Unified.Payment.Alipay.Infrastructure
{
    /// <summary>
    /// 支付宝 Client 构建工厂 — 所有 PayWay 共用
    /// </summary>
    internal static class AlipayClientFactory
    {
        /// <summary>
        /// 从商户配置上下文中构建 IAopClient。
        /// <para>使用 8 参数构造函数: (serverUrl, appId, privateKey, format, version, signType, alipayPublicKey, charset)</para>
        /// </summary>
        public static IAopClient Build(MchAppConfigContext ctx)
        {
            var p = ctx.GetNormalMchParams<AlipayNormalMchParams>(Constants.IfCode.ALIPAY)
                    ?? throw new BizException("未找到支付宝商户参数");
            return new DefaultAopClient(
                p.GetServerUrl(),           // 1. 服务器地址
                p.AppId,                    // 2. 应用 ID
                p.PrivateKey,               // 3. 应用私钥
                "json",                     // 4. 格式
                "1.0",                      // 5. 版本号
                p.SignType ?? "RSA2",       // 6. 签名方式
                p.AlipayPublicKey,          // 7. 支付宝公钥
                "utf-8"                     // 8. 字符集
            );
        }
    }
}