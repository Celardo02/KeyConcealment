using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using KeyConcealment.Cryptography;
using KeyConcealment.Domain;
using KeyConcealment.Pers;
using KeyConcealment.ViewModels;
using MsBox.Avalonia.Enums;

namespace KeyConcealment.Service;

public class CredsHandler : ICredsManager
{
    #region attributes
    private readonly IPersMstPwd _persMastPwd;
    private readonly IPersCred<string,ICred<string>> _persCreds;
    private readonly ICrypto _crypto;

    private IClipboard? _clip;

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

        if(OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
            this._clip = TopLevel.GetTopLevel(((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow).Clipboard;
        else if (OperatingSystem.IsAndroid())
            this._clip = TopLevel.GetTopLevel(((ISingleViewApplicationLifetime)Application.Current.ApplicationLifetime).MainView).Clipboard;
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

    public void AddCredentials(string masterPwd, string id, string? usr, string? mail, string specChars, int pwdLength)
    {
        try 
        {
            if(this._persMastPwd.VerifyInsertedPwd(masterPwd))
                this._persCreds.Create(masterPwd, id, usr, mail, this.GenerateRandomPassword(specChars,pwdLength));
            else 
                PopUpMessageHandler.ShowMessage("Error","Typed master password is incorrect", Icon.Error, ButtonEnum.Ok);
        }
        catch (Exception e)
        {
            PopUpMessageHandler.ShowMessage("Error", e.Message, Icon.Error, ButtonEnum.Ok);
        }
    }

    public void AddCredentials(string masterPwd, string id, string? usr, string? mail, string pwd)
    {
        try
        {    
            if(this._persMastPwd.VerifyInsertedPwd(masterPwd))
                this._persCreds.Create(masterPwd, id, usr, mail, pwd);
            else 
                PopUpMessageHandler.ShowMessage("Error","Typed master password is incorrect", Icon.Error, ButtonEnum.Ok);
        }
        catch (Exception e)
        {
            PopUpMessageHandler.ShowMessage("Error", e.Message, Icon.Error, ButtonEnum.Ok);
        }
    }

    public void PasswordCopyAsync(string masterPwd, string id)
    {
        string plainPwd;
        ICred<string> cred;
        
        if(this._clip != null)
        {
            if(this._persMastPwd.VerifyInsertedPwd(masterPwd))
            {
                cred = this._persCreds.Read(id);
                plainPwd = this._crypto.DecryptAES_GMC(cred.Pwd,masterPwd, cred.EncSalt, cred.EncNonce, cred.EncTag);
                this._clip.SetTextAsync(plainPwd);

                new Task(this.CleanClip).Start();
            }
            else 
                PopUpMessageHandler.ShowMessage("Error","Typed master password is incorrect",Icon.Error,ButtonEnum.Ok);
        }
        else 
            PopUpMessageHandler.ShowMessage("Error","Current platform is not supported",Icon.Error,ButtonEnum.Ok);

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

    /// <summary>
    /// Waits for 20 seconds and then cleans the clipboard. This method is meant to 
    /// be only called by ClipboardCopyAsync. Do note invoke this method
    /// </summary>
    private void CleanClip()
    {
        System.Threading.Thread.Sleep(20000);

        this._clip.ClearAsync();
    }

}
