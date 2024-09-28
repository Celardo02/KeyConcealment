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

    // character set from which derive the password string
    // it contains all capital letters, lowercase letters and digts from 0 to 9
    private const string CHAR_SET = "AaBb0CcDd1EeFf2GgHh3IiJj4KkLl5MmNn6OoPp7QqRr8SsTt9UuVvWwXxYyZz";
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

    public void AddCredentials(string masterPwd, string id, string usr, string mail, string specChars, int pwdLength)
    {
        //TO DO: add try catch
        this._persCreds.Create(new Credentials(id, this.GenerateRandomPassword(specChars,pwdLength), mail, usr), masterPwd);
    }

    public void AddCredentials(string masterPwd, string id, string usr, string mail, string pwd)
    {
        //TO DO: add try catch
        this._persCreds.Create(new Credentials(id, pwd, mail, usr), masterPwd);
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

    /// <summary>
    /// Generates a random password of a specified length that contains given special characters
    /// </summary>
    /// <param name="specChars">string with all required special characters</param>
    /// <param name="pwdLength">generated password length</param>
    /// <returns>Returns the randomly generated password</returns>
    private string GenerateRandomPassword(string specChars, int pwdLength)
    {
        string randomPwd = "";

        // fill the string with random characters from CHAR_SET 
        for (int i = 0; i < (pwdLength - specChars.Length); i++)
        {
            randomPwd += CHAR_SET[this._crypto.GenerateRandomNumber(0,CHAR_SET.Length)];
        }

        // adding required special characters
        randomPwd += specChars;

        // Convert the string to an array to shuffle it
        char[] randomPwdArray = randomPwd.ToCharArray();

        // Fisher–Yates shuffle
        for (int i = randomPwdArray.Length - 1; i > 0; i--)
        {
            int j = this._crypto.GenerateRandomNumber(0, i + 1);
            char temp = randomPwdArray[i];
            randomPwdArray[i] = randomPwdArray[j];
            randomPwdArray[j] = temp;
        }

        // Convert the shuffled array back to a string and return it
        return new string(randomPwdArray);
    }

}
