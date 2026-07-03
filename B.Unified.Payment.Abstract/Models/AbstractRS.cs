using System;
using Newtonsoft.Json;

namespace B.Unified.Payment.Abstract.Models
{
    /// <summary>
    /// 接口抽象RS对象, 本身无需实例化
    /// </summary>
    [Serializable]
    public abstract class AbstractRS
    {
        /// <summary>
        /// 将对象序列化为JSON字符串
        /// </summary>
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}