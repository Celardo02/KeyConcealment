using System;
using KeyConcealment.Cryptography;
using KeyConcealment.Domain;
using KeyConcealment.Pers;
using MsBox.Avalonia.Enums;

namespace KeyConcealment.Service;

public class VaultHandler : IVaultManager
{
    #region attributes
    private readonly IPersMstPwd _persMastPwd;
    private readonly IPersCred<string,ICred<string>> _persCreds;
    private readonly ICrypto _crypto;
    #endregion

    #region singleton
    private static VaultHandler? _instance = null;
    private static object _mutex = new object();

    private VaultHandler()
    {
        this._persMastPwd = PersMstPwd.Instance;
        this._persCreds = PersCred.Instance;
        this._crypto = Crypto.Instance;
    }

    public VaultHandler Instance 
    {
        get 
        {
            lock(_mutex)
            {
                _instance ??= new VaultHandler();

                return _instance;
            }
        }
    }
    #endregion

    public void CreateVault(string mstPwd)
    {
        if(!PersSecMem.ValutExists())
            try
            {
                this._persMastPwd.SetNewMasterPwd(mstPwd);
                PersSecMem.Save(mstPwd,this._persMastPwd.MPwd,this._persCreds.ListAll(),this._crypto.OldNonces,this._crypto.OldSalts);
                PopUpMessageHandler.ShowMessage("Info", "Vault created successfully", Icon.Success);
            } 
            catch(PersExc e)
            {
                PopUpMessageHandler.ShowMessage("Error", e.Message, Icon.Error);
            }

        else 
            PopUpMessageHandler.ShowMessage("Error","Vault file found. To create new vault, reset the existing one first.", Icon.Error);

    }

    public async void ResetVault()
    {
        
        if( await PopUpMessageHandler.ShowMessage("Warning","Resetting the vault implies removing each credential set that has been stored. You will NOT be able to get back any data you are about to delete.\nDo you want to proceed anyway?",Icon.Warning,ButtonEnum.YesNo) == ButtonResult.Yes)
            try
            {
                PersSecMem.Delete();
                this._persCreds.DropContent();
                this._persMastPwd.DropContent();
            }
            catch(PersExcNotFound e)
            {
                await PopUpMessageHandler.ShowMessage("Error",e.Message, Icon.Error);
            }
             
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
                PopUpMessageHandler.ShowMessage("Info", "Password changed successfully.", Icon.Success);
            }
            catch(PersExc e)
            {
                PopUpMessageHandler.ShowMessage("Error", e.Message, Icon.Error);
            }
        else 
            PopUpMessageHandler.ShowMessage("Error", "Old master password is incorrect.", Icon.Error);
    }

    public string? GetMasterPwdExp()
    {
        return this._persMastPwd.MPwd?.Exp.ToString("dd/MM/yyyy");
    }
    #endregion

}
