using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.YsfPay.Constants;
using B.Unified.Payment.YsfPay.Models;

namespace B.Unified.Payment.YsfPay
{
    /// <summary>云闪付配置获取工具</summary>
    internal static class YsfpayConfigHelper
    {
        /// <summary>从商户上下文获取云闪付配置</summary>
        public static YsfpayIsvParams GetConfig(MchAppConfigContext ctx)
        {
            var p = ctx.GetNormalMchParams<YsfpayIsvParams>(IfCode.YSFPAY)
                    ?? throw new BizException("未找到云闪付商户配置");
            return p;
        }
    }
}