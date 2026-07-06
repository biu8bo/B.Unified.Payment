using B.Unified.Payment.Abstract.Models;

namespace B.Unified.Payment.YsfPay.Constants
{
    /// <summary>云闪付支付方式代码常量</summary>
    public static class YsfPayWay
    {
        public static readonly PayWayCode BAR   = PayWayCode.Of("YSF_BAR");
        public static readonly PayWayCode JSAPI = PayWayCode.Of("YSF_JSAPI");
    }
}
