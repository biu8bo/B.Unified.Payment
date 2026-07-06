using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Alipay.Constants;
using B.Unified.Payment.Alipay.Models.Mch;
using B.Unified.Payment.Alipay.Models.MchParams;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.Sample.Alipay.Config;

/// <summary>支付宝共享配置 — 从解决方案根目录 keys.json 读取秘钥</summary>
public static class AlipayConfig
{
    public static readonly MchAppConfigContext Context = Create();

    private static JObject LoadKeys()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var path = Path.Combine(root, "keys.json");
        return JObject.Parse(File.ReadAllText(path));
    }

    private static MchAppConfigContext Create()
    {
        var keys = LoadKeys()["Alipay"];
        var ctx = new MchAppConfigContext();
        ctx.NormalMchParamsMap[IfCode.ALIPAY] = new AlipayNormalMchParams
        {
            AppId            = keys["AppId"].ToString(),
            PrivateKey       = keys["PrivateKey"].ToString(),
            UseCert          = keys["UseCert"] != null ? (CertMode)keys["UseCert"].Value<int>() : (CertMode?)null,
            AlipayPublicKey  = keys["AlipayPublicKey"]?.ToString(),
            AppPublicCert    = keys["AppPublicCert"]?.ToString(),
            AlipayPublicCert = keys["AlipayPublicCert"]?.ToString(),
            AlipayRootCert   = keys["AlipayRootCert"]?.ToString(),
            Sandbox          = (EnvFlag)keys["Sandbox"].Value<int>(),
            SignType         = keys["SignType"].ToString()
        };
        return ctx;
    }
}
