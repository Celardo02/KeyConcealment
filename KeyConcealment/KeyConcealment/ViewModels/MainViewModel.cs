using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyConcealment.Domain;
using KeyConcealment.Service;

namespace KeyConcealment.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region attributes
    // allows to show different GUIs inside the main view
    [ObservableProperty]
    private ViewModelBase _currentPag;

    [ObservableProperty]
    private ObservableCollection<TemplateMenuObj> currentTemplate; 

    private ObservableCollection<TemplateMenuObj> _loginTemplate;

    private ObservableCollection<TemplateMenuObj> _loggedTemplate;

    // allows to know it the side menu is opened or closed
    [ObservableProperty]
    private bool _isMenuOpen;

    // keeps track of the selected object in the side menu
    [ObservableProperty]
    private TemplateMenuObj? _selObj;

    // true if the user logged in; false otherwise
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(this.LogoutCommand))]
    private bool _isUserLoggedIn;

    private readonly ILoginManager _lm;

    #endregion

    #region Constructors

    public MainViewModel(ILoginManager lm)
    {
        this._lm = lm;
        this.Init();
    }

    public MainViewModel()
    {
        this._lm = LoginHandler.Instance;
        this.Init();
    }

    private void Init()
    {
        this._lm.Mvm = this;

        this._loginTemplate = new ObservableCollection<TemplateMenuObj>(
            [
                new TemplateMenuObj(typeof(LoginViewModel), "PersonAccountsRegular", "Login"),
            ]);
        
        this._loggedTemplate = new ObservableCollection<TemplateMenuObj>(
            [
                new TemplateMenuObj(typeof(CredsViewModel), "PasswordRegular", "Credential sets"),
                new TemplateMenuObj(typeof(MasterViewModel), "KeyRegular", "Master password"),
                new TemplateMenuObj(typeof(SyncViewModel), "arrowSyncRegular", "Data syncing"),
            ]);

        this.CurrentPag = new LoginViewModel();
        this.IsMenuOpen = false;
        this.IsUserLoggedIn = false;
        this.CurrentTemplate = this._loginTemplate;
        this.SelObj = CurrentTemplate.First(vm => vm.Model == typeof(LoginViewModel));

    }

    #endregion

    #region Relay command

    [RelayCommand]
    private void ToggleMenu()
    {
        this.IsMenuOpen = !this.IsMenuOpen;
    }

    [RelayCommand(CanExecute = nameof(this.IsUserLoggedIn))]
    private void Logout()
    {
        this._lm.Logout();
    }

    #endregion

    partial void OnSelObjChanged(TemplateMenuObj? value)
    {
        if (value is null) return;

        var viewMod = Activator.CreateInstance(value.Model);

        if (viewMod is not ViewModelBase viewModBase) return;

        this.CurrentPag = viewModBase;
    }

    partial void OnIsUserLoggedInChanged(bool value)
    {
        if(value)
        {
            // shows all password manager functionalities
            this.CurrentTemplate = this._loggedTemplate;
            this.SelObj = this.CurrentTemplate.First(vm => vm.Model == typeof(CredsViewModel));
        }
        else 
        {
            // shows login interface
            this.CurrentTemplate = this._loginTemplate;
            this.SelObj = this.CurrentTemplate.First(vm => vm.Model == typeof(LoginViewModel));
        }
    }

}
