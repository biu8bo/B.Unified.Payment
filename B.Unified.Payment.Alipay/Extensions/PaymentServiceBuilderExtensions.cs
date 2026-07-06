using B.Unified.Payment.Abstract.Factory;

namespace B.Unified.Payment.Alipay.Extensions
{
    /// <summary>PaymentServiceBuilder 支付宝扩展</summary>
    public static class PaymentServiceBuilderExtensions
    {
        /// <summary>注册支付宝全部支付方式及查单/退款服务</summary>
        public static PaymentServiceBuilder AddAlipay(this PaymentServiceBuilder builder)
            => AlipayPaymentRegistration.AddTo(builder);
    }
}
