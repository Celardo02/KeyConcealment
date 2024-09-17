using System.Threading.Tasks;
using KeyConcealment.ViewModels;
using MsBox.Avalonia.Enums;

namespace KeyConcealment.Service;

/// <summary>
/// Service class interface. A service class handles all the business logic of the app
/// </summary>
public interface IService
{
    #region login methods
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
    #endregion

    #region vault managing methods
    /// <summary>
    /// Creates and initialize a new vault only if there isn't already an existing one
    /// </summary>
    /// <param name="mstPwd">master password to be used for the vault</param>
    void CreateVault(string mstPwd);

    /// <summary>
    /// Resets all vault content by deleting the encrypted file used to store 
    /// everything
    /// </summary>
    void ResetVault();

    #region master password 
    /// <summary>
    /// Allows to set new master password
    /// </summary>
    /// <param name="oldPwd">old master password</param>
    /// <param name="newPwd">new master password</param>
    public void SetMasterPwd(string oldPwd, string newPwd);

    /// <summary>
    /// Gets master password expiration date
    /// </summary>
    /// <returns>
    /// Returns the expiration date of the master password in dd/MM/yyyy format or 
    /// <c>null</c> if the master password hasn't been set yet
    /// </returns>
    public string? GetMasterPwdExp();
    #endregion
    #endregion

    /// <summary>
    /// Shows a popup with specified message and buttons
    /// </summary>
    /// <param name="title">popup window title</param>
    /// <param name="msg">message to be displayed</param>
    /// <param name="i">icon to be shown</param>
    /// <param name="b">buttons to be shown</param>
    Task<ButtonResult> ShowMessage(string title, string msg, Icon i, ButtonEnum b);

    /// <summary>
    /// Sets the MainViewModel instance linked to the Service class
    /// </summary>
    public MainViewModel Mvm{set;}
}
