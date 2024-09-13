using System;
using System.Collections.Generic;
using System.IO;
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
    #region attributes
    private static ICrypto? cryp;
    // name of the directory that will contain the vault file 
    private const string VAULT_DIR = "KeyConcealment";
    private const string VAULT_NAME = "Vault.enc";
    #endregion

    /// <summary>
    /// Saves the current persistences content inside the vault file
    /// </summary>
    /// <param name="key">encryption key to use for the file content encryption</param>
    /// <param name="masterPwd">domain class with all the master password data</param>
    /// <param name="creds">list with all credential sets</param>
    /// <param name="oldNonces">list containing all previously used nonce values</param>
    /// <param name="oldSalts">list containing all previously used salt values</param>
    public static void Save(string key, IMasterPwd masterPwd, List<V> creds, List<string> oldNonces, List<string> oldSalts)
    {
        // path where vault will be saved
        string path;
        string vaultContent = ""; 
        // KDF function salt value used by AES
        string salt = "";
        // AES nonce
        string nonce = "";
        // AES tag
        string tag = "";

        // choosing the path where the vault will be saved depending on the running OS
        if (OperatingSystem.IsLinux())
            // Environment.SpecialFolder.LocalApplicationData translates to /$HOME/.local/share
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), VAULT_DIR, VAULT_NAME);
        else if (OperatingSystem.IsAndroid())
            // Environment.SpecialFolder.Personal translates to /data/data/[packageName]/files, 
            // which a dedicated directory for the app on android systems
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), VAULT_NAME);
        else if (OperatingSystem.IsWindows())
            // Environment.SpecialFolder.ApplicationData translates to AppData dir
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), VAULT_DIR, VAULT_NAME);
        else 
            throw new PlatformNotSupportedException("Current platform is not supported. KeyConcealment supports only Linux, Windows and Android platforms");

        // adding all credential sets 
        foreach(V c in creds)
            vaultContent += c.ToString() + ";";

        vaultContent += "\n";

        // adding all nonces 
        foreach(string n in oldNonces)
            vaultContent += n + ";";
        
        vaultContent += "\n";

        // adding all salts
        foreach(string s in oldSalts)
            vaultContent += s + ";";

        cryp = Crypto.Instance;

        vaultContent = cryp.EncryptAES_GMC(vaultContent, key, ref salt, ref nonce, ref tag);

        // adding KDF salt, AES nonce and AES tag on top of the ecrypted vault
        vaultContent = salt + "," + nonce + "," + tag + "\n" + vaultContent;

        // adding master password data on top of everything
        vaultContent = masterPwd.ToString() + "\n" + vaultContent;

        // saving the vault
        File.WriteAllText(path,vaultContent);
    }

    /// <summary>
    /// Load persistences content from vault file
    /// </summary>
    /// <param name="key">encryption key to use for the file content decryption</param>
    /// <returns>
    /// Returns a string containig the content of the file read
    /// </returns>
    public static string Load(string key)
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

    /// <summary>
    /// Checks if a vault exists at a specified path
    /// </summary>
    /// <param name="path">path where the vault may be found</param>
    /// <returns>
    /// <c>True</c> if the vault exists; <c>False</c> otherwise
    /// </returns>
    public static bool ValutExists(string path)
    {        
        throw new NotImplementedException();
    }
}
