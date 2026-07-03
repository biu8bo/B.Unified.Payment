using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Alipay;
using B.Unified.Payment.Alipay.Constants;
using B.Unified.Payment.Alipay.Models;
using Newtonsoft.Json;

// ================================================================
//  B.Unified.Payment.AlipaySample — 支付宝支付 SDK 示例
//  演示 8 种支付宝支付方式的调用方法
// ================================================================

// ---------- 1. 配置商户参数 ----------
var config = new MchAppConfigContext();
config.NormalMchParamsMap[IfCode.ALIPAY] = new AlipayNormalMchParams
{
    AppId           = "2021000147628093",
    PrivateKey      = "MIIEowIBAAKCAQEAzeBX09MzW16qmI8V0/olzpKxq7JI8mxF+7a7LNHUJJlzSFYJYigdtqztOB5zeaJlzGMQ0jDjT6qZqIZr3v7ADM7BGGzCG3piOagK9W0KKfKQ4jjQ4DEPYQbZCTt/xr3hd4/mia16nHlFPeTHcddvbLZYOVvlMn/UM5b6pc1i7OfQ6TOtoqEwsjodw7RsPx2gDh+N5V7WNDXPeMsCoe0k7hMoMZ6FSE0h3J0XV64I9KlX84q65i7lrmim8dVVv9Vd9MvnHSn/Q+uvdo3IA6J+o0jG3xBeCgkAmiMkxs8BKFLp9oa8MXFZB74IeFF4fLZviTMDjmbyhd6IOp5TCUJLFwIDAQABAoIBACFJOGoDJ7aKI8Luv3S6aQpxJVDBTpIDd30vGiww8L/KH51+a533Jna2ltQP+FOeMh9NlRam2Nm0l4tr0F0JizuG4il0zB1tOBxiUwNDUfVeRpaM4RieVgI1/TlE6W/Um3OdTITOC5jo8o0DREvfrSBCixkbBn+Xs1N0Aap0/p2WwvLjFB2DwAOAqlsZZQ74aNyia8QgoBMgY6VtltPRYhf2l/CuOC754VN2gwMULIJduIXHzd6k2zb7b+5ogtuhHJkFI/E71QnUP3qdER9udyazvXTKOnphhXvf9nkmly4RPouyOmnOCT1w66VjZU2rQXMFOcLXbbfrJJcjoYHsswECgYEA+LJ+hV5mww3BK5121hAw960wvXs4Kp1WUcEtCX8njDI9NNefnxXsfQKfiZCLp5S1+3Y66GsIeAJR1alk1zY/lmt1HXCNX4o7NyFRRxsfzXTv6jxTuMYvj05whIGYjYblFAEj4OkCnho80KSPWzrR8PLCnMikIFcexzQzm9ssziECgYEA0+v0rI18G1KyydsoObCUhqA/4rb4eKp60lSH7tujufIIySMmBINP6vraH6qMZ5hVq9fbFtfb37YsGsgXKwlw231QSiX/VdohHvfeYd/rY0LD1rpTIdHQXJS5C1IjBN1usNNyXVwi5AMyVVfjfJ2XNB6jHmMow1DXnKIZVC7NwjcCgYEA9kDf8LV58XfkJ4jCy9G6evSdx3GEOwYSG9+49adXhIWWf4Vmg8LUqS/4wuFCt4wT6ku2pr6c4yAA4hzaQhNwQURj8eOpyMl6OuudrFfaVLmOehSEHfj3zOGxnjMo2DKTEAzU9vYiZmS6hSn83SvQB9KJC2/MvE0np74zwAb1RaECgYA2BVXvjnludZxBvG36lrqlvr/KSR35lGuOpiGoj7Ciu8Hlk+IjEF4U5jEoFU+JMNnV3kZpAkl4M3X2tb7CJ7vvF3iaDimSdvIudLzpci0Mtn45hHGgk11r3DV3X06x9Mg8pwnmJpB2UyJHgwnoQDvE+3JVUq2XbEoqEWAnh27H7QKBgGzxVHnvGWjcDZl7/SxKCmEak0CtZ88tl/CnTF0QTWN7T68Fj0ag+Cq7s77ZTHP2KPS0AvfdRD2qkLOXI/BQjIGkfN7eiikrjU7C9SgIkh1k9d+1feAYCFzzYUgqYlsrx2ajdSzE5Crl/cTNK/iI+oICVN9RbE+MwNoqD6Agrg9K",
    AlipayPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAlTbnWLOqL7YO+dE35+DTE7O5/zX7MtCjXBa6QWcxL7iCrVetiy9GDPaN+OLvkO2iHeTOeZzUwvmYOErAQPYPu4+lcP+fJYdpBgeHugrErtdj6Hzq2IrIMA9JGzJGXJxF1Sd2KNYD1ye+Eo9NySs0HOvN2G+0ArvkZLlxZpIM92vV85BVmaiO1AE4c0BRyGASrgwqIM1WaXtEzWuCJI53i5t64aTFaMc/Y6Iz3yf41YsRrVA48dwzkT2biKkivi9AHgDq0dQ4eA9WC2BX1zUu8AHpL2mSMD5gKJabnsoOVv0SHqie5cj9GLAgi0bb93KwmZ1X1bSNNQXULTF5EchZTwIDAQAB",
    Sandbox         = 1,
    SignType        = "RSA2"
};

