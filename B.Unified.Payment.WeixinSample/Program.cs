using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Models;
using Newtonsoft.Json;

// ================================================================
//  B.Unified.Payment.WeixinSample — 微信支付 SDK 示例
//  演示 6 种微信支付方式的调用方法
// ================================================================

// ---------- 1. 配置商户参数 ----------
// ⚠ 请替换为你在微信支付商户平台申请的真实参数：
//    AppId — 公众号/小程序 AppId (wx 开头)
//    MchId — 微信支付商户号 (10 位数字)
//    ApiV3Key — APIv3 密钥 (32 位，商户平台设置)
//    SerialNo — 商户证书序列号
//    PrivateKey — 商户证书私钥 (PKCS8 PEM 格式)
var config = new MchAppConfigContext();
config.NormalMchParamsMap[IfCode.WXPAY] = new WxpayNormalMchParams
{
    AppId       = "wx2421b1c4370ec43b",          // ⚠ 替换为你的公众号 AppId
    MchId       = "10000100",                     // ⚠ 替换为你的商户号
    ApiV3Key    = "your32charapiv3keyhere123456", // ⚠ 替换为 APIv3 密钥
    SerialNo    = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", // ⚠ 替换为证书序列号
    PrivateKey  = @"-----BEGIN PRIVATE KEY-----
MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQ...
-----END PRIVATE KEY-----",                     // ⚠ 替换为你的私钥
};

var service = new WeixinPaymentService();

// ---------- 2. 基础请求模板 ----------
UnifiedOrderRQ BaseRequest(string payOrderId, PayWayCode wayCode, int amount, string body)
    => new()
    {
        PayOrderId  = payOrderId,
        MchOrderNo  = $"MCH{DateTime.Now:yyyyMMddHHmmss}",
        WayCode     = wayCode,
        Amount      = amount,
        Body        = body,
        Subject     = body,
        NotifyUrl   = "https://your-domain.com/api/notify/weixin",
        ClientIp    = "127.0.0.1"
    };

void Run(string title, PayWayCode wayCode, Action<UnifiedOrderRQ>? setup = null)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"\n═══ {title} ═══");
    Console.ResetColor();

    var orderId = $"WX{DateTime.Now:yyyyMMddHHmmssfff}";
    var rq = BaseRequest(orderId, wayCode, 1, $"{title}测试");
    setup?.Invoke(rq);

    try
    {
        PrintReq(rq);
        var rs = (UnifiedOrderRS)service.Pay(rq, config);
        PrintRs(rs);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ✗ 异常: {ex.Message}");
        Console.ResetColor();
    }

    Console.WriteLine();
}

void PrintReq(UnifiedOrderRQ rq)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("── 请求 ──");
    Console.ResetColor();
    Console.WriteLine(JsonConvert.SerializeObject(new
    {
        rq.PayOrderId, rq.MchOrderNo, WayCode = rq.WayCode.Code,
        Amount = $"{rq.Amount}分 ({rq.Amount / 100m:F2}元)",
        rq.Body, rq.NotifyUrl,
        rq.ChannelUserId, rq.AuthCode, rq.ClientIp
    }, Formatting.Indented));
}

void PrintRs(UnifiedOrderRS rs)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("── 响应 ──");
    Console.ResetColor();
    Console.WriteLine(JsonConvert.SerializeObject(new
    {
        rs.PayOrderId, rs.MchOrderNo, rs.OrderState,
        rs.ErrCode, rs.ErrMsg,
        rs.PayDataType, PayData = Truncate(rs.PayData, 300),
        ChannelRet = rs.ChannelRetMsg == null ? null : new
        {
            rs.ChannelRetMsg.State,
            rs.ChannelRetMsg.ChannelOrderId,
            rs.ChannelRetMsg.ChannelErrCode,
            rs.ChannelRetMsg.ChannelErrMsg,
            rs.ChannelRetMsg.IsNeedQuery
        }
    }, Formatting.Indented));
}

string Truncate(string? s, int max = 200)
    => string.IsNullOrEmpty(s) ? "" : s.Length <= max ? s : s[..max] + "...";

// ---------- 3. 运行所有支付方式示例 ----------

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║   B.Unified.Payment.WeixinSample         ║");
Console.WriteLine("║   微信支付 SDK 完整示例                   ║");
Console.WriteLine("╚══════════════════════════════════════════╝");
Console.WriteLine("  ⚠ 请确保已在 Program.cs 顶部填入真实的微信支付参数");
Console.WriteLine("  ⚠ 当前使用测试占位参数，预期返回签名或网络错误");
Console.WriteLine();

// 1) JSAPI 支付（公众号支付 → 返回调起参数 JSON）
Run("WX_JSAPI — 公众号支付", WxPayWay.JSAPI, rq =>
{
    // ⚠ 替换为真实的用户 openid
    rq.ChannelUserId = "oUpF8uMuAJO_M2pxb1Q9zNjWeS6o";
    Console.WriteLine("  ⚠ JSAPI 需要真实 openid，当前使用示例值");
});

// 2) Native 扫码支付（返回二维码链接 code_url）
Run("WX_NATIVE — 扫码支付", WxPayWay.NATIVE);

// 3) H5 支付（返回 h5_url 跳转链接）
Run("WX_H5 — H5支付", WxPayWay.H5, rq =>
{
    rq.ClientIp = "8.8.8.8"; // H5 支付需要真实客户端 IP
    Console.WriteLine("  ⚠ H5 支付需要真实客户端 IP，当前使用示例值");
});

// 4) APP 支付（返回 APP 调起参数 JSON）
Run("WX_APP — APP支付", WxPayWay.APP);

// 5) 小程序支付（与 JSAPI 相同，使用同接口）
Run("WX_LITE — 小程序支付", WxPayWay.LITE, rq =>
{
    rq.ChannelUserId = "oUpF8uMuAJO_M2pxb1Q9zNjWeS6o";
    Console.WriteLine("  ⚠ 小程序支付与 JSAPI 接口相同，需要真实 openid");
});

// 6) 付款码支付（被扫 → 即时返回交易状态）
Run("WX_BAR — 付款码支付", WxPayWay.BAR, rq =>
{
    // ⚠ 替换为真实的用户付款码（18 位数字，以 13~15 开头）
    rq.AuthCode = "130000000000000000";
    Console.WriteLine("  ⚠ 付款码支付需要真实付款码，当前使用测试值");
});

// ---------- 4. 总结 ----------
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n═══════════════════════════════════");
Console.WriteLine("  全部 6 种微信支付方式示例执行完毕。");
Console.WriteLine("  请替换 Program.cs 中的商户参数为真实值后重新运行。");
Console.WriteLine("═══════════════════════════════════");
Console.ResetColor();

Console.WriteLine(@"
配置指南：
  1. 登录微信支付商户平台: https://pay.weixin.qq.com
  2. 获取商户号 (MchId)
  3. API 安全 → 设置 APIv3 密钥 (ApiV3Key, 32 位)
  4. API 安全 → 申请 API 证书 → 下载证书序列号 (SerialNo)
  5. API 安全 → 下载私钥文件 (apiclient_key.pem 内容作为 PrivateKey)

JSAPI/小程序支付额外需要：
  - 通过微信 OAuth2.0 获取用户 openid 填入 ChannelUserId
  - 配置 JSAPI 支付域名 (商户平台 → 产品中心 → JSAPI 支付)

Native 扫码支付和 H5 支付无需 openid，接入最简单。");
