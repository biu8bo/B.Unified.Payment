using B.Unified.Payment.Abstract;
using B.Unified.Payment.Weixin.PayWay;
using Microsoft.Extensions.DependencyInjection;

namespace B.Unified.Payment.Weixin
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
            return builder;
        }

        internal static IServiceCollection AddTo(IServiceCollection services)
        {
            services.AddTransient<Abstract.IPaymentService, WxJsapi>();
            services.AddTransient<Abstract.IPaymentService, WxNative>();
            services.AddTransient<Abstract.IPaymentService, WxH5>();
            services.AddTransient<Abstract.IPaymentService, WxApp>();
            services.AddTransient<Abstract.IPaymentService, WxLite>();
            services.AddTransient<Abstract.IPaymentService, WxBar>();
            services.AddTransient<Abstract.IPayOrderQueryService, WeixinPayOrderQueryService>();
            services.AddTransient<Abstract.IRefundService, WeixinRefundService>();
            return services;
        }
    }
}
