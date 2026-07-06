using B.Unified.Payment.Abstract.Models;
using B.Unified.Payment.Alipay.Constants;
using B.Unified.Payment.Alipay.Models;

namespace B.Unified.Payment.AlipaySample;

/// <summary>支付宝共享配置 — 支付和查询 Demo 共用</summary>
public static class AlipayConfig
{
    public static readonly MchAppConfigContext Context = Create();

    private static MchAppConfigContext Create()
    {
        var ctx = new MchAppConfigContext();
        ctx.NormalMchParamsMap[IfCode.ALIPAY] = new AlipayNormalMchParams
        {
            AppId           = "2021000147628093",
            PrivateKey      = "MIIEowIBAAKCAQEAzeBX09MzW16qmI8V0/olzpKxq7JI8mxF+7a7LNHUJJlzSFYJYigdtqztOB5zeaJlzGMQ0jDjT6qZqIZr3v7ADM7BGGzCG3piOagK9W0KKfKQ4jjQ4DEPYQbZCTt/xr3hd4/mia16nHlFPeTHcddvbLZYOVvlMn/UM5b6pc1i7OfQ6TOtoqEwsjodw7RsPx2gDh+N5V7WNDXPeMsCoe0k7hMoMZ6FSE0h3J0XV64I9KlX84q65i7lrmim8dVVv9Vd9MvnHSn/Q+uvdo3IA6J+o0jG3xBeCgkAmiMkxs8BKFLp9oa8MXFZB74IeFF4fLZviTMDjmbyhd6IOp5TCUJLFwIDAQABAoIBACFJOGoDJ7aKI8Luv3S6aQpxJVDBTpIDd30vGiww8L/KH51+a533Jna2ltQP+FOeMh9NlRam2Nm0l4tr0F0JizuG4il0zB1tOBxiUwNDUfVeRpaM4RieVgI1/TlE6W/Um3OdTITOC5jo8o0DREvfrSBCixkbBn+Xs1N0Aap0/p2WwvLjFB2DwAOAqlsZZQ74aNyia8QgoBMgY6VtltPRYhf2l/CuOC754VN2gwMULIJduIXHzd6k2zb7b+5ogtuhHJkFI/E71QnUP3qdER9udyazvXTKOnphhXvf9nkmly4RPouyOmnOCT1w66VjZU2rQXMFOcLXbbfrJJcjoYHsswECgYEA+LJ+hV5mww3BK5121hAw960wvXs4Kp1WUcEtCX8njDI9NNefnxXsfQKfiZCLp5S1+3Y66GsIeAJR1alk1zY/lmt1HXCNX4o7NyFRRxsfzXTv6jxTuMYvj05whIGYjYblFAEj4OkCnho80KSPWzrR8PLCnMikIFcexzQzm9ssziECgYEA0+v0rI18G1KyydsoObCUhqA/4rb4eKp60lSH7tujufIIySMmBINP6vraH6qMZ5hVq9fbFtfb37YsGsgXKwlw231QSiX/VdohHvfeYd/rY0LD1rpTIdHQXJS5C1IjBN1usNNyXVwi5AMyVVfjfJ2XNB6jHmMow1DXnKIZVC7NwjcCgYEA9kDf8LV58XfkJ4jCy9G6evSdx3GEOwYSG9+49adXhIWWf4Vmg8LUqS/4wuFCt4wT6ku2pr6c4yAA4hzaQhNwQURj8eOpyMl6OuudrFfaVLmOehSEHfj3zOGxnjMo2DKTEAzU9vYiZmS6hSn83SvQB9KJC2/MvE0np74zwAb1RaECgYA2BVXvjnludZxBvG36lrqlvr/KSR35lGuOpiGoj7Ciu8Hlk+IjEF4U5jEoFU+JMNnV3kZpAkl4M3X2tb7CJ7vvF3iaDimSdvIudLzpci0Mtn45hHGgk11r3DV3X06x9Mg8pwnmJpB2UyJHgwnoQDvE+3JVUq2XbEoqEWAnh27H7QKBgGzxVHnvGWjcDZl7/SxKCmEak0CtZ88tl/CnTF0QTWN7T68Fj0ag+Cq7s77ZTHP2KPS0AvfdRD2qkLOXI/BQjIGkfN7eiikrjU7C9SgIkh1k9d+1feAYCFzzYUgqYlsrx2ajdSzE5Crl/cTNK/iI+oICVN9RbE+MwNoqD6Agrg9K",
            AlipayPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAlTbnWLOqL7YO+dE35+DTE7O5/zX7MtCjXBa6QWcxL7iCrVetiy9GDPaN+OLvkO2iHeTOeZzUwvmYOErAQPYPu4+lcP+fJYdpBgeHugrErtdj6Hzq2IrIMA9JGzJGXJxF1Sd2KNYD1ye+Eo9NySs0HOvN2G+0ArvkZLlxZpIM92vV85BVmaiO1AE4c0BRyGASrgwqIM1WaXtEzWuCJI53i5t64aTFaMc/Y6Iz3yf41YsRrVA48dwzkT2biKkivi9AHgDq0dQ4eA9WC2BX1zUu8AHpL2mSMD5gKJabnsoOVv0SHqie5cj9GLAgi0bb93KwmZ1X1bSNNQXULTF5EchZTwIDAQAB",
            Sandbox         = 1,
            SignType        = "RSA2"
        };
        return ctx;
    }
}