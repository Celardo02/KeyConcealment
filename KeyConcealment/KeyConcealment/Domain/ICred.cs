using System;

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
    /// Password. It is encrypted with master password
    /// </summary>
    string Pwd { get; set;}
    /// <summary>
    /// Nonce/Initialization vector of the encryption function represented by a base 64 string
    /// </summary>
    string EncNonce {get; set;}
    /// <summary>
    /// Tag used by the encryption function represented by a base 64 string
    /// </summary>
    string EncTag {get; set;}
    /// <summary>
    /// Base 64 string containing salt value used to the Key derivation function of 
    /// the encryption function
    /// </summary>
    string EncSalt {get; set;}
    /// <summary>
    /// expiration date: date after which the app advise/force the user to change the password
    /// </summary>
    DateTime Exp {get;}
}
