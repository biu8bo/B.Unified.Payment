using System.Collections.Generic;

namespace B.Unified.Payment.Abstract.Models
{
    /// <summary>
    /// 渠道响应信息包装类
    /// </summary>
    public class ChannelRetMsg
    {
        /// <summary>渠道状态</summary>
        public ChannelState State { get; set; }

        /// <summary>渠道订单号</summary>
        public string ChannelOrderId { get; set; }

        /// <summary>渠道用户标识</summary>
        public string ChannelUserId { get; set; }

        /// <summary>渠道错误码</summary>
        public string ChannelErrCode { get; set; }

        /// <summary>渠道错误描述</summary>
        public string ChannelErrMsg { get; set; }

        /// <summary>渠道支付数据包（用于继续支付操作）</summary>
        public string ChannelAttach { get; set; }

        /// <summary>渠道返回的原始报文</summary>
        public string ChannelOriginResponse { get; set; }

        /// <summary>是否需要轮询查单，默认 false</summary>
        public bool IsNeedQuery { get; set; }

        public enum ChannelState
        {
            CONFIRM_SUCCESS,  // 业务已明确成功
            CONFIRM_FAIL,     // 业务已明确失败
            WAITING,          // 上游处理中，需定时查询/回调
            UNKNOWN,          // 状态不明确（网络异常等）
            API_RET_ERROR,    // 渠道侧异常
            SYS_ERROR         // 本系统不可预知异常
        }

        // 静态工厂方法
        public static ChannelRetMsg ConfirmSuccess(string channelOrderId = null) =>
            new ChannelRetMsg { State = ChannelState.CONFIRM_SUCCESS, ChannelOrderId = channelOrderId };

        public static ChannelRetMsg ConfirmFail(string channelErrCode = null, string channelErrMsg = null) =>
            new ChannelRetMsg { State = ChannelState.CONFIRM_FAIL, ChannelErrCode = channelErrCode, ChannelErrMsg = channelErrMsg };

        public static ChannelRetMsg Waiting(string channelOrderId = null) =>
            new ChannelRetMsg { State = ChannelState.WAITING, ChannelOrderId = channelOrderId };

        public static ChannelRetMsg SysError(string channelErrMsg) =>
            new ChannelRetMsg { State = ChannelState.SYS_ERROR, ChannelErrMsg = "系统：" + channelErrMsg };

        public static ChannelRetMsg Unknown(string channelErrMsg = null) =>
            new ChannelRetMsg { State = ChannelState.UNKNOWN, ChannelErrMsg = channelErrMsg };
    }
}