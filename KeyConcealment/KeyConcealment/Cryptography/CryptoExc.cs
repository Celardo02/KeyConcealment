using System;

namespace KeyConcealment.Cryptography;

public class CryptoExc : Exception
{
    public CryptoExc()
    {
    }

    public CryptoExc(string message) : base("Cryptography Exception: " +  message)
    {
    }

    public CryptoExc(string message, Exception inner) : base("Cryptography exception: " +  message, inner)
    {
    }
}
