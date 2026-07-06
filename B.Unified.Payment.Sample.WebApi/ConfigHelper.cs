using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Alipay.Models;
using B.Unified.Payment.Weixin.Models;
using B.Unified.Payment.YsfPay.Models;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.Sample.WebApi;

/// <summary>配置加载（示例：从 keys.json 读取，实际项目应替换为数据库、配置中心等）</summary>
public static class ConfigHelper
{
    private static readonly JObject _keys;

    static ConfigHelper()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
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
                    Sandbox         = (byte)ali["Sandbox"].Value<int>(),
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
                    Sandbox         = (byte)ysf["Sandbox"].Value<int>(),
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