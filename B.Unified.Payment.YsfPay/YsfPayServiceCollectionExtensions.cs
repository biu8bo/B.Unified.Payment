using B.Unified.Payment.YsfPay.PayWay;
using Microsoft.Extensions.DependencyInjection;

namespace B.Unified.Payment.YsfPay
{
    /// <summary>云闪付支付模块 DI 注册扩展</summary>
    public static class YsfPayServiceCollectionExtensions
    {
        /// <summary>注册云闪付各支付方式及查单/退款服务</summary>
        public static IServiceCollection AddYsfPayPayment(this IServiceCollection services)
        {
            services.AddTransient<Abstract.IPaymentService, YsfBar>();
            services.AddTransient<Abstract.IPaymentService, YsfJsapi>();
            services.AddTransient<Abstract.IPayOrderQueryService, YsfpayPayOrderQueryService>();
            services.AddTransient<Abstract.IRefundService, YsfpayRefundService>();
            return services;
        }
    }
}
