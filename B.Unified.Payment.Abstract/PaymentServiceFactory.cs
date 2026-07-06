using System;
using System.Collections.Generic;
using System.Linq;
using B.Unified.Payment.Abstract.Diagnostics;
using Microsoft.Extensions.Logging;

namespace B.Unified.Payment.Abstract
{
    /// <summary>
    /// 支付服务工厂 — 根据 ifCode 从容器中获取对应的支付/查单/退款服务。
    /// <para>自动初始化 PayLogger（通过构造函数注入 ILoggerFactory）。</para>
    /// </summary>
    public interface IPaymentServiceFactory
    {
        IPaymentService GetPaymentService(string ifCode);
        IPayOrderQueryService GetQueryService(string ifCode);
        IRefundService GetRefundService(string ifCode);
    }

    public class PaymentServiceFactory : IPaymentServiceFactory
    {
        private readonly IEnumerable<IPaymentService> _paymentServices;
        private readonly IEnumerable<IPayOrderQueryService> _queryServices;
        private readonly IEnumerable<IRefundService> _refundServices;

        public PaymentServiceFactory(
            IEnumerable<IPaymentService> paymentServices,
            IEnumerable<IPayOrderQueryService> queryServices,
            IEnumerable<IRefundService> refundServices,
            ILoggerFactory loggerFactory)
        {
            _paymentServices = paymentServices;
            _queryServices = queryServices;
            _refundServices = refundServices;

            // 自动初始化 PayLogger，用户无需手动调用 Configure
            PayLogger.Initialize(loggerFactory);
        }

        public IPaymentService GetPaymentService(string ifCode)
            => _paymentServices.FirstOrDefault(s => s.GetIfCode() == ifCode)
               ?? throw new InvalidOperationException($"未注册 ifCode='{ifCode}' 的 IPaymentService");

        public IPayOrderQueryService GetQueryService(string ifCode)
            => _queryServices.FirstOrDefault(s => s.GetIfCode() == ifCode)
               ?? throw new InvalidOperationException($"未注册 ifCode='{ifCode}' 的 IPayOrderQueryService");

        public IRefundService GetRefundService(string ifCode)
            => _refundServices.FirstOrDefault(s => s.GetIfCode() == ifCode)
               ?? throw new InvalidOperationException($"未注册 ifCode='{ifCode}' 的 IRefundService");
    }
}