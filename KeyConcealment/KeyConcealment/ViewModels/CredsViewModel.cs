using System;
using System.Collections.Generic;
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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCredentialsCommand))]
    private bool _genPwd;

    [ObservableProperty]
    private decimal _pwdLen;

    [ObservableProperty]
    private ObservableCollection<SpecChar> _specChars;

    private IService _s;
    #endregion 

    #region constructors
    public CredsViewModel()
    {
        this._s = Handler.Instance;
        this.Init();
    }

    public CredsViewModel(IService s)
    {
        this._s = s;
        this.Init();
    }

    private void Init()
    {

        this.Creds = new ObservableCollection<ICred<string>>();
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


        // TESTING ONLY. REMOVE FOLLOWING LINES
        Creds.Add(new Credentials("test1", "pwd1", "", "", "", "mail1", "usr1"));
        Creds.Add(new Credentials("test2", "pwd2", "", "", "", "mail2", "usr2"));
        Creds.Add(new Credentials("test3", "pwd3", "", "", "", "mail3", "usr3"));
        Creds.Add(new Credentials("test4", "pwd4", "", "", "", "mail4", "usr4"));
        Creds.Add(new Credentials("test5", "pwd5", "", "", "", "mail5", "usr5"));
        Creds.Add(new Credentials("test1", "pwd1", "", "", "", "mail1", "usr1"));
        Creds.Add(new Credentials("test2", "pwd2", "", "", "", "mail2", "usr2"));
        Creds.Add(new Credentials("test3", "pwd3", "", "", "", "mail3", "usr3"));
        Creds.Add(new Credentials("test4", "pwd4", "", "", "", "mail4", "usr4"));
        Creds.Add(new Credentials("test5", "pwd5", "", "", "", "mail5", "usr5"));
        Creds.Add(new Credentials("test1", "pwd1", "", "", "", "mail1", "usr1"));
        Creds.Add(new Credentials("test2", "pwd2", "", "", "", "mail2", "usr2"));
        Creds.Add(new Credentials("test3", "pwd3", "", "", "", "mail3", "usr3"));
        Creds.Add(new Credentials("test4", "pwd4", "", "", "", "mail4", "usr4"));
        Creds.Add(new Credentials("test5", "pwd5", "", "", "", "mail5", "usr5"));
        // **************************

    }
    #endregion

    #region relay commands

    [RelayCommand(CanExecute = nameof(this.CheckNewCredsData))]
    private void AddCredentials()
    {
        // special characters
        string scs = "";

        // adding credentials generating a password
        if (this.GenPwd)
        {
            foreach(SpecChar sc in this.SpecChars)
                if(sc.Chosen)
                    scs += sc.SpecialCharacter;

            this._s.AddCredentials(this.NewId, this.NewUsr, this.NewMail, scs, Convert.ToInt32(this.PwdLen));

        }
        else // adding credentials using user typed password
            this._s.AddCredentials(this.NewId, this.NewUsr, this.NewMail, this.NewPwd);
    }

    [RelayCommand]
    private void PasswordCopy(ICred<string> c)
    {
        //if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        //{
        //   starTaskWindow.ShowDialog(desktop.MainWindow);
        //}

        throw new NotImplementedException();
    }

    [RelayCommand]
    private void PasswordInfo(ICred<string> c)
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void SaveChanges(ICred<string> c)
    {
        /* REMINDER:
        *  this methd needs to check is current c has any changes before saving new values
        */
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
        return !string.IsNullOrEmpty(this.NewId) && !string.IsNullOrEmpty(this.NewUsr) && !string.IsNullOrEmpty(this.NewMail) && (this.GenPwd || !string.IsNullOrEmpty(this.NewPwd));
    }

}
