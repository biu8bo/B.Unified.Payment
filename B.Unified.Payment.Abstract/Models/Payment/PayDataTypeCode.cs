using System;
using Newtonsoft.Json;

namespace B.Unified.Payment.Abstract.Models.Payment
{
    /// <summary>
    /// 支付参数类型值类 
    /// </summary>
    [JsonConverter(typeof(PayDataTypeCodeJsonConverter))]
    public class PayDataTypeCode : IEquatable<PayDataTypeCode>
    {
        public static readonly PayDataTypeCode None = Of("none");
        public static readonly PayDataTypeCode PayUrl = Of("payurl");
        public static readonly PayDataTypeCode Form = Of("form");
        public static readonly PayDataTypeCode CodeUrl = Of("codeUrl");
        public static readonly PayDataTypeCode CodeImgUrl = Of("codeImgUrl");
        public static readonly PayDataTypeCode WxApp = Of("wxapp");
        public static readonly PayDataTypeCode AliApp = Of("aliapp");
        public static readonly PayDataTypeCode YsfApp = Of("ysfapp");

        /// <summary>类型代码字符串</summary>
        public string Code { get; }

        private PayDataTypeCode(string code)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        /// <summary>创建支付参数类型（用于已知常量或自定义值）</summary>
        public static PayDataTypeCode Of(string code) => new PayDataTypeCode(code);

        #region Equality

        public bool Equals(PayDataTypeCode other)
            => other != null && Code == other.Code;

        public override bool Equals(object obj)
            => obj is PayDataTypeCode other && Equals(other);

        public override int GetHashCode()
            => Code.GetHashCode();

        public override string ToString() => Code;

        public static bool operator ==(PayDataTypeCode a, PayDataTypeCode b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Code == b.Code;
        }

        public static bool operator !=(PayDataTypeCode a, PayDataTypeCode b) => !(a == b);

        #endregion

        #region 隐式转换（兼容 string 使用场景）

        public static implicit operator string(PayDataTypeCode type) => type?.Code;

        #endregion
    }

    /// <summary>Newtonsoft.Json 序列化/反序列化转换器</summary>
    internal class PayDataTypeCodeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(PayDataTypeCode);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var s = reader.Value?.ToString();
            return string.IsNullOrEmpty(s) ? null : PayDataTypeCode.Of(s);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((value as PayDataTypeCode)?.Code);
        }
    }
}
