using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.YsfPay.Constants;
using B.Unified.Payment.YsfPay.Models;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.YsfPaySample;

/// <summary>云闪付共享配置 — 从解决方案根目录 keys.json 读取秘钥</summary>
public static class YsfpayConfig
{
    public static readonly MchAppConfigContext Context = Create();

    private static JObject LoadKeys()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var path = Path.Combine(root, "keys.json");
        return JObject.Parse(File.ReadAllText(path));
    }

    private static MchAppConfigContext Create()
    {
        var keys = LoadKeys()["YsfPay"];
        var ctx = new MchAppConfigContext();
        ctx.NormalMchParamsMap[IfCode.YSFPAY] = new YsfpayIsvParams
        {
            Sandbox         = (EnvFlag)keys["Sandbox"].Value<int>(),
            SerProvId       = keys["SerProvId"].ToString(),
            MerId           = keys["MerId"].ToString(),
            PrivateCert     = keys["PrivateCert"].ToString(),
            PrivateCertPwd  = keys["PrivateCertPwd"].ToString(),
            YsfpayPublicKey = keys["YsfpayPublicKey"].ToString()
        };
        return ctx;
    }
}