using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace B.Unified.Payment.Weixin
{
    /// <summary>微信支付模块 DI 注册扩展</summary>
    public static class WeixinServiceCollectionExtensions
    {
        /// <summary>注册微信支付/查单/退款服务到容器</summary>
        public static IServiceCollection AddWeixinPayment(this IServiceCollection services)
        {
            services.TryAddTransient<Abstract.IPaymentService, WeixinPaymentService>();
            services.TryAddTransient<Abstract.IPayOrderQueryService, WeixinPayOrderQueryService>();
            services.TryAddTransient<Abstract.IRefundService, WeixinRefundService>();
            return services;
        }
    }
}