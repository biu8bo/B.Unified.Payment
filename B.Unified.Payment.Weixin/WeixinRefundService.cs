using System;
using System.Threading.Tasks;
using B.Unified.Payment.Abstract;
using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Abstract.Models.Refund;
using B.Unified.Payment.Weixin.Constants;
using B.Unified.Payment.Weixin.Models;
using B.Unified.Payment.Weixin.PayWay;
using Senparc.Weixin.TenPayV3.Apis;
using Senparc.Weixin.TenPayV3.Apis.BasePay;

namespace B.Unified.Payment.Weixin
{
    /// <summary>
    /// 微信支付退款服务 — Senparc SDK。
    /// <para>发起: BasePayApis.RefundAsync(RefundRequestData), 查单: BasePayApis.RefundQueryAsync(out_refund_no)</para>
    /// <para>状态映射: SUCCESS → CONFIRM_SUCCESS, PROCESSING → WAITING, CLOSED/ABNORMAL → CONFIRM_FAIL</para>
    /// </summary>
    public class WeixinRefundService : IRefundService
    {
        public string GetIfCode() => IfCode.WXPAY;

        public string PreCheck(RefundOrderRQ bizRQ, MchAppConfigContext ctx)
        {
            if (bizRQ == null) return "退款请求参数不能为空";
            if (string.IsNullOrEmpty(bizRQ.RefundOrderId)) return "退款单号不能为空";
            if (bizRQ.RefundAmount == null || bizRQ.RefundAmount <= 0) return "退款金额无效";
            if (string.IsNullOrEmpty(bizRQ.PayOrderId)) return "原支付订单号不能为空";
            if (bizRQ.PayOrderAmount == null || bizRQ.PayOrderAmount <= 0) return "原支付金额无效";
            return null;
        }

        public async Task<ChannelRetMsg> RefundAsync(RefundOrderRQ bizRQ, MchAppConfigContext ctx)
        {
            try
            {
                var cfg = WxPayHelper.GetConfig(ctx);
                var amount = new RefundRequestData.Amount(
                    refund: (int)bizRQ.RefundAmount.Value,
                    from: null,
                    total: (int)bizRQ.PayOrderAmount.Value,
                    currency: bizRQ.Currency ?? "CNY");

                var reqData = new RefundRequestData(
                    transaction_id: bizRQ.ChannelOrderNo,
                    out_trade_no: bizRQ.PayOrderId,
                    out_refund_no: bizRQ.RefundOrderId,
                    reason: bizRQ.RefundReason,
                    notify_url: bizRQ.NotifyUrl,
                    funds_account: null,
                    amount: amount,
                    goods_detail: null);

                var result = await WxPayHelper.BuildApi(cfg).RefundAsync(reqData);
                var status = result?.status;
                var refundId = result?.refund_id;

                if (status == "SUCCESS")
                    return ChannelRetMsg.ConfirmSuccess(refundId);
                if (status == "PROCESSING")
                    return ChannelRetMsg.Waiting(refundId);

                return ChannelRetMsg.ConfirmFail(status, status);
            }
            catch (Exception ex)
            {
                return ChannelRetMsg.SysError(ex.Message);
            }
        }

        public async Task<ChannelRetMsg> QueryAsync(string refundOrderId, string payOrderId, string channelOrderNo, MchAppConfigContext ctx)
        {
            try
            {
                var cfg = WxPayHelper.GetConfig(ctx);
                var result = await WxPayHelper.BuildApi(cfg).RefundQueryAsync(refundOrderId);
                var status = result?.status;

                if (status == "SUCCESS")
                    return ChannelRetMsg.ConfirmSuccess();
                if (status == "CLOSED" || status == "ABNORMAL")
                    return ChannelRetMsg.ConfirmFail(status, status);

                return ChannelRetMsg.Waiting();
            }
            catch (Exception ex)
            {
                return ChannelRetMsg.SysError(ex.Message);
            }
        }
    }
}
