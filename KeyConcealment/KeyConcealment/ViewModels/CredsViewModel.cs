using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyConcealment.Domain;
using KeyConcealment.Service;
using MsBox.Avalonia.Enums;

namespace KeyConcealment.ViewModels;

public partial class CredsViewModel : ViewModelBase
{
    #region attributes
    [ObservableProperty]
    private ObservableCollection<ICred<string>> _creds;

    // attributes to store new credentials data
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCredentialsCommand))]
    private string _newId;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCredentialsCommand))]
    private string _newUsr;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCredentialsCommand))]
    private string _newMail;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCredentialsCommand))]
    private string _newPwd;

    // flag that allow the user to auto generate a new password for a credential set
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCredentialsCommand))]
    private bool _genPwd;

    // length of the auto generate password
    [ObservableProperty]
    private decimal _pwdLen;

    // special characters to be used in the auto generated password
    [ObservableProperty]
    private ObservableCollection<SpecChar> _specChars;

    // minimum auto generated password length
    [ObservableProperty]
    private int _minPwdLen = 15;

    [ObservableProperty]
    private bool _isPopupVisible;

    // master password value typed by the user
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _typedPwd;

    // action selected by the user with buttons on each credential set row
    private Action? _selectedAction;
    // selected credential set
    private ICred<string>? _slectedCred;

    private ICredsManager _cm;


    #endregion

    #region constructors
    public CredsViewModel()
    {
        this._cm = CredsHandler.Instance;
        this.Init();
    }

    public CredsViewModel(ICredsManager cm)
    {
        this._cm = cm;
        this.Init();
    }

    private void Init()
    {
        this.GenPwd = true;
        this.PwdLen = 20;
        this.SpecChars = new ObservableCollection<SpecChar>(
            [
                new SpecChar('-',false),
                new SpecChar('+',false),
                new SpecChar('_',false),
                new SpecChar('&',false),
                new SpecChar('%',false),
                new SpecChar('@',false),
                new SpecChar('$',false),
                new SpecChar('£',false),
                new SpecChar('!',false),
                new SpecChar('#',false)
            ]
        );
        this.IsPopupVisible = false;
        this.TypedPwd = "";
        this._selectedAction = null;
        this.Creds = new ObservableCollection<ICred<string>>();
        List<ICred<string>> credsListAll = this._cm.GetAllCredentials();

        foreach(ICred<string> c in credsListAll)
            this.Creds.Add(c);

    }
    #endregion

    #region relay commands

    [RelayCommand(CanExecute = nameof(this.CheckNewCredsData))]
    private void AddCredentials()
    {
        this._selectedAction = Action.ADD_CRED;
        this.IsPopupVisible = true;
    }

    [RelayCommand]
    private void PasswordCopy(ICred<string> c)
    {
        this._slectedCred = c;
        this._selectedAction = Action.PWD_COPY;
        this.IsPopupVisible = true;
    }

    [RelayCommand]
    private void PasswordInfo(ICred<string> c)
    {
        this._slectedCred = c;
        this._selectedAction = Action.PWD_INFO;
        this.IsPopupVisible = true;
    }

    [RelayCommand]
    private void RegeneratePwd(ICred<string> c)
    {
        this._slectedCred = c;
        this._selectedAction = Action.PWD_REGEN;
        this.IsPopupVisible = true;
    }

    [RelayCommand]
    private void SaveChanges(ICred<string> c)
    {
        this._slectedCred = c;
        this._selectedAction = Action.SAVE_CHANGES;
        this.IsPopupVisible = true;
    }

    [RelayCommand(CanExecute = nameof(CheckTypedPwd))]
    private void Submit()
    {
        switch (this._selectedAction)
        {
            case Action.ADD_CRED:
                // special characters
                string scs = "";

                // adding a credential set with auto generated password
                if (this.GenPwd)
                {
                    foreach (SpecChar sc in this.SpecChars)
                        if (sc.Chosen)
                            scs += sc.SpecialCharacter;

                    this._cm.AddCredentials(this.TypedPwd, this.NewId, this.NewUsr, this.NewMail, scs, Convert.ToInt32(this.PwdLen));

                }
                else // adding a credential set using user typed password
                    this._cm.AddCredentials(this.TypedPwd, this.NewId, this.NewUsr, this.NewMail, this.NewPwd);
                    
                break;
            
            case Action.PWD_COPY:
                this._cm.PasswordCopy(this.TypedPwd, this._slectedCred.Id);
                break;

            case Action.PWD_INFO:
                this._cm.PasswordInfo(this.TypedPwd, this._slectedCred.Id);
                break;

            case Action.PWD_REGEN:
                this._cm.RegeneratePassword(this.TypedPwd, this._slectedCred.Id);
                break;

            case Action.SAVE_CHANGES:
                this._cm.SaveChanges(this.TypedPwd,this._slectedCred);                
                break;

            default:
                PopUpMessageHandler.ShowMessage("Error", "Requested action does not exist.", Icon.Error, ButtonEnum.Ok);
                break;
        }

        this.Cancel();
    }

    [RelayCommand]
    private void Cancel()
    {
        // resetting all attributes involved in user required actions
        this.IsPopupVisible = false;
        this.TypedPwd = "";
        this._selectedAction = null;
        this._slectedCred = null;
    }

    #endregion

    /// <summary>
    /// Cheks if Id, Usr, Mail and Pwd contains something to be used as new credentials
    /// </summary>
    /// <returns>
    /// Returns <c>True</c> if those fields are filled with data; <c>False</c> otherwise
    /// </returns>
    private bool CheckNewCredsData()
    {
        return !string.IsNullOrEmpty(this.NewId) && !string.IsNullOrEmpty(this.NewUsr) && !string.IsNullOrEmpty(this.NewMail) && (this.GenPwd || !string.IsNullOrEmpty(this.NewPwd));
    }

    private bool CheckTypedPwd()
    {
        return !string.IsNullOrEmpty(this.TypedPwd);
    }

}

/// <summary>
/// Enum class that represents which action the user desire between:
/// <list type="bullet">
/// <item><description>Add new credential set</description></item>
/// <item><description>Copy the password of that row</description></item>
/// <item><description>Show every info of the credential set in that row</description></item>
/// <item><description>Save changes on the credential set of that row</description></item>
/// </list>
/// </summary>
public enum Action
{
    ADD_CRED,
    PWD_COPY,
    PWD_INFO,
    PWD_REGEN,
    SAVE_CHANGES
}
