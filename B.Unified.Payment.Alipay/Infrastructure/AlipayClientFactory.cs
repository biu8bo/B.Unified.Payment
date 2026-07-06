using Aop.Api;
using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Alipay.Models.Mch;
using B.Unified.Payment.Alipay.Models.MchParams;

namespace B.Unified.Payment.Alipay.Infrastructure
{
    /// <summary>
    /// 支付宝 Client 构建工厂 — 所有 PayWay 共用。
    /// <para>支持公钥模式（AlipayPublicKey + Execute）与证书模式（三证书 + CertificateExecute）。</para>
    /// </summary>
    internal static class AlipayClientFactory
    {
        internal sealed class ClientHolder
        {
            public IAopClient Client { get; }
            public CertMode Mode { get; }

            internal ClientHolder(IAopClient client, CertMode mode)
            {
                Client = client;
                Mode = mode;
            }

            public T Execute<T>(IAopRequest<T> request) where T : AopResponse, new()
                => Mode == CertMode.Certificate
                    ? Client.CertificateExecute(request)
                    : Client.Execute(request);
        }

        public static ClientHolder Build(MchAppConfigContext ctx)
        {
            var p = ctx.GetNormalMchParams<AlipayNormalMchParams>(Constants.IfCode.ALIPAY)
                    ?? throw new BizException("未找到支付宝商户参数");

            var mode = ResolveMode(p);
            var client = mode == CertMode.Certificate
                ? BuildCertClient(p)
                : BuildPublicKeyClient(p);

            return new ClientHolder(client, mode);
        }

        private static CertMode ResolveMode(AlipayNormalMchParams p)
            => p.UseCert ?? CertMode.PublicKey;

        private static IAopClient BuildPublicKeyClient(AlipayNormalMchParams p)
        {
            if (string.IsNullOrEmpty(p.AlipayPublicKey))
                throw new BizException("公钥模式下 AlipayPublicKey 不能为空");

            return new DefaultAopClient(
                p.GetServerUrl(),
                p.AppId,
                p.PrivateKey,
                AlipayNormalMchParams.Format,
                "1.0",
                p.SignType ?? "RSA2",
                p.AlipayPublicKey,
                AlipayNormalMchParams.Charset
            );
        }

        private static IAopClient BuildCertClient(AlipayNormalMchParams p)
        {
            if (string.IsNullOrEmpty(p.AppPublicCert)
                || string.IsNullOrEmpty(p.AlipayPublicCert)
                || string.IsNullOrEmpty(p.AlipayRootCert))
            {
                throw new BizException("证书模式下 AppPublicCert、AlipayPublicCert、AlipayRootCert 不能为空");
            }

            var certParams = new CertParams
            {
                AppCertContent = p.AppPublicCert,
                AlipayPublicCertContent = p.AlipayPublicCert,
                RootCertContent = p.AlipayRootCert
            };

            return new DefaultAopClient(
                p.GetServerUrl(),
                p.AppId,
                p.PrivateKey,
                AlipayNormalMchParams.Format,
                "1.0",
                p.SignType ?? "RSA2",
                AlipayNormalMchParams.Charset,
                false,
                certParams
            );
        }
    }
}
