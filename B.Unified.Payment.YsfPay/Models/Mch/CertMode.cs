namespace B.Unified.Payment.YsfPay.Models.Mch
{
    /// <summary>云闪付密钥模式: 0-公钥模式, 1-证书模式</summary>
    public enum CertMode : byte
    {
        PublicKey = 0,
        Certificate = 1
    }
}
