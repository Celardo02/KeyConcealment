using KeyConcealment.ViewModels;
using MsBox.Avalonia.Enums;

namespace KeyConcealment.Service;

/// <summary>
/// Service class interface. A service class handles all the business logic of the app
/// </summary>
public interface IService
{
    /// <summary>
    /// Checks if the inserted password correspond to the master password. If it does, 
    /// all the password manager content becomes avilable
    /// </summary>
    /// <param name="insPwd">password inserted by the user</param>
    void Login(string insPwd);

    /// <summary>
    /// Logs out from the password manager
    /// </summary>
    void Logout();

    /// <summary>
    /// Shows a popup with specified message and buttons
    /// </summary>
    /// <param name="title">popup window title</param>
    /// <param name="msg">message to be displayed</param>
    /// <param name="i">icon to be shown</param>
    /// <param name="b">buttons to be shown</param>
    void ShowMessage(string title, string msg, Icon i, ButtonEnum b);

    /// <summary>
    /// Sets the MainViewModel instance linked to the Service class
    /// </summary>
    public MainViewModel Mvm{set;}
}
