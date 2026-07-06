using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace B.Unified.Payment.Alipay
{
    /// <summary>支付宝支付模块 DI 注册扩展</summary>
    public static class AlipayServiceCollectionExtensions
    {
        /// <summary>注册支付宝支付/查单/退款服务到容器</summary>
        public static IServiceCollection AddAlipayPayment(this IServiceCollection services)
        {
            services.TryAddTransient<Abstract.IPaymentService, AlipayPaymentService>();
            services.TryAddTransient<Abstract.IPayOrderQueryService, AlipayPayOrderQueryService>();
            services.TryAddTransient<Abstract.IRefundService, AlipayRefundService>();
            return services;
        }
    }
}