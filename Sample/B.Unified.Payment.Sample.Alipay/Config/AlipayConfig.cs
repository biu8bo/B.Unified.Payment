using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Alipay.Constants;
using B.Unified.Payment.Alipay.Models.MchParams;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.Sample.Alipay.Config;

/// <summary>??????? ? ???????? keys.json ????</summary>
public static class AlipayConfig
{
    public static readonly MchAppConfigContext Context = Create();

    private static JObject LoadKeys()
    {
        // ??????? (B.Unified.Payment)
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
            AppId           = keys["AppId"].ToString(),
            PrivateKey      = keys["PrivateKey"].ToString(),
            AlipayPublicKey = keys["AlipayPublicKey"].ToString(),
            Sandbox         = (EnvFlag)keys["Sandbox"].Value<int>(),
            SignType        = keys["SignType"].ToString()
        };
        return ctx;
    }
}