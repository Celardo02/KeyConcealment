using System;
using System.Collections.Generic;
using KeyConcealment.Cryptography;
using KeyConcealment.Domain;

namespace KeyConcealment.Pers;

/// <summary>
/// Defines static methods to centralize all reading and 
/// writing operations performed on secondary memory, in order to keep data after closing 
/// the app and restoring them on following launching
/// </summary>
public static class PersSecMem<K,V> where V : ICred<K>
{
    private static ICrypto? _crypto;

     /// <summary>
    /// Saves the current persistences content inside a file
    /// </summary>
    /// <param name="path">path where to save the file</param>
    /// <param name="key">encryption key to use for the file content encryption</param>
    /// <param name="masterPwd">domain class with all the master password data</param>
    /// <param name="creds">list with all credential sets</param>
    /// <param name="oldNonces">list containing all previously used nonce values</param>
    /// <param name="oldSalts">list containing all previously used salt values</param>
    public static void Save(string path, string key, IMasterPwd masterPwd, List<V> creds, List<string> oldNonces, List<string> oldSalts)
    {
        /* Reminder: 
        *  - keep in mind that the password file must contain all the data
        *  about the master password (the ones in the domain class), all the old 
        *  values used as nonces or salts and all credential sets.
        *
        *  - KDF salt, AES nonce (IV), AES tag and masterPwd hash must be saved unencrypted
        *    to allow decrytpion, as they are not sensitive picies of information indeed 
        *
        *  - key must be checked against masterPwd hash to verify if it is correct or 
        *    not
        */
        throw new NotImplementedException();
    }

    /// <summary>
    /// Load persistences content from a file 
    /// </summary>
    /// <param name="path">path where to find the file</param>
    /// <param name="key">encryption key to use for the file content decryption</param>
    public static void Load(string path, string key)
    {
        /* Reminder: 
        *  - keep in mind that the password file must contain all the data
        *  about the master password (the ones in the domain class), all the old 
        *  values used as nonces or salts and all credential sets.
        *
        *  - KDF salt, AES nonce (IV), AES tag and masterPwd hash are saved unencrypted
        *
        *  - key must be checked against masterPwd hash to verify if it is correct or 
        *    not
        */
        throw new NotImplementedException();
    }
}
