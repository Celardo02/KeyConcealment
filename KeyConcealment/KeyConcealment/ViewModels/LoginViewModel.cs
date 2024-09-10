using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyConcealment.Service;

namespace KeyConcealment.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    #region  attributes
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string? _insPwd;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateVaultCommand))]
    private string? _newPwd;

    private IService _s;
    #endregion

    #region constructors
    public LoginViewModel()
    {
        this._s = Handler.Instance;
    }

    public LoginViewModel(IService s)
    {
        this._s = s;
    }
    #endregion 

    #region relay commands
    [RelayCommand(CanExecute = nameof(this.IsPasswordInserted))]
    private void Login()
    {
        this._s.Login(InsPwd);
    }

    [RelayCommand(CanExecute = nameof(this.IsNewPasswordInserted))]
    private void CreateVault()
    {
        this._s.CreateVault(this.NewPwd);
    }

    [RelayCommand]
    private void ResetVault()
    {
        this._s.ResetVault();
    }
    #endregion

    private bool IsPasswordInserted()
    {
        return !string.IsNullOrEmpty(this.InsPwd);
    }

    private bool IsNewPasswordInserted()
    {
        return !string.IsNullOrEmpty(this.NewPwd);
    }

}
