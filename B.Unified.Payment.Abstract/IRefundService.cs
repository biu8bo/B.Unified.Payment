using B.Unified.Payment.Abstract.Models;

namespace B.Unified.Payment.Abstract
{
    /// <summary>
    /// 退款接口 — 所有支付通道必须实现。
    /// <para>包含发起退款和退款查单两个核心能力。</para>
    /// </summary>
    public interface IRefundService
    {
        /// <summary>获取支付接口代码（alipay / wxpay 等）</summary>
        string GetIfCode();

        /// <summary>
        /// 前置校验 — 校验退款参数是否合法。
        /// <para>返回 null/空字符串 表示通过，否则返回错误描述。</para>
        /// </summary>
        string PreCheck(Models.Refund.RefundOrderRQ bizRQ, MchAppConfigContext ctx);

        /// <summary>
        /// 发起退款 — 调用渠道退款 API。
        /// <para>返回标准化的渠道状态消息。</para>
        /// </summary>
        /// <param name="bizRQ">退款请求参数</param>
        /// <param name="ctx">商户配置上下文</param>
        /// <returns>渠道状态</returns>
        ChannelRetMsg Refund(Models.Refund.RefundOrderRQ bizRQ, MchAppConfigContext ctx);

        /// <summary>
        /// 退款查单 — 查询退款订单的最终状态。
        /// </summary>
        /// <param name="refundOrderId">退款订单号（商户侧唯一）</param>
        /// <param name="payOrderId">原支付订单号</param>
        /// <param name="channelOrderNo">渠道侧支付订单号</param>
        /// <param name="ctx">商户配置上下文</param>
        /// <returns>渠道状态</returns>
        ChannelRetMsg Query(string refundOrderId, string payOrderId, string channelOrderNo, MchAppConfigContext ctx);
    }
}