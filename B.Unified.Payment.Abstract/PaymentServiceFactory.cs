using System;
using System.Collections.Generic;
using System.Linq;
using B.Unified.Payment.Abstract.Diagnostics;
using Microsoft.Extensions.Logging;

namespace B.Unified.Payment.Abstract
{
    /// <summary>
    /// 支付服务工厂 — 根据 ifCode + wayCode 从容器中获取对应的支付/查单/退款服务。
    /// </summary>
    public interface IPaymentServiceFactory
    {
        /// <summary>按渠道 + 支付方式获取支付服务</summary>
        IPaymentService GetPaymentService(string ifCode, string wayCode);

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
            PayLogger.Initialize(loggerFactory);
        }

        public IPaymentService GetPaymentService(string ifCode, string wayCode)
        {
            if (string.IsNullOrEmpty(ifCode))
                throw new ArgumentException("ifCode 不能为空", nameof(ifCode));
            if (string.IsNullOrEmpty(wayCode))
                throw new ArgumentException("wayCode 不能为空", nameof(wayCode));

            return _paymentServices.FirstOrDefault(s => s.GetIfCode() == ifCode && s.IsSupport(wayCode))
                ?? throw new InvalidOperationException($"未注册 ifCode='{ifCode}' wayCode='{wayCode}' 的 IPaymentService");
        }

        public IPayOrderQueryService GetQueryService(string ifCode)
            => _queryServices.FirstOrDefault(s => s.GetIfCode() == ifCode)
               ?? throw new InvalidOperationException($"未注册 ifCode='{ifCode}' 的 IPayOrderQueryService");

        public IRefundService GetRefundService(string ifCode)
            => _refundServices.FirstOrDefault(s => s.GetIfCode() == ifCode)
               ?? throw new InvalidOperationException($"未注册 ifCode='{ifCode}' 的 IRefundService");
    }
}
