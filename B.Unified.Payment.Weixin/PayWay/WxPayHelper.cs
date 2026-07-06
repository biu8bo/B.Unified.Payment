using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Models;
using Senparc.Weixin.Entities;
using Senparc.Weixin.TenPayV3.Apis;
using Senparc.Weixin.TenPayV3.Apis.BasePay;
using Senparc.Weixin.TenPayV3.Helpers;

namespace B.Unified.Payment.Weixin.PayWay
{
    /// <summary>
    /// 微信支付共享工具 — 使用 Senparc.Weixin.TenPayV3 SDK。
    /// <para>签名、HTTP 调用、JSAPI/APP 调起签名均由 Senparc SDK 内部完成。</para>
    /// </summary>
    internal static class WxPayHelper
    {
        /// <summary>获取商户配置</summary>
        public static WxpayNormalMchParams GetConfig(MchAppConfigContext ctx)
            => ctx.GetNormalMchParams<WxpayNormalMchParams>(IfCode.WXPAY)
               ?? throw new BizException("未找到微信支付商户配置");

        /// <summary>构建 Senparc 微信支付 V3 配置（用于 BasePayApis 与 TenPaySignHelper）</summary>
        public static ISenparcWeixinSettingForTenpayV3 BuildSetting(WxpayNormalMchParams cfg)
        {
            var setting = new SenparcWeixinSetting();
            setting.TenPayV3_AppId        = cfg.AppId;
            setting.TenPayV3_MchId        = cfg.MchId;
            setting.TenPayV3_APIv3Key     = cfg.ApiV3Key;
            setting.TenPayV3_SerialNumber = cfg.SerialNo;
            setting.TenPayV3_PrivateKey   = cfg.PrivateKey;
            return setting;
        }

        /// <summary>构建 Senparc BasePayApis 实例（SDK 内部处理签名和 HTTP）</summary>
        public static BasePayApis BuildApi(WxpayNormalMchParams cfg)
            => new BasePayApis(BuildSetting(cfg));

        /// <summary>构建统一下单请求数据</summary>
        public static TransactionsRequestData BuildReqData(UnifiedOrderRQ rq, WxpayNormalMchParams cfg)
        {
            var amount = new TransactionsRequestData.Amount
            {
                total = rq.GetAmountFen(),
                currency = rq.Currency ?? "CNY"
            };

            return new TransactionsRequestData(
                appid: cfg.AppId,
                mchid: cfg.MchId,
                description: rq.Body,
                out_trade_no: rq.PayOrderId,
                time_expire: null,
                attach: null,
                notify_url: rq.NotifyUrl,
                goods_tag: null,
                amount: amount,
                payer: null,
                detail: null,
                settle_info: null,
                scene_info: null,
                support_fapiao: false
            );
        }
    }
}