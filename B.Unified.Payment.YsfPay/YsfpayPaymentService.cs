using System;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.YsfPay.PayWay;

namespace B.Unified.Payment.YsfPay
{
    /// <summary>
    /// 云闪付支付通道入口 — 支持条码支付（YSF_BAR）和 JSAPI 支付（YSF_JSAPI）
    /// </summary>
    public class YsfpayPaymentService : AbstractPaymentService
    {
        public override string GetIfCode() => Constants.IfCode.YSFPAY;

        public override bool IsSupport(string wayCode)
            => !string.IsNullOrEmpty(wayCode) && wayCode.StartsWith("YSF_", StringComparison.OrdinalIgnoreCase);

        protected override string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (rq == null) return "请求参数不能为空";
            if (string.IsNullOrEmpty(rq.PayOrderId)) return "PayOrderId 不能为空";
            if (rq.Amount == null || rq.Amount <= 0) return "支付金额无效";
            if (string.IsNullOrEmpty(rq.Body)) return "商品描述不能为空";

            return GetHandler(rq.WayCode).PreCheck(rq, ctx);
        }

        protected override AbstractRS ExecutePay(UnifiedOrderRQ rq, MchAppConfigContext ctx)
            => GetHandler(rq.WayCode).Pay(rq, ctx);

        private IYsfPayWay GetHandler(PayWayCode wc)
        {
            if (wc?.Code == "YSF_BAR") return new YsfBar();
            if (wc?.Code == "YSF_JSAPI") return new YsfJsapi();
            throw new Abstract.Exceptions.BizException($"不支持的云闪付支付方式: {wc}");
        }
    }
}