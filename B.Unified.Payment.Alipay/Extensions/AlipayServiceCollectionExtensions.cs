using Microsoft.Extensions.DependencyInjection;

namespace B.Unified.Payment.Alipay
{
    /// <summary>支付宝支付模块 DI 注册扩展</summary>
    public static class AlipayServiceCollectionExtensions
    {
        /// <summary>注册支付宝各支付方式及查单/退款服务到容器</summary>
        public static IServiceCollection AddAlipayPayment(this IServiceCollection services)
            => AlipayPaymentRegistration.AddTo(services);
    }
}
