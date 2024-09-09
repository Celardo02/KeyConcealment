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

    [RelayCommand(CanExecute = nameof(this.IsPasswordInserted))]
    private void Login()
    {
        this._s.Login(InsPwd);
    }

    private bool IsPasswordInserted()
    {
        return !string.IsNullOrEmpty(InsPwd);
    }

}
