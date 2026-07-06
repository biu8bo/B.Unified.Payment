using Newtonsoft.Json;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Base;
using B.Unified.Payment.Abstract.Models.Channel;

namespace B.Unified.Payment.Abstract.Models.Payment
{
    /// <summary>
    /// 统一支付响应基类
    /// </summary>
    public class UnifiedOrderRS : AbstractRS
    {
        /// <summary>支付订单号</summary>
        public string PayOrderId { get; set; }

        /// <summary>商户订单号</summary>
        public string MchOrderNo { get; set; }

        /// <summary>订单状态（0-订单生成 1-支付中 2-支付成功 3-支付失败 4-已撤销 5-已退款 6-订单关闭）</summary>
        public PayOrderState? OrderState { get; set; }

        /// <summary>支付参数类型</summary>
        public PayDataTypeCode PayDataType { get; set; }

        /// <summary>支付参数（根据 PayDataType 解析，如跳转URL / 表单HTML / 调起参数JSON）</summary>
        public string PayData { get; set; }

        /// <summary>错误码</summary>
        public string ErrCode { get; set; }

        /// <summary>错误信息</summary>
        public string ErrMsg { get; set; }

        /// <summary>上游渠道响应</summary> 
        public ChannelRetMsg ChannelRetMsg { get; set; }

        /// <summary>生成 PayDataType（子类覆写）</summary>
        public virtual PayDataTypeCode BuildPayDataType() => PayDataTypeCode.None;

        /// <summary>生成 PayData（子类覆写）</summary>
        public virtual string BuildPayData() => "";

        /// <summary>
        /// 填充对外响应字段。
        /// <para>失败时填充 ErrCode/ErrMsg；Init/Paying/Success 或未设置状态时填充 PayDataType/PayData。</para>
        /// </summary>
        public void FinalizePayData()
        {
            if (OrderState == PayOrderState.Failed)
            {
                if (string.IsNullOrEmpty(ErrCode))
                    ErrCode = ChannelRetMsg?.ChannelErrCode;
                if (string.IsNullOrEmpty(ErrMsg))
                    ErrMsg = ChannelRetMsg?.ChannelErrMsg;
                return;
            }

            if (OrderState == null
                || OrderState == PayOrderState.Init
                || OrderState == PayOrderState.Paying
                || OrderState == PayOrderState.Success)
            {
                PayDataType = BuildPayDataType();
                PayData = BuildPayData();
            }
        }
    }
}
