using System;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Channel;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace B.Unified.Payment.Abstract.Diagnostics
{
    /// <summary>
    /// SDK 内部日志 — 基于 Microsoft.Extensions.Logging。
    /// <para>DI 模式下自动初始化（通过 PaymentServiceFactory 注入 ILoggerFactory）。</para>
    /// <para>非 DI 模式（直接 new）静默跳过，不输出日志。</para>
    /// </summary>
    public static class PayLogger
    {
        private static ILogger? _logger;
        private static readonly object _lock = new object();

        /// <summary>由 DI 自动调用，用户无需关心</summary>
        internal static void Initialize(ILoggerFactory factory)
        {
            if (_logger == null)
            {
                lock (_lock)
                {
                    _logger ??= factory.CreateLogger("B.Unified.Payment");
                }
            }
        }

        public static void LogRequest(string channel, string wayCode, string api, object body)
        {
            if (_logger == null) return;
            var json = JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            _logger.LogInformation("[{Channel}] >>> {WayCode} | {Api}\n{Body}", channel, wayCode, api, json);
        }

        public static void LogResponse(string channel, string wayCode, object response, ChannelRetMsg ret)
        {
            if (_logger == null) return;
            var respJson = JsonConvert.SerializeObject(response);
            _logger.LogInformation("[{Channel}] <<< {WayCode}\n{Resp}", channel, wayCode, respJson);
            if (ret != null)
            {
                _logger.LogInformation("[{Channel}] <<< CHANNEL: state={State} orderId={OrderId} err={ErrCode}/{ErrMsg}",
                    channel, ret.State, ret.ChannelOrderId, ret.ChannelErrCode, ret.ChannelErrMsg);
            }
        }

        public static void LogError(string channel, string wayCode, Exception ex)
        {
            _logger?.LogError(ex, "[{Channel}] !!! {WayCode} ERROR", channel, wayCode);
        }
    }
}