using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Abstract
{
    /// <summary>
    /// 支付服务抽象基类 — 模板方法：ValidateCommon → PreCheckWay → ExecutePayAsync → FinalizePayData。
    /// <para>子类实现渠道公共校验、支付方式专项校验及支付执行逻辑。</para>
    /// </summary>
    public abstract class AbstractPaymentService : IPaymentService
    {
        #region IPaymentService 抽象实现

        public abstract string GetIfCode();
        public abstract bool IsSupport(string wayCode);

        /// <summary>模板方法 — 先校验再支付</summary>
        public virtual async Task<AbstractRS> PayAsync(UnifiedOrderRQ bizRQ, MchAppConfigContext ctx)
        {
            try
            {
                var err = PreCheck(bizRQ, ctx);
                if (!string.IsNullOrEmpty(err))
                {
                    return Finalize(new UnifiedOrderRS
                    {
                        PayOrderId = bizRQ?.PayOrderId,
                        MchOrderNo = bizRQ?.MchOrderNo,
                        ErrCode = "PRECHECK_FAIL",
                        ErrMsg = err,
                        ChannelRetMsg = ChannelRetMsg.ConfirmFail("PRECHECK_FAIL", err)
                    });
                }

                return Finalize(await ExecutePayAsync(bizRQ, ctx));
            }
            catch (Exceptions.BizException ex)
            {
                return Finalize(new UnifiedOrderRS
                {
                    PayOrderId = bizRQ?.PayOrderId,
                    MchOrderNo = bizRQ?.MchOrderNo,
                    ErrCode = "BIZ_ERROR",
                    ErrMsg = ex.Message,
                    ChannelRetMsg = ChannelRetMsg.ConfirmFail("BIZ_ERROR", ex.Message)
                });
            }
            catch (Exceptions.ChannelException ex)
            {
                return Finalize(new UnifiedOrderRS
                {
                    PayOrderId = bizRQ?.PayOrderId,
                    MchOrderNo = bizRQ?.MchOrderNo,
                    ErrCode = "CHANNEL_ERROR",
                    ErrMsg = ex.Message,
                    ChannelRetMsg = ex.ChannelRetMsg
                });
            }
            catch (System.Exception ex)
            {
                return Finalize(new UnifiedOrderRS
                {
                    PayOrderId = bizRQ?.PayOrderId,
                    MchOrderNo = bizRQ?.MchOrderNo,
                    ErrCode = "SYS_ERROR",
                    ErrMsg = ex.Message,
                    ChannelRetMsg = ChannelRetMsg.SysError(ex.Message)
                });
            }
        }

        #endregion

        #region 子类实现

        /// <summary>渠道公共参数校验</summary>
        protected virtual string ValidateCommon(UnifiedOrderRQ rq, MchAppConfigContext ctx) => null;

        /// <summary>支付方式专项校验</summary>
        protected abstract string PreCheckWay(UnifiedOrderRQ rq, MchAppConfigContext ctx);

        /// <summary>执行支付</summary>
        protected abstract Task<AbstractRS> ExecutePayAsync(UnifiedOrderRQ rq, MchAppConfigContext ctx);

        #endregion

        private string PreCheck(UnifiedOrderRQ rq, MchAppConfigContext ctx)
        {
            var err = ValidateCommon(rq, ctx);
            if (!string.IsNullOrEmpty(err)) return err;
            return PreCheckWay(rq, ctx);
        }

        private static AbstractRS Finalize(AbstractRS rs)
        {
            if (rs is UnifiedOrderRS orderRs)
                orderRs.FinalizePayData();
            return rs;
        }
    }
}
