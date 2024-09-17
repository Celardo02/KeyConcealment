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
public static class PersSecMem
{
    #region attributes
    private static ICrypto? cryp;
    // name of the directory that is going to contain the vault file 
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
    public static void Save(string key, IMasterPwd masterPwd, List<ICred<string>> creds, List<string> oldNonces, List<string> oldSalts)
    {
        // path where vault is going to be saved
        string path;
        string vaultContent = ""; 
        // KDF function salt value used by AES
        string salt = "";
        // AES nonce
        string nonce = "";
        // AES tag
        string tag = "";

        path = DefinePath();

        // adding all credential sets 
        foreach(ICred<string> c in creds)
            vaultContent += c.ToString() + ";";

        // removing last ';' character
        vaultContent = vaultContent.Remove(vaultContent.Length - 1);
        vaultContent += "\n";

        // adding all nonces 
        foreach(string n in oldNonces)
            vaultContent += n + ";";
        
        // removing last ';' character
        vaultContent = vaultContent.Remove(vaultContent.Length - 1);
        vaultContent += "\n";

        // adding all salts
        foreach(string s in oldSalts)
            vaultContent += s + ";";
        // removing last ';' character
        vaultContent = vaultContent.Remove(vaultContent.Length - 1);

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
    /// Loads all the data contained in the vault file using the encryption key
    /// </summary>
    /// <param name="key">Plain text vault encryption key</param>
    /// <param name="masterPwd">parameter that is going to be filled with master password data</param>
    /// <param name="creds">parameter that is going to be filled with credential sets data</param>
    /// <param name="oldNonces">parameter that is going to be filled with old nonces values</param>
    /// <param name="oldSalts">parameter that is going to be filled with old salts values</param>
    public static void Load(string key, ref IMasterPwd? masterPwd, ref List<ICred<string>>? creds, ref List<string>? oldNonces, ref List<string>? oldSalts)
    {
        string vaultContent;
        string mstPwdString;
        string AESparams;
        string credsString;
        string oldNoncesString;
        string oldSaltsString;

        // checking if the vault exists
        if(!ValutExists())
            throw new PersExcNotFound("Vault loading operations failed: vault file does not exist yet.");

        cryp = Crypto.Instance;
        // importing  vault file
        vaultContent = File.ReadAllText(DefinePath());

        // gaining all the master password data from vaultContent
        mstPwdString = vaultContent.Split("\n")[0];

        // checking if key contains the correct value for the master password
        if(!cryp.VerifyHash(key, mstPwdString.Split(",")[0], mstPwdString.Split(",")[1]))
            throw new PersExc("Provided encryption key is incorrect");

        // gaining AES salt, nonce and tag from vaultContent
        AESparams = vaultContent.Split("\n")[1];

        // removing master password data and AES parameters from vaultContent
        vaultContent = vaultContent.Split("\n")[2];
        vaultContent = cryp.DecryptAES_GMC(vaultContent, key, AESparams.Split(",")[0], AESparams.Split(",")[1], AESparams.Split(",")[2]);

        // gaining all the vault data
        credsString = vaultContent.Split("\n")[0];
        oldNoncesString = vaultContent.Split("\n")[1];
        oldSaltsString = vaultContent.Split("\n")[2];

        creds = new List<ICred<string>>();
        oldNonces = new List<string>();
        oldSalts = new List<string>();

        // loading into method params vault data
        foreach(string c in credsString.Split(";"))
            creds.Add(new Credentials(c.Split(",")[0], c.Split(",")[1], c.Split(",")[2], c.Split(",")[3], c.Split(",")[4], DateTime.Parse(c.Split(",")[5]) , c.Split(",")[6], c.Split(",")[7]));
        
        foreach(string n in oldNoncesString.Split(";"))
            oldNonces.Add(n);

        foreach(string s in oldSaltsString.Split(";"))
            oldSalts.Add(s);

        masterPwd = new MasterPwd(mstPwdString.Split(",")[0],mstPwdString.Split(",")[1],DateTime.Parse(mstPwdString.Split(",")[2]));
    }

    /// <summary>
    /// Checks whether a vault file exists or not
    /// </summary>
    /// <returns>
    /// <c>True</c> if the vault exists; <c>False</c> otherwise
    /// </returns>
    public static bool ValutExists()
    {        
        return File.Exists(DefinePath());
    }

    /// <summary>
    /// Deletes vault file
    /// </summary>
    /// <exception cref="PersExcNotFound">
    /// Throws <c>PersExcNotFound</c> exception if vault does not exist
    /// </exception>
    public static void Delete()
    {
        // checking if the vault exists
        if(!ValutExists())
            throw new PersExcNotFound("Vault loading operations failed: vault file does not exist yet.");

        File.Delete(DefinePath());
    }

    /// <summary>
    /// Defines vault file path depending on running Operating System
    /// </summary>
    /// <returns>
    /// Returns a path according to the running OS file system standard
    /// </returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Throws a <c>PlatformNotSupportedException</c> if the OS isn't one between 
    /// Linux, Windows or Android
    /// </exception>
    private static string DefinePath()
    {
        string path;
        // choosing the path where the vault is going to be saved depending on the running OS
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

        return path;
    }
}
