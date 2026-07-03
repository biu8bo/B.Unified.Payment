using B.Unified.Payment.Abstract.Models;

namespace B.Unified.Payment.Alipay.Constants
{
    /// <summary>
    /// 支付宝支付方式代码常量 — 通过 PayWayCode 强类型引用，避免字符串硬编码
    /// </summary>
    public static class AlipayPayWay
    {
        /// <summary>支付宝条码支付（用户出示付款码）</summary>
        public static readonly PayWayCode BAR   = PayWayCode.Of("ALI_BAR");

        /// <summary>支付宝 PC 网站支付</summary>
        public static readonly PayWayCode PC    = PayWayCode.Of("ALI_PC");

        /// <summary>支付宝手机网站支付</summary>
        public static readonly PayWayCode WAP   = PayWayCode.Of("ALI_WAP");

        /// <summary>支付宝生活号 / JSAPI 支付</summary>
        public static readonly PayWayCode JSAPI = PayWayCode.Of("ALI_JSAPI");

        /// <summary>支付宝 APP 支付</summary>
        public static readonly PayWayCode APP   = PayWayCode.Of("ALI_APP");

        /// <summary>支付宝扫码支付（正扫）</summary>
        public static readonly PayWayCode QR    = PayWayCode.Of("ALI_QR");

        /// <summary>支付宝小程序支付</summary>
        public static readonly PayWayCode LITE  = PayWayCode.Of("ALI_LITE");

        /// <summary>支付宝订单码支付（反扫）</summary>
        public static readonly PayWayCode OC    = PayWayCode.Of("ALI_OC");
    }
}