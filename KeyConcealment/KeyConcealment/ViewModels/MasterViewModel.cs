using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyConcealment.Service;

namespace KeyConcealment.ViewModels;

public partial class MasterViewModel : ViewModelBase
{

    #region attributes
    [ObservableProperty]
    private string _exp;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangePwdCommand))]
    private string? _newPwd;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangePwdCommand))]
    private string? _oldPwd;

    private IService _s;
    #endregion 

    #region Constructors
    public MasterViewModel()
    {
        this._s = Handler.Instance;
        this._exp = this._s.GetMasterPwdExp();
    }

    public MasterViewModel(IService s)
    {
        this._s = s;
        this._exp = this._s.GetMasterPwdExp();
    }
    #endregion 

    [RelayCommand(CanExecute = nameof(ArePwdsInserted))]
    private void ChangePwd()
    {
        this._s.SetMasterPwd(this.OldPwd,this.NewPwd);

        this.Exp = this._s.GetMasterPwdExp(); 
    }

    /// <summary>
    /// Cheks wheter old master password and new master password were inserted or not
    /// </summary>
    /// <returns>
    /// Returns <c>true</c> if old master password and new master password were typed in; 
    /// <c>false</c> otherwise
    /// </returns>
    private bool ArePwdsInserted()
    {
        return !string.IsNullOrEmpty(NewPwd) && !string.IsNullOrEmpty(OldPwd);
    }

}
