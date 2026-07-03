using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.PayWay;

namespace B.Unified.Payment.Weixin
{
    /// <summary>
    /// 微信支付通道入口 — 根据 WayCode 分发到对应的 PayWay 处理类。
    /// </summary>
    public class WeixinPaymentService : AbstractPaymentService
    {
        public override string GetIfCode() => IfCode.WXPAY;

        /// <summary>
        /// 判断是否支持该支付方式 — 微信前缀匹配
        /// </summary>
        public override bool IsSupport(string wayCode)
            => !string.IsNullOrEmpty(wayCode)
            && wayCode.StartsWith("WX_", System.StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// 公共参数校验 + 委托给具体 PayWay 做专项校验
        /// </summary>
        protected override string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (rq == null) return "请求参数不能为空";
            if (string.IsNullOrEmpty(rq.PayOrderId)) return "PayOrderId 不能为空";
            if (rq.Amount == null || rq.Amount <= 0) return "支付金额无效";
            if (string.IsNullOrEmpty(rq.Body)) return "商品描述不能为空";
            if (string.IsNullOrEmpty(rq.NotifyUrl)) return "NotifyUrl 不能为空";

            return GetHandler(rq.WayCode).PreCheck(rq, ctx);
        }

        /// <summary>
        /// 执行支付 — 路由到具体 PayWay
        /// </summary>
        protected override AbstractRS ExecutePay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            return GetHandler(rq.WayCode).Pay(rq, ctx);
        }

        /// <summary>
        /// 根据 WayCode 获取对应的 PayWay 处理器
        /// </summary>
        private IWxPayWay GetHandler(PayWayCode wc)
        {
            if (wc == WxPayWay.JSAPI)  return new WxJsapi();
            if (wc == WxPayWay.NATIVE) return new WxNative();
            if (wc == WxPayWay.H5)     return new WxH5();
            if (wc == WxPayWay.APP)    return new WxApp();
            if (wc == WxPayWay.LITE)   return new WxLite();
            if (wc == WxPayWay.BAR)    return new WxBar();

            throw new Abstract.Exceptions.BizException($"不支持的微信支付方式: {wc}");
        }
    }
}