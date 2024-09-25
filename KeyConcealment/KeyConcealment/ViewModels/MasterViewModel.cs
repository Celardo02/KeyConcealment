using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyConcealment.Service;
using MsBox.Avalonia.Enums;

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
    private string? _confNewPwd;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangePwdCommand))]
    private string? _oldPwd;

    private IVaultManager _vm;
    #endregion 

    #region Constructors
    public MasterViewModel()
    {
        this._vm = VaultHandler.Instance;
        this._exp = this._vm.GetMasterPwdExp();
    }

    public MasterViewModel(IVaultManager vm)
    {
        this._vm = vm;
        this._exp = this._vm.GetMasterPwdExp();
    }
    #endregion 

    [RelayCommand(CanExecute = nameof(ArePwdsInserted))]
    private void ChangePwd()
    {
        if(this.NewPwd.Equals(this.ConfNewPwd))
        {
            this._vm.SetMasterPwd(this.OldPwd,this.NewPwd);
            this.Exp = this._vm.GetMasterPwdExp(); 
        }
        else 
            PopUpMessageHandler.ShowMessage("Error","New master password and confirmation password do not match.", Icon.Error, ButtonEnum.Ok);

        this.OldPwd = "";
        this.NewPwd = "";
        this.ConfNewPwd = "";
    }

    /// <summary>
    /// Cheks whether all three password text boxes were filled or not
    /// </summary>
    /// <returns>
    /// Returns <c>true</c> if all three text boxes were filled; 
    /// <c>false</c> otherwise
    /// </returns>
    private bool ArePwdsInserted()
    {
        return !string.IsNullOrEmpty(this.NewPwd) && !string.IsNullOrEmpty(this.OldPwd) && !string.IsNullOrEmpty(this.ConfNewPwd);
    }

}
