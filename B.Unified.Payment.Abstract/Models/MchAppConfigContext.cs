using System.Collections.Generic;

namespace B.Unified.Payment.Abstract.Models
{
    /// <summary>
    /// 商户应用支付配置上下文 — caller 负责填充后传入 SDK。
    /// <para>注意：当前仅支持普通商户模式，不含分账及子商户功能。</para>
    /// </summary>
    public class MchAppConfigContext
    {
        #region 商户/应用

        /// <summary>商户号</summary>
        public string MchNo { get; set; }

        /// <summary>应用ID</summary>
        public string AppId { get; set; }

        /// <summary>商户信息（caller 注入）</summary>
        public object MchInfo { get; set; }

        /// <summary>商户应用信息（caller 注入）</summary>
        public object MchApp { get; set; }

        #endregion

        #region 渠道参数

        /// <summary>商户支付参数, &lt;接口代码, 参数对象&gt;</summary>
        public Dictionary<string, NormalMchParams> NormalMchParamsMap { get; set; } =
            new Dictionary<string, NormalMchParams>();

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

        #endregion
    }

    /// <summary>商户参数基类（各通道继承）</summary>
    public class NormalMchParams { }
}