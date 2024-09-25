using System;
using System.Collections.Generic;
using KeyConcealment.Cryptography;
using KeyConcealment.Domain;
using KeyConcealment.Pers;
using KeyConcealment.ViewModels;
using MsBox.Avalonia.Enums;

namespace KeyConcealment.Service;

public class LoginHandler : ILoginManager
{
    #region attributes
    private readonly IPersMstPwd _persMastPwd;
    private readonly IPersCred<string,ICred<string>> _persCreds;
    private readonly ICrypto _crypto;
    private MainViewModel? _mvm;
    #endregion

    #region singleton
    private static LoginHandler? _instance = null;
    private static object _mutex = new object();

    private LoginHandler()
    {
        this._persMastPwd = PersMstPwd.Instance;
        this._persCreds = PersCred.Instance;
        this._crypto = Crypto.Instance;
    }

    public static LoginHandler Instance 
    {
        get 
        {
            lock(_mutex)
            {
                _instance ??= new LoginHandler();

                return _instance;
            }
        }
    }
    #endregion
    
    public void Login(string insPwd)
    {
        //mp = master password
        IMasterPwd? mp = null;

        // lc= list credentials
        List<ICred<string>>? lc = null;

        // lon = list old nonces
        List<string>? lon = null;
        
        // los = list old salts
        List<string>? los = null;

        if(this._mvm != null)
            try{
                mp = PersSecMem.GetMasterPwd();
                if(this._crypto.VerifyHash(insPwd, mp.Hash, mp.Salt))
                {
                    PersSecMem.Load(insPwd, ref mp, ref lc, ref lon, ref los);
                    this._persMastPwd.MPwd = mp;

                    foreach(ICred<string> c in lc)
                        this._persCreds.Create(c);
                    
                    this._crypto.OldNonces = lon;
                    this._crypto.OldSalts = los;

                    // singaling main view model that the user has correctly logged in
                    this._mvm.IsUserLoggedIn = true;
                }
                else 
                    PopUpMessageHandler.ShowMessage("Error","Inserted password is not correct. Please, try again.", Icon.Error);
            } catch(PersExc e)
            {
                PopUpMessageHandler.ShowMessage("Error", e.Message, Icon.Error);
            }
        else 
            PopUpMessageHandler.ShowMessage("Error","Main view is not set. Login can not be performed.", Icon.Error);

    }

    public void Logout()
    {
        if(this._mvm != null)
            // singaling main view model that the user has correctly logged out
            this._mvm.IsUserLoggedIn = false;
        else 
            PopUpMessageHandler.ShowMessage("Error","Main view is not set. Logout can not be performed.", Icon.Error);
    }

    public MainViewModel Mvm {set => this._mvm = value;}
}
