using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Models;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.Sample.Weixin;

/// <summary>微信支付共享配置 — 从解决方案根目录 keys.json 读取秘钥</summary>
public static class WeixinConfig
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
        var keys = LoadKeys()["Weixin"];
        var ctx = new MchAppConfigContext();
        ctx.NormalMchParamsMap[IfCode.WXPAY] = new WxpayNormalMchParams
        {
            AppId       = keys["AppId"].ToString(),
            MchId       = keys["MchId"].ToString(),
            ApiV3Key    = keys["ApiV3Key"].ToString(),
            SerialNo    = keys["SerialNo"].ToString(),
            PrivateKey  = keys["PrivateKey"].ToString()
        };
        return ctx;
    }
}