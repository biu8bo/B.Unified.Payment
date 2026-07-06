using System.Threading.Tasks;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.YsfPay.PayWay
{
    /// <summary>云闪付 PayWay 处理器接口</summary>
    internal interface IYsfPayWay
    {
        string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx);
        Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx);
    }
}