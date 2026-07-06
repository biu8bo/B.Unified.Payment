namespace B.Unified.Payment.Abstract.Models.Payment
{
    /// <summary>
    /// 通用支付数据响应 — 根据已填充的字段自动推导 PayDataType
    /// </summary>
    public class CommonPayDataRS : UnifiedOrderRS
    {
        /// <summary>跳转地址</summary>
        public string PayUrl { get; set; }

        /// <summary>二维码链接</summary>
        public string CodeUrl { get; set; }

        /// <summary>二维码图片地址</summary>
        public string CodeImgUrl { get; set; }

        /// <summary>表单HTML内容</summary>
        public string FormContent { get; set; }

        public override PayDataTypeCode BuildPayDataType()
        {
            if (!string.IsNullOrEmpty(PayUrl)) return PayDataTypeCode.PayUrl;
            if (!string.IsNullOrEmpty(CodeUrl)) return PayDataTypeCode.CodeUrl;
            if (!string.IsNullOrEmpty(CodeImgUrl)) return PayDataTypeCode.CodeImgUrl;
            if (!string.IsNullOrEmpty(FormContent)) return PayDataTypeCode.Form;
            return PayDataTypeCode.PayUrl;
        }

        public override string BuildPayData()
        {
            if (!string.IsNullOrEmpty(PayUrl)) return PayUrl;
            if (!string.IsNullOrEmpty(CodeUrl)) return CodeUrl;
            if (!string.IsNullOrEmpty(CodeImgUrl)) return CodeImgUrl;
            if (!string.IsNullOrEmpty(FormContent)) return FormContent;
            return "";
        }
    }
}