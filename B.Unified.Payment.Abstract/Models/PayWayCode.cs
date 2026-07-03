using System;
using Newtonsoft.Json;

namespace B.Unified.Payment.Abstract.Models
{
    /// <summary>
    /// 支付方式代码值类 — 强类型封装，防止字符串拼写错误。
    /// <para>各实现项目通过静态字段定义已知代码（如 AlipayPayWay.BAR / WxPayWay.JSAPI）。</para>
    /// <para>调用方可使用 PayWayCode.Of("自定义值") 创建自定义代码。</para>
    /// </summary>
    [JsonConverter(typeof(PayWayCodeJsonConverter))]
    public class PayWayCode : IEquatable<PayWayCode>
    {
        /// <summary>支付方式代码字符串</summary>
        public string Code { get; }

        private PayWayCode(string code)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        /// <summary>
        /// 创建支付方式代码（用于定义已知常量或自定义值）
        /// </summary>
        public static PayWayCode Of(string code) => new PayWayCode(code);

        #region Equality

        public bool Equals(PayWayCode other)
            => other != null && Code == other.Code;

        public override bool Equals(object obj)
            => obj is PayWayCode other && Equals(other);

        public override int GetHashCode()
            => Code.GetHashCode();

        public override string ToString() => Code;

        public static bool operator ==(PayWayCode a, PayWayCode b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Code == b.Code;
        }

        public static bool operator !=(PayWayCode a, PayWayCode b) => !(a == b);

        #endregion

        #region 隐式转换（兼容 string 使用场景）

        public static implicit operator string(PayWayCode wc) => wc?.Code;

        #endregion
    }

    /// <summary>
    /// Newtonsoft.Json 序列化/反序列化转换器
    /// </summary>
    internal class PayWayCodeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(PayWayCode);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var s = reader.Value?.ToString();
            return string.IsNullOrEmpty(s) ? null : PayWayCode.Of(s);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((value as PayWayCode)?.Code);
        }
    }
}