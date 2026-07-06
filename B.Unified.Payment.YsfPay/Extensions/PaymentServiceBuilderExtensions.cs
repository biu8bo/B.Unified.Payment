using B.Unified.Payment.Abstract;

namespace B.Unified.Payment.YsfPay
{
    /// <summary>PaymentServiceBuilder 云闪付扩展</summary>
    public static class PaymentServiceBuilderExtensions
    {
        /// <summary>注册云闪付全部支付方式及查单/退款服务</summary>
        public static PaymentServiceBuilder AddYsfPay(this PaymentServiceBuilder builder)
            => YsfPaymentRegistration.AddTo(builder);
    }
}
