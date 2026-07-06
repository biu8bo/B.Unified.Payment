using B.Unified.Payment.Abstract;

namespace B.Unified.Payment.Weixin
{
    /// <summary>PaymentServiceBuilder 微信扩展</summary>
    public static class PaymentServiceBuilderExtensions
    {
        /// <summary>注册微信全部支付方式及查单/退款服务</summary>
        public static PaymentServiceBuilder AddWeixin(this PaymentServiceBuilder builder)
            => WeixinPaymentRegistration.AddTo(builder);
    }
}
