using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace B.Unified.Payment.YsfPay
{
    /// <summary>云闪付支付模块 DI 注册扩展</summary>
    public static class YsfPayServiceCollectionExtensions
    {
        /// <summary>注册云闪付支付/查单/退款服务到容器</summary>
        public static IServiceCollection AddYsfPayPayment(this IServiceCollection services)
        {
            services.TryAddTransient<Abstract.IPaymentService, YsfpayPaymentService>();
            services.TryAddTransient<Abstract.IPayOrderQueryService, YsfpayPayOrderQueryService>();
            services.TryAddTransient<Abstract.IRefundService, YsfpayRefundService>();
            return services;
        }
    }
}