using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Models;

namespace B.Unified.Payment.WeixinSample;

/// <summary>微信支付共享配置</summary>
public static class WeixinConfig
{
    public static readonly MchAppConfigContext Context = Create();

    private static MchAppConfigContext Create()
    {
        var ctx = new MchAppConfigContext();
        ctx.NormalMchParamsMap[IfCode.WXPAY] = new WxpayNormalMchParams
        {
            AppId       = "wx2421b1c4370ec43b",
            MchId       = "10000100",
            ApiV3Key    = "your32charapiv3keyhere123456",
            SerialNo    = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            PrivateKey  = @"-----BEGIN PRIVATE KEY-----
MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQ...
-----END PRIVATE KEY-----",
        };
        return ctx;
    }
}