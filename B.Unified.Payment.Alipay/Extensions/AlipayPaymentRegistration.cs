using B.Unified.Payment.Abstract.Factory;
using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Alipay.PayWay;
using B.Unified.Payment.Alipay.Services.Close;
using B.Unified.Payment.Alipay.Services.Query;
using B.Unified.Payment.Alipay.Services.Refund;
using Microsoft.Extensions.DependencyInjection;

namespace B.Unified.Payment.Alipay.Extensions
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
            builder.AddCloseService<AlipayPayOrderCloseService>();
            return builder;
        }

        internal static IServiceCollection AddTo(IServiceCollection services)
        {
            services.AddTransient<IPaymentService, AliBar>();
            services.AddTransient<IPaymentService, AliPc>();
            services.AddTransient<IPaymentService, AliWap>();
            services.AddTransient<IPaymentService, AliJsapi>();
            services.AddTransient<IPaymentService, AliApp>();
            services.AddTransient<IPaymentService, AliQr>();
            services.AddTransient<IPaymentService, AliLite>();
            services.AddTransient<IPaymentService, AliOc>();
            services.AddTransient<IPayOrderQueryService, AlipayPayOrderQueryService>();
            services.AddTransient<IRefundService, AlipayRefundService>();
            services.AddTransient<IPayOrderCloseService, AlipayPayOrderCloseService>();
            return services;
        }
    }
}