var service = new AlipayPaymentService();

// ---------- 2. 基础请求模板 ----------
UnifiedOrderRQ BaseRequest(string payOrderId, PayWayCode wayCode, long amount, string subject)
    => new()
    {
        PayOrderId  = payOrderId,
        MchOrderNo  = $"MCH{DateTime.Now:yyyyMMddHHmmss}",
        WayCode     = wayCode,
        Amount      = amount,
        Subject     = subject,
        Body        = subject,
        NotifyUrl   = "https://your-domain.com/api/notify/alipay",
        ReturnUrl   = "https://your-domain.com/return",
        ClientIp    = "127.0.0.1"
    };

void Run(string title, PayWayCode wayCode, Action<UnifiedOrderRQ>? setup = null)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"\n═══ {title} ═══");
    Console.ResetColor();

    var orderId = $"TEST{DateTime.Now:yyyyMMddHHmmssfff}";
    var rq = BaseRequest(orderId, wayCode, 1, $"测试{title}");
    setup?.Invoke(rq);

    PrintReq(rq);
    var rs = (UnifiedOrderRS)service.Pay(rq, config);
    PrintRs(rs);
}

void PrintReq(UnifiedOrderRQ rq)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("── 请求 ──");
    Console.ResetColor();
    Console.WriteLine(JsonConvert.SerializeObject(rq, Formatting.Indented));
}

void PrintRs(UnifiedOrderRS rs)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("── 响应 ──");
    Console.ResetColor();
    Console.WriteLine(JsonConvert.SerializeObject(rs, Formatting.Indented));
    Console.WriteLine();
}

string Truncate(string? s, int max = 200)
    => string.IsNullOrEmpty(s) ? "" : s.Length <= max ? s : s[..max] + "...";

// ---------- 3. 运行所有支付方式示例 ----------

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║   B.Unified.Payment.AlipaySample         ║");
Console.WriteLine("║   支付宝支付 SDK 完整示例                 ║");
Console.WriteLine($"║   AppId: {config.GetNormalMchParams<AlipayNormalMchParams>(IfCode.ALIPAY)!.AppId}    ║");
Console.WriteLine("╚══════════════════════════════════════════╝");

// 1) 扫码支付（预创建二维码）
Run("ALI_QR — 扫码支付", AlipayPayWay.QR);

// 2) PC 网站支付（返回 HTML 表单）
Run("ALI_PC — PC网站支付", AlipayPayWay.PC);

// 3) WAP 手机网站支付
Run("ALI_WAP — 手机网站支付", AlipayPayWay.WAP);

// 4) APP 支付（返回调起参数字符串）
Run("ALI_APP — APP支付", AlipayPayWay.APP);

// 5) JSAPI 生活号支付（需要真实 buyer_id）
Run("ALI_JSAPI — 生活号支付", AlipayPayWay.JSAPI, rq =>
{
    // ⚠ 替换为真实的支付宝买家 userId (2088 开头)
    rq.ChannelUserId = "2088000000000000";
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("  ⚠ JSAPI 需要真实 buyer_id，当前使用测试值，预期返回业务失败");
    Console.ResetColor();
});

// 6) 小程序支付（需要真实 buyer_id）
Run("ALI_LITE — 小程序支付", AlipayPayWay.LITE, rq =>
{
    rq.ChannelUserId = "2088000000000000";
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("  ⚠ 小程序支付需要真实 buyer_id，预期返回业务失败");
    Console.ResetColor();
});

// 7) 条码支付（需要真实付款码）
Run("ALI_BAR — 条码支付", AlipayPayWay.BAR, rq =>
{
    // ⚠ 替换为真实的用户付款码（28 位数字）
    rq.AuthCode = "2800000000000000000000000000";
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("  ⚠ 条码支付需要真实付款码，当前使用测试值，预期返回业务失败");
    Console.ResetColor();
});

// 8) 订单码支付
Run("ALI_OC — 订单码支付", AlipayPayWay.OC);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n═══════════════════════════════════");
Console.WriteLine("  全部 8 种支付方式示例执行完毕。");
Console.WriteLine("  请检查各响应中的 ErrCode/PayDataType 确认结果。");
Console.WriteLine("═══════════════════════════════════");
Console.ResetColor();
