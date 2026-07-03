using System;

namespace B.Unified.Payment.Abstract.Exceptions
{
    /// <summary>
    /// 通用业务异常
    /// </summary>
    public class BizException : Exception
    {
        public BizException(string message) : base(message) { }
        public BizException(string message, Exception innerException) : base(message, innerException) { }
    }
}