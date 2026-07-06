using B.Unified.Payment.Abstract.Factory;
using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Weixin.PayWay;
using B.Unified.Payment.Weixin.Services.Close;
using B.Unified.Payment.Weixin.Services.Query;
using B.Unified.Payment.Weixin.Services.Refund;
using Microsoft.Extensions.DependencyInjection;

namespace B.Unified.Payment.Weixin.Extensions
{
    /// <summary>微信模块服务注册（DI 与 Builder 共用）</summary>
    internal static class WeixinPaymentRegistration
    {
        internal static PaymentServiceBuilder AddTo(PaymentServiceBuilder builder)
        {
            builder.AddPaymentService<WxJsapi>();
            builder.AddPaymentService<WxNative>();
            builder.AddPaymentService<WxH5>();
            builder.AddPaymentService<WxApp>();
            builder.AddPaymentService<WxLite>();
            builder.AddPaymentService<WxBar>();
            builder.AddQueryService<WeixinPayOrderQueryService>();
            builder.AddRefundService<WeixinRefundService>();
            builder.AddCloseService<WeixinPayOrderCloseService>();
            return builder;
        }

        internal static IServiceCollection AddTo(IServiceCollection services)
        {
            services.AddTransient<IPaymentService, WxJsapi>();
            services.AddTransient<IPaymentService, WxNative>();
            services.AddTransient<IPaymentService, WxH5>();
            services.AddTransient<IPaymentService, WxApp>();
            services.AddTransient<IPaymentService, WxLite>();
            services.AddTransient<IPaymentService, WxBar>();
            services.AddTransient<IPayOrderQueryService, WeixinPayOrderQueryService>();
            services.AddTransient<IRefundService, WeixinRefundService>();
            services.AddTransient<IPayOrderCloseService, WeixinPayOrderCloseService>();
            return services;
        }
    }
}
