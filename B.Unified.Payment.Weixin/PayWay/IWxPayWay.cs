using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>
    /// 微信支付 PayWay 处理器接口
    /// </summary>
    internal interface IWxPayWay
    {
        /// <summary>前置校验，返回错误描述（null/空 表示通过）</summary>
        string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx);

        /// <summary>执行支付</summary>
        AbstractRS Pay(UnifiedOrderRQ rq, MchAppConfigContext ctx);
    }
}