using Aop.Api;
using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Alipay.Models;

namespace B.Unified.Payment.Alipay.PayWay
{
    /// <summary>
    /// 支付宝 Client 构建工厂 — 所有 PayWay 共用
    /// </summary>
    internal static class AlipayClientFactory
    {
        /// <summary>
        /// 从商户配置上下文中构建 IAopClient
        /// </summary>
        public static IAopClient Build(MchAppConfigContext ctx)
        {
            var p = ctx.GetNormalMchParams<AlipayNormalMchParams>(Constants.IfCode.ALIPAY)
                    ?? throw new BizException("未找到支付宝商户参数");
            return new DefaultAopClient(p.GetServerUrl(), p.AppId, p.PrivateKey,
                AlipayNormalMchParams.Format, AlipayNormalMchParams.Charset,
                p.SignType ?? "RSA2" , p.AlipayPublicKey);
        }
    }
}