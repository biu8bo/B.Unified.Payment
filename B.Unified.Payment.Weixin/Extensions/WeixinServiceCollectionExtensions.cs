using Microsoft.Extensions.DependencyInjection;

namespace B.Unified.Payment.Weixin.Extensions
{
    /// <summary>微信支付模块 DI 注册扩展</summary>
    public static class WeixinServiceCollectionExtensions
    {
        /// <summary>注册微信支付各支付方式及查单/退款服务到容器</summary>
        public static IServiceCollection AddWeixinPayment(this IServiceCollection services)
            => WeixinPaymentRegistration.AddTo(services);
    }
}
