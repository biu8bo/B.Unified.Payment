# B.Unified.Payment

统一支付 SDK — 支持支付宝、微信支付、云闪付三大渠道，提供统一的支付、查单、退款接口。

> ⚠️ **当前支持范围**
> 
> | 功能 | 状态 |
> |------|------|
> | 统一支付（`IPaymentService.PayAsync`） | ✅ 已实现 |
> | 统一查单（`IPayOrderQueryService.QueryAsync`） | ✅ 已实现 |
> | 统一退款（`IRefundService.RefundAsync`） | ✅ 已实现 |
> | 退款查单（`IRefundService.QueryAsync`） | ✅ 已实现 |
> | 分账、服务商子商户、支付回调验签、关单 等 | ❌ 暂不支持 |
> 
> 需要其他功能请提交 [Issue](https://github.com/your-repo/issues)，分账、子商户等功能可通过 PR 贡献。

## 项目介绍

B.Unified.Payment 是一套 .NET 统一支付 SDK，参考 [Jeepay](https://github.com/jeequan/jeepay) 的 Java 架构设计，用 C# 重新实现。核心设计原则：

- **抽象层零业务依赖** — `Abstract` 项目只包含接口、抽象类、共享 DTO、工厂，不含渠道具体实现
- **渠道实现隔离** — 每个支付渠道（Alipay / Weixin / YsfPay）是独立类库，互不引用，按需安装
- **内置 DI 支持** — 每个渠道自带 `AddAlipayPayment()` / `AddWeixinPayment()` 扩展方法，工厂根据 `ifCode` + `wayCode` 自动路由到对应 PayWay 实现
- **统一接口** — `IPaymentService` / `IPayOrderQueryService` / `IRefundService` 统一支付、查单、退款能力
- **强类型安全** — `PayWayCode` / `PayDataTypeCode` 值类、`PayOrderState` 枚举，编译期防止拼写错误
- **内置日志** — `PayLogger` 基于 `Microsoft.Extensions.Logging`，支持标准 .NET 日志体系
- **全异步** — 所有接口方法均为 `Task` 异步（`PayAsync` / `QueryAsync` / `RefundAsync`）
- **盛派 SDK** — 微信支付模块使用 [Senparc.Weixin.TenPayV3](https://github.com/JeffreySu/WeiXinMPSDK)，签名和 HTTP 调用均由 SDK 内部完成

## 已完成功能

### 支付渠道（16 种支付方式）

| 渠道 | 支付方式 | 接口 | 说明 |
|------|----------|------|------|
| 支付宝 | ALI_BAR | `alipay.trade.pay` | 条码支付（被扫） |
| 支付宝 | ALI_PC | `alipay.trade.page.pay` | PC 网站支付 → HTML 表单 |
| 支付宝 | ALI_WAP | `alipay.trade.wap.pay` | 手机网站支付 → HTML 表单 |
| 支付宝 | ALI_JSAPI | `alipay.trade.create` | 生活号 / JSAPI 支付 |
| 支付宝 | ALI_APP | `alipay.trade.app.pay` | APP 支付 → SDK 调起串 |
| 支付宝 | ALI_QR | `alipay.trade.precreate` | 扫码支付（正扫）→ 二维码 |
| 支付宝 | ALI_LITE | `alipay.trade.create` | 小程序支付 |
| 支付宝 | ALI_OC | `alipay.trade.precreate` | 订单码支付（QR_CODE_OFFLINE） |
| 微信 | WX_JSAPI | `POST /v3/pay/transactions/jsapi` | 公众号支付 |
| 微信 | WX_NATIVE | `POST /v3/pay/transactions/native` | 扫码支付 → code_url |
| 微信 | WX_H5 | `POST /v3/pay/transactions/h5` | H5 支付 → h5_url |
| 微信 | WX_APP | `POST /v3/pay/transactions/app` | APP 支付 |
| 微信 | WX_LITE | 同 JSAPI | 小程序支付 |
| 微信 | WX_BAR | `POST /v3/pay/transactions/micropay` | 付款码支付（被扫） |
| 云闪付 | YSF_BAR | `/gateway/api/pay/micropay` | 条码支付（被扫） |
| 云闪付 | YSF_JSAPI | `/gateway/api/pay/unifiedorder` | JSAPI 支付 → redirectUrl |

### 核心接口

| 接口 | 说明 | 方法 |
|------|------|------|
| `IPaymentService` | 统一支付 | `Task<AbstractRS> PayAsync(UnifiedOrderRQ, MchAppConfigContext)` |
| `IPayOrderQueryService` | 支付订单查询 | `Task<ChannelRetMsg> QueryAsync(payOrderId, MchAppConfigContext)` |
| `IRefundService` | 退款 + 退款查单 | `Task<ChannelRetMsg> RefundAsync(RefundOrderRQ, ctx)` / `Task<ChannelRetMsg> QueryAsync(...)` |
| `IPaymentServiceFactory` | 服务工厂 | `GetPaymentService(ifCode, wayCode)` / `GetQueryService(ifCode)` / `GetRefundService(ifCode)` |

### 其他特性

- `AbstractRS.ChannelOriginResponse` — 每个响应包含渠道原始返回 JSON，便于排查
- `PayLogger` — 基于 `Microsoft.Extensions.Logging`，工厂构造时自动初始化
- `AbstractPaymentService` — 模板方法：`ValidateCommon` → `PreCheckWay` → `ExecutePayAsync` → `FinalizePayData`，自动异常捕获
- 每个 PayWay 类（如 `AliQr`、`WxJsapi`）直接实现 `IPaymentService`，由工厂按 `wayCode` 路由
- `PayWayCode` / `PayDataTypeCode` — 值类（`==` / `Equals` / 隐式 `string`），强类型防拼写错误
- `PayOrderState` — 支付订单状态枚举（与 Jeepay `orderState` 数值对齐）
- `UnifiedOrderRS.FinalizePayData()` — 自动填充 `PayDataType` / `PayData`（对齐 Jeepay 统一下单响应逻辑）
- 微信模块使用 [Senparc.Weixin.TenPayV3](https://github.com/JeffreySu/WeiXinMPSDK)，无需手动计算 RSA 签名
- 秘钥统一从 `keys.json` 读取，已在 `.gitignore` 中忽略

## 项目结构

```
B.Unified.Payment/
├── B.Unified.Payment.Abstract/          # 抽象层（接口 + 共享 DTO + 工厂）
│   ├── IPaymentService.cs               #   统一支付接口
│   ├── IPayOrderQueryService.cs         #   统一查单接口
│   ├── IRefundService.cs                #   统一退款接口
│   ├── PaymentServiceFactory.cs         #   服务工厂（ifCode + wayCode → PayWay 路由）
│   ├── AbstractPaymentService.cs        #   支付抽象基类（模板方法）
│   ├── Models/
│   │   ├── PayWayCode.cs                #   支付方式值类
│   │   ├── Payment/PayDataTypeCode.cs   #   支付参数类型值类
│   │   ├── Payment/PayOrderState.cs     #   订单状态枚举
│   │   ├── ChannelState.cs              #   渠道状态枚举
│   │   ├── ChannelRetMsg.cs             #   渠道状态消息包装
│   │   ├── MchAppConfigContext.cs       #   商户配置上下文（仅普通商户模式）
│   │   ├── Payment/UnifiedOrderRQ.cs    #   统一支付请求
│   │   ├── Payment/UnifiedOrderRS.cs    #   统一支付响应
│   │   ├── Payment/CommonPayDataRS.cs   #   通用支付数据响应
│   │   └── Refund/RefundOrderRQ.cs      #   退款请求
│   └── Diagnostics/PayLogger.cs         #   日志工具
│
├── B.Unified.Payment.Alipay/            # 支付宝实现
│   ├── Constants/                       #   IfCode + AlipayPayWay 常量
│   ├── Models/                          #   AlipayNormalMchParams + 8 个 RS
│   ├── PayWay/                          #   8 个 PayWay（各实现 IPaymentService）+ ClientFactory
│   ├── AlipayPayOrderQueryService.cs    #   查单
│   ├── AlipayRefundService.cs           #   退款
│   └── AlipayServiceCollectionExtensions.cs  # DI: AddAlipayPayment()
│
├── B.Unified.Payment.Weixin/            # 微信支付实现（Senparc SDK）
│   ├── Constants/                       #   IfCode + WxPayWay 常量
│   ├── Models/                          #   WxpayNormalMchParams + 6 个 RS
│   ├── PayWay/                          #   6 个 PayWay（各实现 IPaymentService）+ WxPayHelper
│   ├── WeixinPayOrderQueryService.cs    #   查单（Senparc: OrderQueryByOutTradeNoAsync）
│   ├── WeixinRefundService.cs           #   退款（Senparc: RefundAsync / RefundQueryAsync）
│   └── WeixinServiceCollectionExtensions.cs  # DI: AddWeixinPayment()
│
├── B.Unified.Payment.YsfPay/            # 云闪付实现
│   ├── Constants/                       #   IfCode + YsfPayWay 常量
│   ├── Models/                          #   YsfpayIsvParams + RS
│   ├── PayWay/                          #   YsfBar + YsfJsapi（各实现 IPaymentService）
│   ├── YsfHttpUtil.cs                   #   RSA SHA256withRSA 签名 + HTTP
│   ├── YsfpayPayOrderQueryService.cs    #   查单
│   ├── YsfpayRefundService.cs           #   退款
│   └── YsfPayServiceCollectionExtensions.cs  # DI: AddYsfPayPayment()
│
├── B.Unified.Payment.AlipaySample/      # 支付宝控制台示例（支付/查单/退款）
├── B.Unified.Payment.WeixinSample/      # 微信控制台示例（支付/查单/退款）
├── B.Unified.Payment.YsfPaySample/      # 云闪付控制台示例（支付/查单/退款）
├── B.Unified.Payment.Sample.WebApi/     # Web API 示例（DI + 控制器）
├── keys.json                            # 秘钥配置（.gitignore 忽略）
├── keys.template.json                   # 秘钥模板
└── .gitignore
```

## 快速开始


### 1. 配置秘钥

```bash
cp keys.template.json keys.json
# 编辑 keys.json 填入真实秘钥
```

### 2. 直接调用示例

```csharp
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Alipay.Constants;
using B.Unified.Payment.Alipay.Models;
using B.Unified.Payment.Alipay.PayWay;
using Microsoft.Extensions.Logging.Abstractions;

// 构建配置
var config = new MchAppConfigContext();
config.NormalMchParamsMap[IfCode.ALIPAY] = new AlipayNormalMchParams
{
    AppId = "你的AppId", PrivateKey = "应用私钥",
    AlipayPublicKey = "支付宝公钥", SignType = "RSA2"
};

// 发起支付
var rq = new UnifiedOrderRQ
{
    PayOrderId = "ORDER001", WayCode = AlipayPayWay.QR,
    Amount = 100, Subject = "测试商品", Body = "描述",
    NotifyUrl = "https://your-domain.com/api/notify/alipay"
};

// 方式 A：工厂路由（推荐）
var factory = new PaymentServiceFactory(
    new IPaymentService[] { new AliBar(), new AliPc(), new AliWap(), new AliJsapi(), new AliApp(), new AliQr(), new AliLite(), new AliOc() },
    Array.Empty<IPayOrderQueryService>(),
    Array.Empty<IRefundService>(),
    NullLoggerFactory.Instance);
var service = factory.GetPaymentService(IfCode.ALIPAY, AlipayPayWay.QR);

// 方式 B：直接实例化对应 PayWay
// var service = new AliQr();

var rs = (UnifiedOrderRS)await service.PayAsync(rq, config);
```

### 3. DI 方式调用（推荐）

```csharp
// Program.cs
using B.Unified.Payment.Alipay;
using B.Unified.Payment.Weixin;
using B.Unified.Payment.Abstract;

builder.Services.AddAlipayPayment();   // 只要支付宝就只加这个
builder.Services.AddWeixinPayment();   // 需要微信再加这个
builder.Services.AddSingleton<IPaymentServiceFactory, PaymentServiceFactory>();
```

```csharp
// 控制器中
public class PaymentController : ControllerBase
{
    private readonly IPaymentServiceFactory _factory;
    public PaymentController(IPaymentServiceFactory factory) => _factory = factory;

    [HttpPost("pay")]
    public async Task<IActionResult> Pay([FromBody] PayRequest req)
    {
        var service = _factory.GetPaymentService(req.IfCode, req.WayCode); // ifCode + wayCode
        var rs = await service.PayAsync(buildRq(req), loadConfig(req.IfCode));
        return Ok(rs);
    }
}
```

### 4. 查单 / 退款

```csharp
// 查单
var queryService = new AlipayPayOrderQueryService();
var result = await queryService.QueryAsync("ORDER001", config);

// 退款
var refundService = new AlipayRefundService();
var result = await refundService.RefundAsync(new RefundOrderRQ
{
    PayOrderId = "ORDER001", RefundOrderId = "RF001",
    RefundAmount = 100, RefundReason = "用户申请"
}, config);
```

### 5. 运行 Sample

```bash
# 控制台示例
cd B.Unified.Payment.AlipaySample && dotnet run

# Web API 示例（Swagger 不包含，直接 curl）
cd B.Unified.Payment.Sample.WebApi && dotnet run

curl -X POST http://localhost:5000/api/payment/pay \
  -H "Content-Type: application/json" \
  -d '{"ifCode":"alipay","wayCode":"ALI_QR","amount":100}'
```

## 渠道状态映射

所有渠道通过 `ChannelState` 枚举返回统一状态（包装在 `ChannelRetMsg.State` 中）：

| 状态 | 含义 | 支付宝 | 微信 V3 | 云闪付 |
|------|------|--------|--------|--------|
| `CONFIRM_SUCCESS` | 明确成功 | `TRADE_SUCCESS` | `SUCCESS` | `respCode=00` |
| `CONFIRM_FAIL` | 明确失败 | `IsError` | `CLOSED/REVOKED/PAYERROR` | `respCode≠00` |
| `WAITING` | 处理中 | `WAIT_BUYER_PAY` | `USERPAYING/PROCESSING` | `02/05` |
| `UNKNOWN` | 状态不明 | — | 其他 | — |
| `SYS_ERROR` | 系统异常 | 异常 | 异常 | 异常 |

## 配置参数说明

### 支付宝（keys.json → Alipay）

| 字段 | 说明 |
|------|------|
| `AppId` | 支付宝开放平台应用 ID |
| `PrivateKey` | 应用私钥（PKCS8 格式） |
| `AlipayPublicKey` | 支付宝公钥 |
| `Sandbox` | 1=沙箱, 0=生产 |
| `SignType` | 签名方式，推荐 RSA2 |

### 微信支付（keys.json → Weixin）

| 字段 | 说明 |
|------|------|
| `AppId` | 公众号/小程序 AppId |
| `MchId` | 微信支付商户号 |
| `ApiV3Key` | APIv3 密钥（32 位） |
| `SerialNo` | 商户证书序列号 |
| `PrivateKey` | 商户私钥（PKCS8 PEM 格式） |

### 云闪付（keys.json → YsfPay）

| 字段 | 说明 |
|------|------|
| `SerProvId` | 服务商标识 |
| `MerId` | 子商户号 |
| `PrivateCert` | PKCS12 证书（Base64） |
| `PrivateCertPwd` | 证书密码 |
| `YsfpayPublicKey` | 云闪付公钥 |
| `Sandbox` | 1=沙箱, 0=生产 |

## 技术栈

| 组件 | 版本 | 用途 |
|------|------|------|
| .NET | netstandard2.0 / 2.1 / net8.0 | SDK 及 Sample 目标框架 |
| Microsoft.Extensions.Logging | 8.0 | 日志抽象层 |
| Microsoft.Extensions.DependencyInjection | 8.0 | DI 容器集成 |
| AlipaySDKNet.Standard | 4.9.1234 | 支付宝官方 SDK |
| Senparc.Weixin.TenPayV3 | 2.4.0 | 微信支付 Senparc SDK |
| Newtonsoft.Json | 13.0.x | JSON 序列化 |

## 贡献

目前仅实现了统一支付、统一查单、统一退款、退款查单四个核心接口，仅支持普通商户直连模式。

以下功能不在当前计划中，欢迎通过 PR 贡献：

| 功能 | 说明 |
|------|------|
| 分账 | 订单分账接口 |
| 服务商/子商户 | ISV 服务商模式 |
| 支付回调验签 | 各渠道回调通知解密验签 |

> 需要其他功能请提交 [Issue](https://github.com/biu8bo/B.Unified.Payment/issues)。

## 💐 特别鸣谢

| 项目 | 说明 |
|------|------|
| [Jeepay](https://github.com/jeequan/jeepay) | Java 支付系统，本项目的架构设计参考 |
| [AlipaySDKNet.Standard](https://www.nuget.org/packages/AlipaySDKNet.Standard) | 支付宝官方 .NET SDK，支付宝模块基于此实现 |
| [Senparc.Weixin](https://github.com/JeffreySu/WeiXinMPSDK) | 微信 .NET SDK（盛派），微信支付模块基于此实现 |

## License

MIT