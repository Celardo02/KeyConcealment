using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyConcealment.Domain;
using KeyConcealment.Service;

namespace KeyConcealment.ViewModels;

public partial class CredsViewModel : ViewModelBase
{
    #region attributes
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ShowCredentialSetsCommand))]
    private ObservableCollection<ICred<string>> _creds;

    [ObservableProperty]
    private ICred<string> _selRow;

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

    private IService _s;
    #endregion 

    #region constructors
    public CredsViewModel()
    {
        this._s = Handler.Instance;
        this.Creds = new ObservableCollection<ICred<string>>();
        this.Init();
    }

    public CredsViewModel(IService s)
    {
        this._s = s;
        this.Creds = new ObservableCollection<ICred<string>>();
        this.Init();
    }

    private void Init()
    {
        

        // TESTING ONLY. REMOVE THIS ENTIRE METHOD
        Creds.Add(new Credentials("test1","pwd1","","","","mail1","usr1"));
        Creds.Add(new Credentials("test2","pwd2","","","","mail2","usr2"));
        Creds.Add(new Credentials("test3","pwd3","","","","mail3","usr3"));
        Creds.Add(new Credentials("test4","pwd4","","","","mail4","usr4"));
        Creds.Add(new Credentials("test5","pwd5","","","","mail5","usr5"));
        // **************************

    }
    #endregion

    #region relay commands

    [RelayCommand]
    private void ShowCredentialSets()
    {
        throw new NotImplementedException();
    }

    [RelayCommand(CanExecute = nameof(this.CheckNewCredsData))]
    private void AddCredentials()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void PasswordCopy(ICred<string> c)
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void PasswordInfo(ICred<string> c)
    {
        throw new NotImplementedException();
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
        return !string.IsNullOrEmpty(this.NewId) && !string.IsNullOrEmpty(this.NewUsr) && !string.IsNullOrEmpty(this.NewMail) && !string.IsNullOrEmpty(this.NewPwd);
    }

}
