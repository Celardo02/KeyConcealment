using System;
using System.Security.Cryptography;

namespace KeyConcealment.Domain;

/// <summary>
/// interface used for a domain class that represents the master password. This password 
/// is used to unlock the app and to cypher all the other passwords
/// </summary>
public interface IMasterPwd
{
    /// <summary>
    /// base 64 string containing master password salted hash
    /// </summary>
    string Hash {get; set;}
    
    /// <summary>
    /// salt added to the password before computing the hash. base64 format only
    /// </summary>
    string Salt{get; set;} 
    
    /// <summary>
    /// expiration date of the master password
    /// </summary>
    DateTime Exp {get; set;}
}
