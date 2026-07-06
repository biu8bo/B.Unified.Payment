using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.YsfPay.Constants;
using B.Unified.Payment.YsfPay.Models;

namespace B.Unified.Payment.YsfPaySample;

/// <summary>云闪付共享配置</summary>
public static class YsfpayConfig
{
    public static readonly MchAppConfigContext Context = Create();

    private static MchAppConfigContext Create()
    {
        var ctx = new MchAppConfigContext();
        ctx.NormalMchParamsMap[IfCode.YSFPAY] = new YsfpayIsvParams
        {
            Sandbox         = 1,
            SerProvId       = "your_ser_prov_id",     // ⚠ 替换
            MerId           = "your_mer_id",           // ⚠ 替换
            PrivateCert     = "base64_pkcs12_cert",    // ⚠ 替换为 PKCS12 证书 Base64
            PrivateCertPwd  = "cert_password",         // ⚠ 替换
            YsfpayPublicKey = "base64_public_key",     // ⚠ 替换
        };
        return ctx;
    }
}