using KeyConcealment.ViewModels;

namespace KeyConcealment.Service;

/// <summary>
/// Interface for a service class that provides login logic
/// </summary>
public interface ILoginManager
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
    /// Sets the MainViewModel instance linked to the Service class
    /// </summary>
    public MainViewModel Mvm{set;}

}
