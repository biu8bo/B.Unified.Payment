using System.Collections.Generic;

namespace B.Unified.Payment.Abstract.Models
{
    /// <summary>
    /// 商户应用支付配置上下文 — caller 负责填充后传入 SDK
    /// </summary>
    public class MchAppConfigContext
    {
        #region 商户/应用

        /// <summary>商户号</summary>
        public string MchNo { get; set; }

        /// <summary>应用ID</summary>
        public string AppId { get; set; }

        /// <summary>商户类型: 1-普通商户, 2-特约商户(服务商模式)</summary>
        public byte? MchType { get; set; }

        /// <summary>商户信息（caller注入）</summary>
        public object MchInfo { get; set; }

        /// <summary>商户应用信息（caller注入）</summary>
        public object MchApp { get; set; }

        #endregion

        #region 渠道参数

        /// <summary>普通商户支付参数, &lt;接口代码, 参数对象&gt;</summary>
        public Dictionary<string, NormalMchParams> NormalMchParamsMap { get; set; } =
            new Dictionary<string, NormalMchParams>();

        /// <summary>特约商户(子商户)支付参数</summary>
        public Dictionary<string, IsvsubMchParams> IsvsubMchParamsMap { get; set; } =
            new Dictionary<string, IsvsubMchParams>();

        #endregion

        #region 服务商

        /// <summary>服务商配置</summary>
        public IIsvConfigContext IsvConfigContext { get; set; }

        #endregion

        #region 便捷方法

        public NormalMchParams GetNormalMchParams(string ifCode)
        {
            NormalMchParams result;
            NormalMchParamsMap.TryGetValue(ifCode, out result);
            return result;
        }

        public T GetNormalMchParams<T>(string ifCode) where T : NormalMchParams
        {
            NormalMchParams result;
            NormalMchParamsMap.TryGetValue(ifCode, out result);
            return result as T;
        }

        public IsvsubMchParams GetIsvsubMchParams(string ifCode)
        {
            IsvsubMchParams result;
            IsvsubMchParamsMap.TryGetValue(ifCode, out result);
            return result;
        }

        public T GetIsvsubMchParams<T>(string ifCode) where T : IsvsubMchParams
        {
            IsvsubMchParams result;
            IsvsubMchParamsMap.TryGetValue(ifCode, out result);
            return result as T;
        }

        public bool IsIsvsubMch() => MchType == 2;

        #endregion
    }

    #region 占位基类 / 接口

    /// <summary>普通商户参数基类（各通道继承）</summary>
    public class NormalMchParams { }

    /// <summary>特约商户参数基类</summary>
    public class IsvsubMchParams { }

    /// <summary>服务商配置上下文接口</summary>
    public interface IIsvConfigContext { }

    #endregion
}