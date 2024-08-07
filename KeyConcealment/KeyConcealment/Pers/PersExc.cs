using System;

namespace KeyConcealment.Pers;

public class PersExc : Exception
{
    public PersExc()
    {
    }

    public PersExc(string message) : base("Persistence Exception: " +  message)
    {
    }

    public PersExc(string message, Exception inner) : base("Persistence exception: " +  message, inner)
    {
    }
}
