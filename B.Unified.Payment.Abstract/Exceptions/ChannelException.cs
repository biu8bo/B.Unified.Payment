using System;
using B.Unified.Payment.Abstract.Models;

namespace B.Unified.Payment.Abstract.Exceptions
{
    /// <summary>
    /// 支付通道异常 — 携带 ChannelRetMsg，区分"未知状态"和"系统异常"
    /// </summary>
    public class ChannelException : Exception
    {
        public ChannelRetMsg ChannelRetMsg { get; }

        private ChannelException(string message, ChannelRetMsg channelRetMsg)
            : base(message ?? channelRetMsg?.ChannelErrMsg)
        {
            ChannelRetMsg = channelRetMsg;
        }

        /// <summary>未知状态（接口超时/网络异常等）</summary>
        public static ChannelException Unknown(string channelErrMsg)
        {
            return new ChannelException(channelErrMsg, ChannelRetMsg.Unknown(channelErrMsg));
        }

        /// <summary>系统内部异常</summary>
        public static ChannelException SysError(string channelErrMsg)
        {
            return new ChannelException(channelErrMsg, ChannelRetMsg.SysError(channelErrMsg));
        }
    }
}