using System.Threading.Tasks;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Alipay.PayWay
{
    /// <summary>支付宝 PayWay 处理器接口</summary>
    internal interface IAliPayWay
    {
        string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx);
        Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx);
    }
}