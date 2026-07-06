using B.Unified.Payment.Alipay.PayWay;
using Microsoft.Extensions.DependencyInjection;

namespace B.Unified.Payment.Alipay
{
    /// <summary>支付宝支付模块 DI 注册扩展</summary>
    public static class AlipayServiceCollectionExtensions
    {
        /// <summary>注册支付宝各支付方式及查单/退款服务</summary>
        public static IServiceCollection AddAlipayPayment(this IServiceCollection services)
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
