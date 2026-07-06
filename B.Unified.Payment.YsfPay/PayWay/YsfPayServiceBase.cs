using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Abstract.Services;
using B.Unified.Payment.YsfPay.Constants;

namespace B.Unified.Payment.YsfPay.PayWay
{
    /// <summary>云闪付支付服务基类 — 公共参数校验</summary>
    public abstract class YsfPayServiceBase : AbstractPaymentService
    {
        public override string GetIfCode() => IfCode.YSFPAY;

        protected override string ValidateCommon(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            if (rq == null) return "请求参数不能为空";
            if (string.IsNullOrEmpty(rq.PayOrderId)) return "PayOrderId 不能为空";
            if (rq.Amount == null || rq.Amount <= 0) return "支付金额无效";
            if (string.IsNullOrEmpty(rq.Body)) return "商品描述不能为空";
            return null;
        }
    }
}
