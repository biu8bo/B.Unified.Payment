using System;
using System.Collections.Generic;
using System.Linq;
using B.Unified.Payment.Abstract.Diagnostics;
using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Abstract.Models;
using Microsoft.Extensions.Logging;

namespace B.Unified.Payment.Abstract.Factory
{
    /// <summary>
    /// 支付服务工厂 — 根据 ifCode + wayCode 获取对应的支付/查单/退款服务。
    /// </summary>
    public interface IPaymentServiceFactory
    {
        /// <summary>按渠道 + 支付方式获取支付服务</summary>
        IPaymentService GetPaymentService(string ifCode, string wayCode);

        /// <summary>按支付方式获取支付服务（在已注册实现中匹配 IsSupport）</summary>
        IPaymentService GetPaymentService(PayWayCode wayCode);

        IPayOrderQueryService GetQueryService(string ifCode);
        IRefundService GetRefundService(string ifCode);
    }

    public class PaymentServiceFactory : IPaymentServiceFactory
    {
        private readonly IReadOnlyList<Func<IPaymentService>> _paymentFactories;
        private readonly IReadOnlyList<Func<IPayOrderQueryService>> _queryFactories;
        private readonly IReadOnlyList<Func<IRefundService>> _refundFactories;

        /// <summary>DI 容器注入（MS.DI 解析 IEnumerable&lt;IPaymentService&gt; 时使用）</summary>
        public PaymentServiceFactory(
            IEnumerable<IPaymentService> paymentServices,
            IEnumerable<IPayOrderQueryService> queryServices,
            IEnumerable<IRefundService> refundServices,
            ILoggerFactory loggerFactory)
            : this(
                (paymentServices ?? Array.Empty<IPaymentService>()).Select(s => (Func<IPaymentService>)(() => s)),
                (queryServices ?? Array.Empty<IPayOrderQueryService>()).Select(s => (Func<IPayOrderQueryService>)(() => s)),
                (refundServices ?? Array.Empty<IRefundService>()).Select(s => (Func<IRefundService>)(() => s)),
                loggerFactory)
        {
        }

        internal PaymentServiceFactory(
            IEnumerable<Func<IPaymentService>> paymentFactories,
            IEnumerable<Func<IPayOrderQueryService>> queryFactories,
            IEnumerable<Func<IRefundService>> refundFactories,
            ILoggerFactory loggerFactory)
        {
            _paymentFactories = (paymentFactories ?? Array.Empty<Func<IPaymentService>>()).ToList();
            _queryFactories = (queryFactories ?? Array.Empty<Func<IPayOrderQueryService>>()).ToList();
            _refundFactories = (refundFactories ?? Array.Empty<Func<IRefundService>>()).ToList();
            PayLogger.Initialize(loggerFactory);
        }

        public IPaymentService GetPaymentService(string ifCode, string wayCode)
        {
            if (string.IsNullOrEmpty(ifCode))
                throw new ArgumentException("ifCode 不能为空", nameof(ifCode));
            if (string.IsNullOrEmpty(wayCode))
                throw new ArgumentException("wayCode 不能为空", nameof(wayCode));

            return ResolvePaymentService(s => s.GetIfCode() == ifCode && s.IsSupport(wayCode))
                ?? throw new InvalidOperationException($"未注册 ifCode='{ifCode}' wayCode='{wayCode}' 的 IPaymentService");
        }

        public IPaymentService GetPaymentService(PayWayCode wayCode)
        {
            if (wayCode == null)
                throw new ArgumentNullException(nameof(wayCode));

            return ResolvePaymentService(s => s.IsSupport(wayCode))
                ?? throw new InvalidOperationException($"未注册 wayCode='{wayCode}' 的 IPaymentService");
        }

        public IPayOrderQueryService GetQueryService(string ifCode)
        {
            foreach (var create in _queryFactories)
            {
                var service = create();
                if (service.GetIfCode() == ifCode)
                    return service;
            }
            throw new InvalidOperationException($"未注册 ifCode='{ifCode}' 的 IPayOrderQueryService");
        }

        public IRefundService GetRefundService(string ifCode)
        {
            foreach (var create in _refundFactories)
            {
                var service = create();
                if (service.GetIfCode() == ifCode)
                    return service;
            }
            throw new InvalidOperationException($"未注册 ifCode='{ifCode}' 的 IRefundService");
        }

        private IPaymentService ResolvePaymentService(Func<IPaymentService, bool> predicate)
        {
            foreach (var create in _paymentFactories)
            {
                var service = create();
                if (predicate(service))
                    return service;
            }
            return null;
        }
    }
}
