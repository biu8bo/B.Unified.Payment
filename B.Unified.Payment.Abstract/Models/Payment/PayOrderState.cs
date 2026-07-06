namespace B.Unified.Payment.Abstract.Models.Payment
{
    /// <summary>
    /// 支付订单状态
    /// </summary>
    public enum PayOrderState : byte
    {
        /// <summary>订单生成</summary>
        Init = 0,

        /// <summary>支付中</summary>
        Paying = 1,

        /// <summary>支付成功</summary>
        Success = 2,

        /// <summary>支付失败</summary>
        Failed = 3,

        /// <summary>已撤销</summary>
        Revoked = 4,

        /// <summary>已退款</summary>
        Refunded = 5,

        /// <summary>订单关闭</summary>
        Closed = 6
    }
}
