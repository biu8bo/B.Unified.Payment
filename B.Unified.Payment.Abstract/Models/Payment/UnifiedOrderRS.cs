using Newtonsoft.Json;

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

        /// <summary>订单状态</summary>
        public byte? OrderState { get; set; }

        /// <summary>支付参数类型 ( NONE / payurl / form / codeUrl / codeImgUrl / wxapp / aliapp )</summary>
        public string PayDataType { get; set; }

        /// <summary>支付参数（根据 PayDataType 解析，如跳转URL / 表单HTML / 调起参数JSON）</summary>
        public string PayData { get; set; }

        /// <summary>错误码</summary>
        public string ErrCode { get; set; }

        /// <summary>错误信息</summary>
        public string ErrMsg { get; set; }

        /// <summary>上游渠道响应（不参与 JSON 序列化输出给客户端）</summary>
 
        public ChannelRetMsg ChannelRetMsg { get; set; }

        /// <summary>生成 PayDataType（子类覆写）</summary>
        public virtual string BuildPayDataType() => "none";

        /// <summary>生成 PayData（子类覆写）</summary>
        public virtual string BuildPayData() => "";
    }
}