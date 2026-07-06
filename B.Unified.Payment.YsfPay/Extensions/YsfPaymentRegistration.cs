using B.Unified.Payment.Abstract.Factory;
using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.YsfPay.PayWay;
using B.Unified.Payment.YsfPay.Services.Query;
using B.Unified.Payment.YsfPay.Services.Refund;
using Microsoft.Extensions.DependencyInjection;

namespace B.Unified.Payment.YsfPay.Extensions
{
    /// <summary>云闪付模块服务注册（DI 与 Builder 共用）</summary>
    internal static class YsfPaymentRegistration
    {
        internal static PaymentServiceBuilder AddTo(PaymentServiceBuilder builder)
        {
            builder.AddPaymentService<YsfBar>();
            builder.AddPaymentService<YsfJsapi>();
            builder.AddQueryService<YsfpayPayOrderQueryService>();
            builder.AddRefundService<YsfpayRefundService>();
            return builder;
        }

        internal static IServiceCollection AddTo(IServiceCollection services)
        {
            services.AddTransient<IPaymentService, YsfBar>();
            services.AddTransient<IPaymentService, YsfJsapi>();
            services.AddTransient<IPayOrderQueryService, YsfpayPayOrderQueryService>();
            services.AddTransient<IRefundService, YsfpayRefundService>();
            return services;
        }
    }
}
