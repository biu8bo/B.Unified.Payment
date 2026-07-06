using System;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Mch;
using Newtonsoft.Json;

namespace B.Unified.Payment.Abstract.Models.Payment
{
    /// <summary>
    /// 统一支付请求参数 — 所有支付方式共用同一入参。
    /// <para>caller 填充所有公共字段，渠道特有字段走 ChannelExtra JSON。</para>
    /// </summary>
    public class UnifiedOrderRQ
    {
        // #region 商户信息
        //
        // /// <summary>商户号</summary>
        // public string MchNo { get; set; }
        //
        // /// <summary>商户应用ID</summary>
        // public string AppId { get; set; }
        //
        // #endregion
        //
        // #region 签名/版本（可选，caller 自行管理签名）
        //
        // /// <summary>接口版本号</summary>
        // public string Version { get; set; }
        //
        // /// <summary>签名类型</summary>
        // public string SignType { get; set; }
        //
        // /// <summary>签名值</summary>
        // public string Sign { get; set; }
        //
        // /// <summary>请求时间</summary>
        // public string ReqTime { get; set; }
        //
        // #endregion

        #region 核心参数

        /// <summary>支付订单号（对应渠道 out_trade_no）</summary>
        public string PayOrderId { get; set; }

        /// <summary>商户订单号</summary>
        public string MchOrderNo { get; set; }

        /// <summary>支付方式代码（如 AlipayPayWay.BAR / WxPayWay.JSAPI）</summary>
        public PayWayCode WayCode { get; set; }

        /// <summary>支付金额，单位：分</summary>
        public long? Amount { get; set; }

        /// <summary>货币代码，默认 CNY</summary>
        public string Currency { get; set; } = CurrencyCode.CNY;

        #endregion

        #region 商品信息

        /// <summary>商品标题</summary>
        public string Subject { get; set; }

        /// <summary>商品描述</summary>
        public string Body { get; set; }

        #endregion

        #region 通知 / 跳转 — 由调用方传入

        /// <summary>异步通知地址（必须由调用方传入）</summary>
        public string NotifyUrl { get; set; }

        /// <summary>同步跳转地址</summary>
        public string ReturnUrl { get; set; }

        #endregion

        #region 时间 / 客户端

        /// <summary>订单失效时间</summary>
        public DateTime? ExpiredTime { get; set; }

        /// <summary>客户端IP</summary>
        public string ClientIp { get; set; }

        #endregion

        #region 渠道用户

        /// <summary>渠道用户标识（微信 openid / 支付宝 buyerUserId）</summary>
        public string ChannelUserId { get; set; }

        #endregion

        #region 被扫支付

        /// <summary>用户支付条码 / 授权码（条码支付）</summary>
        public string AuthCode { get; set; }

        #endregion

        #region 扩展参数

        /// <summary>渠道特有额外参数（JSON格式，各通道自行解析）</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ChannelExtra { get; set; }

        /// <summary>商户自定义扩展参数</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ExtParam { get; set; }

        #endregion

        #region 便捷方法

        /// <summary>
        /// 从 ChannelExtra JSON 解析指定类型的扩展数据。
        /// <para>用法: var extra = rq.ParseChannelExtra&lt;WxH5Extra&gt;();</para>
        /// </summary>
        public T ParseChannelExtra<T>() where T : class, new()
        {
            if (string.IsNullOrEmpty(ChannelExtra))
                return new T();
            return JsonConvert.DeserializeObject<T>(ChannelExtra) ?? new T();
        }

        /// <summary>分→元（长格式，如 100 → "1.00"）</summary>
        public string GetAmountYuan()
        {
            if (Amount == null) return "0.00";
            return (Amount.Value / 100.0m).ToString("F2");
        }

        /// <summary>获取金额（分）</summary>
        public int GetAmountFen()
        {
            return (int)(Amount ?? 0);
        }

        #endregion
    }
}