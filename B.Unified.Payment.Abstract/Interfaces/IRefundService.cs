using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Models.Channel;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Abstract.Models.Refund;

namespace B.Unified.Payment.Abstract.Interfaces
{
    /// <summary>
    /// 退款接口 — 所有支付通道必须实现。
    /// </summary>
    public interface IRefundService
    {
        string GetIfCode();

        string PreCheck(RefundOrderRQ bizRQ, MchAppConfigContext ctx);

        /// <summary>发起退款（异步）</summary>
        Task<ChannelRetMsg> RefundAsync(RefundOrderRQ bizRQ, MchAppConfigContext ctx);

        /// <summary>退款查单（异步）</summary>
        Task<ChannelRetMsg> QueryAsync(string refundOrderId, string payOrderId, string channelOrderNo, MchAppConfigContext ctx);
    }
}