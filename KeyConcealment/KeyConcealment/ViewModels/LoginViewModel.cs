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

    private ILoginManager _lm;
    private IVaultManager _vm;
    #endregion

    #region constructors
    public LoginViewModel()
    {
        this._lm = LoginHandler.Instance;
        this._vm = VaultHandler.Instance;
    }

    public LoginViewModel(LoginHandler lm, VaultHandler vm)
    {
        this._lm = lm;
        this._vm = vm;
    }
    #endregion 

    #region relay commands
    [RelayCommand(CanExecute = nameof(this.IsPasswordInserted))]
    private void Login()
    {
        this._lm.Login(InsPwd);
        this.InsPwd = "";
    }

    [RelayCommand(CanExecute = nameof(this.IsNewPasswordInserted))]
    private void CreateVault()
    {
        this._vm.CreateVault(this.NewPwd);
        this.NewPwd = "";
    }

    [RelayCommand]
    private void ResetVault()
    {
        this._vm.ResetVault();
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
