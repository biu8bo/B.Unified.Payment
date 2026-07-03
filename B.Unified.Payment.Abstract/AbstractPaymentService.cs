using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Abstract
{
    /// <summary>
    /// 支付服务抽象基类 — 模板方法模式：Pay() 先执行 PreCheck() 再执行 ExecutePay()。
    /// <para>子类只需实现 GetIfCode / IsSupport / ExecutePay，按需覆写 PreCheck。</para>
    /// </summary>
    public abstract class AbstractPaymentService : IPaymentService
    {
        #region IPaymentService 抽象实现

        public abstract string GetIfCode();
        public abstract bool IsSupport(string wayCode);

        /// <summary>
        /// 模板方法 — 先校验再支付
        /// </summary>
        public virtual AbstractRS Pay(UnifiedOrderRQ bizRQ, MchAppConfigContext ctx)
        {
            try
            {
                var err = PreCheck(bizRQ, ctx);
                if (!string.IsNullOrEmpty(err))
                {
                    return new UnifiedOrderRS
                    {
                        PayOrderId = bizRQ?.PayOrderId,
                        MchOrderNo = bizRQ?.MchOrderNo,
                        ErrCode = "PRECHECK_FAIL",
                        ErrMsg = err,
                        ChannelRetMsg = ChannelRetMsg.ConfirmFail("PRECHECK_FAIL", err)
                    };
                }

                return ExecutePay(bizRQ, ctx);
            }
            catch (Exceptions.BizException ex)
            {
                return new UnifiedOrderRS
                {
                    PayOrderId = bizRQ?.PayOrderId,
                    MchOrderNo = bizRQ?.MchOrderNo,
                    ErrCode = "BIZ_ERROR",
                    ErrMsg = ex.Message,
                    ChannelRetMsg = ChannelRetMsg.ConfirmFail("BIZ_ERROR", ex.Message)
                };
            }
            catch (Exceptions.ChannelException ex)
            {
                return new UnifiedOrderRS
                {
                    PayOrderId = bizRQ?.PayOrderId,
                    MchOrderNo = bizRQ?.MchOrderNo,
                    ErrCode = "CHANNEL_ERROR",
                    ErrMsg = ex.Message,
                    ChannelRetMsg = ex.ChannelRetMsg
                };
            }
            catch (System.Exception ex)
            {
                return new UnifiedOrderRS
                {
                    PayOrderId = bizRQ?.PayOrderId,
                    MchOrderNo = bizRQ?.MchOrderNo,
                    ErrCode = "SYS_ERROR",
                    ErrMsg = ex.Message,
                    ChannelRetMsg = ChannelRetMsg.SysError(ex.Message)
                };
            }
        }

        #endregion

        #region 子类可覆写

        /// <summary>前置参数校验，返回错误描述（null/空表示通过）</summary>
        protected virtual string PreCheck(UnifiedOrderRQ bizRQ, MchAppConfigContext ctx) => null;

        /// <summary>执行支付（子类必须实现）</summary>
        protected abstract AbstractRS ExecutePay(UnifiedOrderRQ bizRQ, MchAppConfigContext ctx);

        #endregion

        #region 公用工具

        /// <summary>判断是否需要分账</summary>
        protected virtual bool IsDivisionOrder(UnifiedOrderRQ bizRQ)
        {
            if (bizRQ?.DivisionMode != null)
            {
                var mode = bizRQ.DivisionMode.Value;
                return mode == 1 || mode == 2;
            }
            return false;
        }

        #endregion
    }
}