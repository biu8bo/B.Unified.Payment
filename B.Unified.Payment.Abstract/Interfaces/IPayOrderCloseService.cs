using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Models.Channel;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Abstract.Interfaces
{
    /// <summary>
    /// 关单接口 — 所有支付通道必须实现。
    /// </summary>
    public interface IPayOrderCloseService
    {
        string GetIfCode();

        /// <summary>关闭订单（异步）</summary>
        Task<ChannelRetMsg> CloseAsync(CloseOrderRQ rq, MchAppConfigContext ctx);
    }

}
