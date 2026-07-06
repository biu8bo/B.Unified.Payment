using B.Unified.Payment.Abstract.Models;

namespace B.Unified.Payment.Abstract
{
    /// <summary>
    /// 订单查询接口 — 所有支付通道必须实现。
    /// <para>通过商户订单号查询渠道侧订单状态。</para>
    /// </summary>
    public interface IPayOrderQueryService
    {
        /// <summary>获取支付接口代码（alipay / wxpay 等）</summary>
        string GetIfCode();

        /// <summary>
        /// 查询订单 — 传入商户订单号，返回标准化的渠道状态消息。
        /// <para>状态映射规则（子类覆写）：SUCCESS → CONFIRM_SUCCESS；USERPAYING/WAIT_BUYER_PAY → WAITING；CLOSED/REVOKED/PAYERROR → CONFIRM_FAIL；其他 → UNKNOWN</para>
        /// </summary>
        /// <param name="payOrderId">商户订单号（对应渠道 out_trade_no）</param>
        /// <param name="ctx">商户配置上下文</param>
        /// <returns>标准化的渠道状态消息</returns>
        ChannelRetMsg Query(string payOrderId, MchAppConfigContext ctx);
    }
}