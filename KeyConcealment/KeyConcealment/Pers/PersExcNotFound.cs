using System;

namespace KeyConcealment.Pers;

public class PersExcNotFound : PersExc
{
    public PersExcNotFound()
    {
    }

    public PersExcNotFound(string message) : base(message)
    {
    }

    public PersExcNotFound(string message, Exception inner) : base(message, inner)
    {
    }
}
