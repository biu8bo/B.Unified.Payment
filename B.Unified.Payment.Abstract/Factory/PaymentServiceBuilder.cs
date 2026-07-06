using System;
using System.Collections.Generic;
using B.Unified.Payment.Abstract.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace B.Unified.Payment.Abstract.Factory
{
    /// <summary>
    /// 支付服务构建器 — 非 DI 场景下一行注册渠道，支持扩展自定义 PayWay。
    /// <para>示例: PaymentServiceBuilder.Create().AddPaymentService&lt;MyPay&gt;().Build()</para>
    /// </summary>
    public sealed class PaymentServiceBuilder
    {
        private readonly List<Func<IPaymentService>> _paymentFactories = new List<Func<IPaymentService>>();
        private readonly List<Func<IPayOrderQueryService>> _queryFactories = new List<Func<IPayOrderQueryService>>();
        private readonly List<Func<IRefundService>> _refundFactories = new List<Func<IRefundService>>();
        private readonly List<Func<IPayOrderCloseService>> _closeFactories = new List<Func<IPayOrderCloseService>>();
        private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;

        public static PaymentServiceBuilder Create() => new PaymentServiceBuilder();

        /// <summary>配置日志（默认 NullLogger）</summary>
        public PaymentServiceBuilder WithLogger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            return this;
        }

        /// <summary>注册一个支付方式实现（无参构造）</summary>
        public PaymentServiceBuilder AddPaymentService<T>() where T : class, IPaymentService, new()
        {
            _paymentFactories.Add(() => new T());
            return this;
        }

        /// <summary>注册一个支付方式实现（自定义工厂，可用于带参构造或单例）</summary>
        public PaymentServiceBuilder AddPaymentService(Func<IPaymentService> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            _paymentFactories.Add(factory);
            return this;
        }

        /// <summary>注册查单服务</summary>
        public PaymentServiceBuilder AddQueryService<T>() where T : class, IPayOrderQueryService, new()
        {
            _queryFactories.Add(() => new T());
            return this;
        }

        /// <summary>注册退款服务</summary>
        public PaymentServiceBuilder AddRefundService<T>() where T : class, IRefundService, new()
        {
            _refundFactories.Add(() => new T());
            return this;
        }

        /// <summary>注册关单服务</summary>
        public PaymentServiceBuilder AddCloseService<T>() where T : class, IPayOrderCloseService, new()
        {
            _closeFactories.Add(() => new T());
            return this;
        }

        public PaymentServiceFactory Build()
            => new PaymentServiceFactory(_paymentFactories, _queryFactories, _refundFactories, _closeFactories, _loggerFactory);
    }
}
