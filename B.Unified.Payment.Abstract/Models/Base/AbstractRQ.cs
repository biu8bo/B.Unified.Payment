using System;

namespace B.Unified.Payment.Abstract.Models.Base
{
    /// <summary>
    /// 基础请求参数
    /// </summary>
    [Serializable]
    public abstract class AbstractRQ
    {
        /// <summary>版本号</summary> 
        public string Version { get; set; }

        /// <summary>签名类型</summary> 
        public string SignType { get; set; }

        /// <summary>签名值</summary> 
        public string Sign { get; set; }

        /// <summary>接口请求时间</summary> 
        public string ReqTime { get; set; }
    }
}