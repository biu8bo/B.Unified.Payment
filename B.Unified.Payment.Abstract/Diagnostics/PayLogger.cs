using System;
using System.Diagnostics;
using B.Unified.Payment.Abstract.Models;
using Newtonsoft.Json;

namespace B.Unified.Payment.Abstract.Diagnostics
{
    /// <summary>
    /// SDK 内部日志 — 通过 System.Diagnostics.Trace 输出请求/响应摘要。
    /// <para>启用方法：在 app 启动时添加 Trace.Listeners.Add(new ConsoleTraceListener());</para>
    /// </summary>
    public static class PayLogger
    {
        private const string Prefix = "[B.Unified.Payment]";

        /// <summary>记录支付请求参数</summary>
        public static void LogRequest(string channel, string wayCode, string api, object body)
        {
            var json = JsonConvert.SerializeObject(body, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
            Trace.WriteLine($"{Prefix} [{channel}] >>> {wayCode} | {api}");
            Trace.WriteLine($"{Prefix} [{channel}] >>> REQ:\n{json}");
        }

        /// <summary>记录支付响应数据</summary>
        public static void LogResponse(string channel, string wayCode, object response, ChannelRetMsg ret)
        {
            var respJson = JsonConvert.SerializeObject(response, Formatting.Indented);
            Trace.WriteLine($"{Prefix} [{channel}] <<< {wayCode}");
            Trace.WriteLine($"{Prefix} [{channel}] <<< RESP:\n{respJson}");
            if (ret != null)
            {
                Trace.WriteLine($"{Prefix} [{channel}] <<< CHANNEL: state={ret.State} orderId={ret.ChannelOrderId} userId={ret.ChannelUserId} err={ret.ChannelErrCode}/{ret.ChannelErrMsg}");
            }
        }

        /// <summary>记录异常</summary>
        public static void LogError(string channel, string wayCode, Exception ex)
        {
            Trace.WriteLine($"{Prefix} [{channel}] !!! {wayCode} ERROR: {ex.Message}");
        }
    }
}