using System.Threading.Tasks;
using B.Unified.Payment.Abstract.Exceptions;
using B.Unified.Payment.Abstract.Interfaces;
using B.Unified.Payment.Abstract.Models.Channel;
using B.Unified.Payment.Abstract.Models.Mch;
using B.Unified.Payment.Abstract.Models.Payment;
using B.Unified.Payment.YsfPay.Utils;
using Newtonsoft.Json.Linq;

namespace B.Unified.Payment.YsfPay.Services.Close
{
    /// <summary>云闪付关单 — /gateway/api/pay/closeOrder</summary>
    public class YsfpayPayOrderCloseService : IPayOrderCloseService
    {
        public string GetIfCode() => Constants.IfCode.YSFPAY;

        public Task<ChannelRetMsg> CloseAsync(CloseOrderRQ rq, MchAppConfigContext ctx)
        {
            if (string.IsNullOrEmpty(rq.WayCode))
                throw new BizException("云闪付关单 WayCode 不能为空");

            var cfg = YsfpayConfigHelper.GetConfig(ctx);

            var reqParams = new JObject
            {
                ["orderNo"]   = rq.PayOrderId,
                ["orderType"] = YsfHttpUtil.GetOrderType(rq.WayCode)
            };

            try
            {
                var resJson = YsfHttpUtil.PackageParamAndReq("/gateway/api/pay/closeOrder", reqParams, cfg);
                if (resJson == null)
                    return Task.FromResult(ChannelRetMsg.SysError("【云闪付】请求关闭订单异常"));

                var respCode = resJson["respCode"]?.ToString();
                if (respCode == "00")
                    return Task.FromResult(ChannelRetMsg.ConfirmSuccess());

                return Task.FromResult(ChannelRetMsg.SysError(resJson["respMsg"]?.ToString()));
            }
            catch (System.Exception ex)
            {
                return Task.FromResult(ChannelRetMsg.SysError(ex.Message));
            }
        }
    }
}
