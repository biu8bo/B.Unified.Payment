using B.Unified.Payment.Abstract.Models;

namespace B.Unified.Payment.Weixin.Constants
{
    /// <summary>
    /// 微信支付方式代码常量 — 通过 PayWayCode 强类型引用，避免字符串硬编码
    /// </summary>
    public static class WxPayWay
    {
        /// <summary>微信公众号 / JSAPI 支付</summary>
        public static readonly PayWayCode JSAPI  = PayWayCode.Of("WX_JSAPI");

        /// <summary>微信小程序支付</summary>
        public static readonly PayWayCode LITE   = PayWayCode.Of("WX_LITE");

        /// <summary>微信付款码支付（被扫）</summary>
        public static readonly PayWayCode BAR    = PayWayCode.Of("WX_BAR");

        /// <summary>微信 H5 支付</summary>
        public static readonly PayWayCode H5     = PayWayCode.Of("WX_H5");

        /// <summary>微信 Native 扫码支付</summary>
        public static readonly PayWayCode NATIVE = PayWayCode.Of("WX_NATIVE");

        /// <summary>微信 APP 支付</summary>
        public static readonly PayWayCode APP    = PayWayCode.Of("WX_APP");
    }
}