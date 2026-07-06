using B.Unified.Payment.Abstract;
using B.Unified.Payment.YsfPay.PayWay;
using Microsoft.Extensions.DependencyInjection;

namespace B.Unified.Payment.YsfPay
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
            services.AddTransient<Abstract.IPaymentService, YsfBar>();
            services.AddTransient<Abstract.IPaymentService, YsfJsapi>();
            services.AddTransient<Abstract.IPayOrderQueryService, YsfpayPayOrderQueryService>();
            services.AddTransient<Abstract.IRefundService, YsfpayRefundService>();
            return services;
        }
    }
}
