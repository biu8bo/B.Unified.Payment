namespace B.Unified.Payment.Abstract.Models.Channel
{
    /// <summary>
    /// 渠道响应状态 — 各支付渠道统一映射到此枚举
    /// </summary>
    public enum ChannelState
    {
        CONFIRM_SUCCESS,  // 业务已明确成功
        CONFIRM_FAIL,     // 业务已明确失败
        WAITING,          // 上游处理中，需定时查询/回调
        UNKNOWN,          // 状态不明确（网络异常等）
        API_RET_ERROR,    // 渠道侧异常
        SYS_ERROR         // 本系统不可预知异常
    }
}
