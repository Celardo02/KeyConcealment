using System;

namespace KeyConcealment.Domain;

/// <summary>
/// interface used for a domain class which represents a credential set
/// </summary>
/// <typeparam name="ID"></typeparam>
public interface ICred<ID>
{
    ID Id {get; set;}
    string? Usr {get; set;}
    string Mail {get; set;}
    string Pwd { get; set;}
    DateTime Exp {get;}
}
