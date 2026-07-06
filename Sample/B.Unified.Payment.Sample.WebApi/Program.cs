using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Factory;
using B.Unified.Payment.Alipay;
using B.Unified.Payment.Alipay.Extensions;
using B.Unified.Payment.Weixin;
using B.Unified.Payment.Weixin.Extensions;
using B.Unified.Payment.YsfPay;
using B.Unified.Payment.YsfPay.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

// 按需注册渠道
builder.Services.AddAlipayPayment();
builder.Services.AddWeixinPayment();
builder.Services.AddYsfPayPayment();

// 注册工厂 — 自动初始化 PayLogger（无需手动 Configure）
builder.Services.AddSingleton<IPaymentServiceFactory, PaymentServiceFactory>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new B.Unified.Payment.Sample.WebApi.Json.PayWayCodeJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new B.Unified.Payment.Sample.WebApi.Json.PayDataTypeCodeJsonConverter());
    });

var app = builder.Build();
app.MapControllers();
app.Run();