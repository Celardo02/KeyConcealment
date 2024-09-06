using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyConcealment.Domain;

namespace KeyConcealment.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // allows to show different GUIs inside the main view
    [ObservableProperty]
    private ViewModelBase _currentPag;

    private readonly ObservableCollection<TemplateMenuObj> _templates; 

    // allows to know it the side menu is opened or closed
    [ObservableProperty]
    private bool _isMenuOp;

    // keeps track of the selected object in the side menu
    [ObservableProperty]
    private TemplateMenuObj? _selObj;

    public MainViewModel()
    {
        _currentPag = new LoginViewModel();
        _isMenuOp = false;
        _templates = new ObservableCollection<TemplateMenuObj>(
            [
                new TemplateMenuObj(typeof(LoginViewModel), "PersonAccountsRegular", "Login"),
                new TemplateMenuObj(typeof(MasterViewModel), "KeyRegular", "Master password"),
                new TemplateMenuObj(typeof(CredsViewModel), "PasswordRegular", "Credential sets"),
                new TemplateMenuObj(typeof(SyncViewModel), "arrowSyncRegular", "Data syncing"),
                
            ]);
        _selObj = Templates.First(vm => vm.Model == typeof(LoginViewModel));
    }

    partial void OnSelObjChanged(TemplateMenuObj? value)
    {
        if (value is null) return;

        var viewMod = Activator.CreateInstance(value.Model);

        if (viewMod is not ViewModelBase viewModBase) return;

        this.CurrentPag = viewModBase;
    }

    [RelayCommand]
    private void ToggleMenu()
    {
        IsMenuOp = !IsMenuOp;
    }

    public ObservableCollection<TemplateMenuObj> Templates { get => _templates; }
}
