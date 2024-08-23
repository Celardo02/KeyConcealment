using System;

namespace KeyConcealment.Cryptography;

public class CryptoArgExc : CryptoExc
{
    public CryptoArgExc()
    {
    }

    public CryptoArgExc(string message) : base(message)
    {
    }

    public CryptoArgExc(string message, Exception inner) : base(message, inner)
    {
    }
}
