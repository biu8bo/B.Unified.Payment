using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using B.Unified.Payment.Abstract.Models.Payment;

namespace B.Unified.Payment.Sample.WebApi.Json;

/// <summary>System.Text.Json 对 PayDataTypeCode 的序列化支持</summary>
public sealed class PayDataTypeCodeJsonConverter : JsonConverter<PayDataTypeCode>
{
    public override PayDataTypeCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var code = reader.GetString();
        return string.IsNullOrEmpty(code) ? null : PayDataTypeCode.Of(code);
    }

    public override void Write(Utf8JsonWriter writer, PayDataTypeCode value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Code);
    }
}
