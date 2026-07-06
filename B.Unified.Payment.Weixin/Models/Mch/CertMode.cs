namespace B.Unified.Payment.Weixin.Models.Mch
{
    /// <summary>微信支付密钥模式: 0-公钥模式, 1-平台证书模式</summary>
    public enum CertMode : byte
    {
        PublicKey = 0,
        Certificate = 1
    }
}
