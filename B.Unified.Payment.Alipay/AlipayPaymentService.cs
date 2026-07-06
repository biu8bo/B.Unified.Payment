using B.Unified.Payment.Abstract;
using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Alipay.Constants;
using B.Unified.Payment.Alipay.PayWay;

namespace B.Unified.Payment.Alipay
{
    /// <summary>
    /// 支付宝支付通道入口 — 根据 WayCode 分发到对应的 PayWay 处理类。
    /// </summary>
    public class AlipayPaymentService : AbstractPaymentService
    {
        public override string GetIfCode() => IfCode.ALIPAY;

        /// <summary>
        /// 判断是否支持该支付方式 — 阿里系前缀匹配
        /// </summary>
        public override bool IsSupport(string wayCode)
            => !string.IsNullOrEmpty(wayCode)
            && wayCode.StartsWith("ALI", System.StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// 公共参数校验 + 委托给具体 PayWay 做专项校验
        /// </summary>
        protected override string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (rq == null) return "请求参数不能为空";
            if (string.IsNullOrEmpty(rq.PayOrderId)) return "PayOrderId 不能为空";
            if (rq.Amount == null || rq.Amount <= 0) return "支付金额无效";
            if (string.IsNullOrEmpty(rq.Subject)) return "商品标题不能为空";

            return GetHandler(rq.WayCode).PreCheck(rq, ctx);
        }

        /// <summary>
        /// 执行支付 — 路由到具体 PayWay
        /// </summary>
        protected override async Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            return await GetHandler(rq.WayCode).PayAsync(rq, ctx);
        }

        /// <summary>
        /// 根据 WayCode 获取对应的 PayWay 处理器
        /// </summary>
        private IAliPayWay GetHandler(PayWayCode wc)
        {
            if (wc == AlipayPayWay.BAR)   return new AliBar();
            if (wc == AlipayPayWay.PC)    return new AliPc();
            if (wc == AlipayPayWay.WAP)   return new AliWap();
            if (wc == AlipayPayWay.JSAPI) return new AliJsapi();
            if (wc == AlipayPayWay.APP)   return new AliApp();
            if (wc == AlipayPayWay.QR)    return new AliQr();
            if (wc == AlipayPayWay.LITE)  return new AliLite();
            if (wc == AlipayPayWay.OC)    return new AliOc();

            throw new Abstract.Exceptions.BizException($"不支持的支付宝支付方式: {wc}");
        }
    }
}