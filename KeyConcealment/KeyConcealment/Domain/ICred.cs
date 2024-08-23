﻿using System;

namespace KeyConcealment.Domain;

/// <summary>
/// interface used for a domain class which represents a credential set
/// </summary>
/// <typeparam name="ID"></typeparam>
public interface ICred<ID>
{
    /// <summary>
    /// Unuique id. It's a label assigned by the user to understand which service is  
    /// related to the class instance
    /// </summary>
    ID Id {get; set;}
    /// <summary>
    /// Username. It is nullable as an account may use the e-mail as username
    /// </summary>
    string? Usr {get; set;}
    /// <summary>
    /// E-mail
    /// </summary>
    string? Mail {get; set;}
    /// <summary>
    /// Password
    /// </summary>
    string Pwd { get; set;}
    /// <summary>
    /// Password hash
    /// </summary>
    byte[] Hash {get; set;}
    /// <summary>
    /// Base 64 string containing salt value used to compute Hash
    /// </summary>
    string Salt {get; set;}
    /// <summary>
    /// expiration date: date after which the app advise/force the user to change the password
    /// </summary>
    DateTime Exp {get;}
}
