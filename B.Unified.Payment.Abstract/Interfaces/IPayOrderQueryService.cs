using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Models.Channel;
using B.Unified.Payment.Abstract.Models.Mch;

namespace B.Unified.Payment.Abstract.Interfaces
{
    /// <summary>
    /// 订单查询接口 — 所有支付通道必须实现。
    /// </summary>
    public interface IPayOrderQueryService
    {
        string GetIfCode();

        /// <summary>查询订单（异步）</summary>
        Task<ChannelRetMsg> QueryAsync(string payOrderId, MchAppConfigContext ctx);
    }
}