using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.YsfPay.Constants;
using B.Unified.Payment.YsfPay.Models.MchParams;

namespace B.Unified.Payment.YsfPay.Utils
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