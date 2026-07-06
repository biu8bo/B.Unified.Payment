namespace B.Unified.Payment.Abstract.Models.Mch
{
    /// <summary>
    /// 0-公钥模式, 1-证书模式
    /// </summary>
    public enum CertMode : byte
    {
        PublicKey = 0,
        Certificate = 1
    }
}
