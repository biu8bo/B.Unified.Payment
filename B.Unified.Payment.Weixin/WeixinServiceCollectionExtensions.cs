using B.Unified.Payment.Weixin.PayWay;
using Microsoft.Extensions.DependencyInjection;

namespace B.Unified.Payment.Weixin
{
    /// <summary>微信支付模块 DI 注册扩展</summary>
    public static class WeixinServiceCollectionExtensions
    {
        /// <summary>注册微信各支付方式及查单/退款服务</summary>
        public static IServiceCollection AddWeixinPayment(this IServiceCollection services)
        {
            services.AddTransient<Abstract.IPaymentService, WxJsapi>();
            services.AddTransient<Abstract.IPaymentService, WxNative>();
            services.AddTransient<Abstract.IPaymentService, WxH5>();
            services.AddTransient<Abstract.IPaymentService, WxApp>();
            services.AddTransient<Abstract.IPaymentService, WxLite>();
            services.AddTransient<Abstract.IPaymentService, WxBar>();
            services.AddTransient<Abstract.IPayOrderQueryService, WeixinPayOrderQueryService>();
            services.AddTransient<Abstract.IRefundService, WeixinRefundService>();
            return services;
        }
    }
}
