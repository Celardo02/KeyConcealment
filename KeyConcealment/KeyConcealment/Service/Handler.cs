using System;
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


    MainViewModel? _mvm;
    #endregion

    #region singleton
    private static Handler? _instance = null;
    private static object _mutex = new object();

    private Handler()
    {
        this._persMastPwd = PersMstPwd.Instance;
        this._persCreds = PersCred.Instance;
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
    public void Login(string insPwd)
    {
        try{
            if(this._persMastPwd.VerifyInsertedPwd(insPwd))
                this._mvm.IsUserLoggedIn = true;
            else 
                this.ShowMessage("Error","Inserted password is not correct. Please, try again.", Icon.Error);
        } catch(PersExc e)
        {
            this.ShowMessage("Error", e.Message, Icon.Error);
        }

    }

    public void Logout()
    {
        // TODO: use this._mvm to update _isUserLoggedIn in order to change the menu sidebar
        
        this._mvm.IsUserLoggedIn = false;
        //throw new NotImplementedException();
    }

    public void CreateVault(string mstPwd)
    {
        /*
        *   Reminder: 
        *   this method needs to check wheter a vault already exists or not and create 
        *   a new vault ONLY in the second case
        */
        throw new NotImplementedException();
    }

    public void ShowMessage(string title, string msg,Icon i = Icon.None, ButtonEnum b = ButtonEnum.Ok)
    {
        MessageBoxManager.GetMessageBoxStandard(title, msg, b, i).ShowAsync();
    }

    #endregion

    public MainViewModel Mvm {set => this._mvm = value;}
}
