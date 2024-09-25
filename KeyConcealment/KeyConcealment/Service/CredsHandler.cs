using System;
using System.Collections.Generic;
using KeyConcealment.Cryptography;
using KeyConcealment.Domain;
using KeyConcealment.Pers;

namespace KeyConcealment.Service;

public class CredsHandler : ICredsManager
{
    #region attributes
    private readonly IPersMstPwd _persMastPwd;
    private readonly IPersCred<string,ICred<string>> _persCreds;
    private readonly ICrypto _crypto;
    #endregion


    #region singleton
    private static CredsHandler? _instance = null;
    private static object _mutex = new object();

    private CredsHandler()
    {
        this._persMastPwd = PersMstPwd.Instance;
        this._persCreds = PersCred.Instance;
        this._crypto = Crypto.Instance;
    }

    public static CredsHandler Instance {
        get
        {
            lock(_mutex)
            {
                _instance ??= new CredsHandler();

                return _instance;
            }
        }
    }

    #endregion

    #region ICredsManager methods
    public List<ICred<string>> GetAllCredentials()
    {
        return this._persCreds.ListAll();
    }

    public void AddCredentials(string masterPwd, string id, string usr, string mail, string speChars, int pwdLength)
    {
        throw new NotImplementedException();
    }

    public void AddCredentials(string masterPwd, string id, string usr, string mail, string pwd)
    {
        throw new NotImplementedException();
    }

    public void PasswordCopy(string masterPwd, string id)
    {
        /*
        public static void CleanClipboard(string pattern)
        {
            string clipboardText = Clipboard.GetText();

            // Remove the pattern from the clipboard text
            string cleanedText = clipboardText.Replace(pattern, "");

            // Set the cleaned text back to the clipboard
            Clipboard.SetText(cleanedText);
        }
        */
        throw new NotImplementedException();
    }

    public void PasswordInfo(string masterPwd, string id)
    {
        throw new NotImplementedException();
    }

    public void RegeneratePassword(string masterPwd, string id)
    {
        throw new NotImplementedException();
    }

    public void SaveChanges(string masterPwd, ICred<string> c)
    {
        /* REMINDER:
        *  this methd needs to check if current c has any changes before saving new values
        */
        throw new NotImplementedException();
    }
    #endregion

}
