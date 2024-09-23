using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KeyConcealment.Cryptography;
using KeyConcealment.Domain;
using KeyConcealment.Pers;
using KeyConcealment.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace KeyConcealment.Service;

public class Handler : IService
{
    #region attributes
    private readonly IPersMstPwd _persMastPwd;
    private readonly IPersCred<string,ICred<string>> _persCreds;
    private readonly ICrypto _crypto;



    private MainViewModel? _mvm;
    #endregion

    #region singleton
    private static Handler? _instance = null;
    private static object _mutex = new object();

    private Handler()
    {
        this._persMastPwd = PersMstPwd.Instance;
        this._persCreds = PersCred.Instance;
        this._crypto = Crypto.Instance;
    }

    public static Handler Instance {
        get
        {
            lock(_mutex)
            {
                _instance ??= new Handler();

                return _instance;
            }
        }
    }

    #endregion


    #region IService methods

    #region login methods
    public void Login(string insPwd)
    {
        IMasterPwd? mp = null;
        List<ICred<string>>? lc = null;
        List<string>? lon = null;
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

                    this._mvm.IsUserLoggedIn = true;
                }
                else 
                    this.ShowMessage("Error","Inserted password is not correct. Please, try again.", Icon.Error);
            } catch(PersExc e)
            {
                this.ShowMessage("Error", e.Message, Icon.Error);
            }
        else 
            this.ShowMessage("Error","Main view is not set. Login can not be performed.", Icon.Error);

    }

    public void Logout()
    {
        if(this._mvm != null)
            this._mvm.IsUserLoggedIn = false;
        else 
            this.ShowMessage("Error","Main view is not set. Logout can not be performed.", Icon.Error);
    }
    #endregion

    #region Vault managing
    public void CreateVault(string mstPwd)
    {
        if(!PersSecMem.ValutExists())
            try
            {
                this._persMastPwd.SetNewMasterPwd(mstPwd);
                PersSecMem.Save(mstPwd,this._persMastPwd.MPwd,this._persCreds.ListAll(),this._crypto.OldNonces,this._crypto.OldSalts);
                this.ShowMessage("Info", "Vault created successfully", Icon.Success);
            } 
            catch(PersExc e)
            {
                this.ShowMessage("Error", e.Message, Icon.Error);
            }

        else 
            this.ShowMessage("Error","Vault file found. To create new vault, reset the existing one first.", Icon.Error);

    }

    public async void ResetVault()
    {
        
        if( await this.ShowMessage("Warning","Resetting the vault implies removing each credential set that has been stored. You will NOT be able to get back any data you are about to delete.\nDo you want to proceed anyway?",Icon.Warning,ButtonEnum.YesNo) == ButtonResult.Yes)
            try
            {
                PersSecMem.Delete();
                this._persCreds.DropContent();
                this._persMastPwd.DropContent();
            }
            catch(PersExcNotFound e)
            {
                this.ShowMessage("Error",e.Message, Icon.Error);
            }
             
    }

    public void AddCredentials(string masterPwd, string id, string usr, string mail, string speChars, int pwdLength)
    {
        throw new NotImplementedException();
    }

    public void AddCredentials(string masterPwd, string id, string usr, string mail, string pwd)
    {
        throw new NotImplementedException();
    }

    #region master password

    public void SetMasterPwd(string oldPwd, string newPwd)
    {
        if(this._persMastPwd.VerifyInsertedPwd(oldPwd))
            try
            {
                this._persMastPwd.SetNewMasterPwd(newPwd);
                // Encrypting all credential sets passwords with the new master password
                this._persCreds.UpdateCredsEncryption(oldPwd,newPwd);
                // saving changes inside vault file
                PersSecMem.Save(newPwd,this._persMastPwd.MPwd,this._persCreds.ListAll(),this._crypto.OldNonces,this._crypto.OldSalts);
                this.ShowMessage("Info", "Password changed successfully.", Icon.Success);
            }
            catch(PersExc e)
            {
                this.ShowMessage("Error", e.Message, Icon.Error);
            }
        else 
            this.ShowMessage("Error", "Old master password is incorrect.", Icon.Error);
    }

    public string? GetMasterPwdExp()
    {
        return this._persMastPwd.MPwd?.Exp.ToString("dd/MM/yyyy");
    }
    #endregion
    #endregion

    public Task<ButtonResult> ShowMessage(string title, string msg,Icon i = Icon.None, ButtonEnum b = ButtonEnum.Ok)
    {
        return MessageBoxManager.GetMessageBoxStandard(title, msg, b, i).ShowAsync();
    }

    #endregion

    public MainViewModel Mvm {set => this._mvm = value;}
}
