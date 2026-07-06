using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Mch;using B.Unified.Payment.Alipay.Models.MchParams;
using B.Unified.Payment.Weixin.Models.MchParams;
using B.Unified.Payment.YsfPay.Models.MchParams;
using Newtonsoft.Json.Linq;
using AlipayCertMode = B.Unified.Payment.Alipay.Models.Mch.CertMode;
using WxpayCertMode = B.Unified.Payment.Weixin.Models.Mch.CertMode;
using YsfpayCertMode = B.Unified.Payment.YsfPay.Models.Mch.CertMode;

namespace B.Unified.Payment.Sample.WebApi;

/// <summary>配置加载（示例：从 keys.json 读取，实际项目应替换为数据库、配置中心等）</summary>
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
                    AppId            = ali["AppId"].ToString(),
                    PrivateKey       = ali["PrivateKey"].ToString(),
                    UseCert          = ali["UseCert"] != null ? (AlipayCertMode)ali["UseCert"].Value<int>() : (AlipayCertMode?)null,
                    AlipayPublicKey  = ali["AlipayPublicKey"]?.ToString(),
                    AppPublicCert    = ali["AppPublicCert"]?.ToString(),
                    AlipayPublicCert = ali["AlipayPublicCert"]?.ToString(),
                    AlipayRootCert   = ali["AlipayRootCert"]?.ToString(),
                    Sandbox          = (EnvFlag)ali["Sandbox"].Value<int>(),
                    SignType         = ali["SignType"].ToString()
                };
                break;

            case "wxpay":
                var wx = _keys["Weixin"];
                ctx.NormalMchParamsMap["wxpay"] = new WxpayNormalMchParams
                {
                    AppId            = wx["AppId"].ToString(),
                    MchId            = wx["MchId"].ToString(),
                    ApiV3Key         = wx["ApiV3Key"].ToString(),
                    SerialNo         = wx["SerialNo"].ToString(),
                    PrivateKey       = wx["PrivateKey"].ToString(),
                    UseCert          = wx["UseCert"] != null ? (WxpayCertMode)wx["UseCert"].Value<int>() : (WxpayCertMode?)null,
                    WxpayPublicKeyId = wx["WxpayPublicKeyId"]?.ToString(),
                    WxpayPublicKey   = wx["WxpayPublicKey"]?.ToString()
                };
                break;

            case "ysfpay":
                var ysf = _keys["YsfPay"];
                ctx.NormalMchParamsMap["ysfpay"] = new YsfpayIsvParams
                {
                    Sandbox         = (EnvFlag)ysf["Sandbox"].Value<int>(),
                    SerProvId       = ysf["SerProvId"].ToString(),
                    MerId           = ysf["MerId"].ToString(),
                    UseCert         = ysf["UseCert"] != null ? (YsfpayCertMode)ysf["UseCert"].Value<int>() : (YsfpayCertMode?)null,
                    PrivateCert     = ysf["PrivateCert"]?.ToString(),
                    PrivateCertPwd  = ysf["PrivateCertPwd"]?.ToString(),
                    PrivateKey      = ysf["PrivateKey"]?.ToString(),
                    YsfpayPublicKey = ysf["YsfpayPublicKey"].ToString()
                };
                break;
        }

        return ctx;
    }
}
