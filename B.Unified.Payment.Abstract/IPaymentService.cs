using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Abstract
{
    /// <summary>
    /// 统一支付接口 — 所有支付通道必须实现
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>获取支付接口代码（alipay / wxpay / ysfpay 等）</summary>
        string GetIfCode();

        /// <summary>是否支持该支付方式（如 WX_JSAPI / ALI_BAR）</summary>
        bool IsSupport(string wayCode);

        /// <summary>执行支付</summary>
        AbstractRS Pay(UnifiedOrderRQ bizRQ, MchAppConfigContext mchAppConfigContext);
    }
}