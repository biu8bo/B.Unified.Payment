using B.Unified.Payment.Alipay.PayWay;
using B.Unified.Payment.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace B.Unified.Payment.Alipay
{
    /// <summary>支付宝模块服务注册（DI 与 Builder 共用）</summary>
    internal static class AlipayPaymentRegistration
    {
        internal static PaymentServiceBuilder AddTo(PaymentServiceBuilder builder)
        {
            builder.AddPaymentService<AliBar>();
            builder.AddPaymentService<AliPc>();
            builder.AddPaymentService<AliWap>();
            builder.AddPaymentService<AliJsapi>();
            builder.AddPaymentService<AliApp>();
            builder.AddPaymentService<AliQr>();
            builder.AddPaymentService<AliLite>();
            builder.AddPaymentService<AliOc>();
            builder.AddQueryService<AlipayPayOrderQueryService>();
            builder.AddRefundService<AlipayRefundService>();
            return builder;
        }

        internal static IServiceCollection AddTo(IServiceCollection services)
        {
            services.AddTransient<Abstract.IPaymentService, AliBar>();
            services.AddTransient<Abstract.IPaymentService, AliPc>();
            services.AddTransient<Abstract.IPaymentService, AliWap>();
            services.AddTransient<Abstract.IPaymentService, AliJsapi>();
            services.AddTransient<Abstract.IPaymentService, AliApp>();
            services.AddTransient<Abstract.IPaymentService, AliQr>();
            services.AddTransient<Abstract.IPaymentService, AliLite>();
            services.AddTransient<Abstract.IPaymentService, AliOc>();
            services.AddTransient<Abstract.IPayOrderQueryService, AlipayPayOrderQueryService>();
            services.AddTransient<Abstract.IRefundService, AlipayRefundService>();
            return services;
        }
    }
}
