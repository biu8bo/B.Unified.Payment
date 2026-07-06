using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Alipay.Models;
using B.Unified.Payment.Alipay.Models.MchParams;
using B.Unified.Payment.Weixin.Models;
using B.Unified.Payment.Weixin.Models.MchParams;
using B.Unified.Payment.YsfPay.Models;
using B.Unified.Payment.YsfPay.Models.MchParams;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.Sample.WebApi;

/// <summary>酝置加载（示例：从 keys.json 读坖，实际项目应替杢为数杮库〝酝置中心等）</summary>
public static class ConfigHelper
{
    private static readonly JObject _keys;

    static ConfigHelper()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var path = Path.Combine(root, "keys.json");
        _keys = JObject.Parse(File.ReadAllText(path));
    }

    public static MchAppConfigContext Load(string ifCode)
    {
        var ctx = new MchAppConfigContext();

        switch (ifCode)
        {
            case "alipay":
                var ali = _keys["Alipay"];
                ctx.NormalMchParamsMap["alipay"] = new AlipayNormalMchParams
                {
                    AppId           = ali["AppId"].ToString(),
                    PrivateKey      = ali["PrivateKey"].ToString(),
                    AlipayPublicKey = ali["AlipayPublicKey"].ToString(),
                    Sandbox         = (EnvFlag)ali["Sandbox"].Value<int>(),
                    SignType        = ali["SignType"].ToString()
                };
                break;

            case "wxpay":
                var wx = _keys["Weixin"];
                ctx.NormalMchParamsMap["wxpay"] = new WxpayNormalMchParams
                {
                    AppId       = wx["AppId"].ToString(),
                    MchId       = wx["MchId"].ToString(),
                    ApiV3Key    = wx["ApiV3Key"].ToString(),
                    SerialNo    = wx["SerialNo"].ToString(),
                    PrivateKey  = wx["PrivateKey"].ToString()
                };
                break;

            case "ysfpay":
                var ysf = _keys["YsfPay"];
                ctx.NormalMchParamsMap["ysfpay"] = new YsfpayIsvParams
                {
                    Sandbox         = (EnvFlag)ysf["Sandbox"].Value<int>(),
                    SerProvId       = ysf["SerProvId"].ToString(),
                    MerId           = ysf["MerId"].ToString(),
                    PrivateCert     = ysf["PrivateCert"].ToString(),
                    PrivateCertPwd  = ysf["PrivateCertPwd"].ToString(),
                    YsfpayPublicKey = ysf["YsfpayPublicKey"].ToString()
                };
                break;
        }

        return ctx;
    }
}