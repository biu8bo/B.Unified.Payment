using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using B.Unified.Payment.Abstract.Models;

namespace B.Unified.Payment.Sample.WebApi.Json;

/// <summary>System.Text.Json 对 PayWayCode 的序列化支持</summary>
public sealed class PayWayCodeJsonConverter : JsonConverter<PayWayCode>
{
    public override PayWayCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var code = reader.GetString();
        return string.IsNullOrEmpty(code) ? null : PayWayCode.Of(code);
    }

    public override void Write(Utf8JsonWriter writer, PayWayCode value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Code);
    }
}
