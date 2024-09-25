namespace KeyConcealment.Service;

/// <summary>
/// Interface for a service class that provides high level vault management logic
/// </summary>
public interface IVaultManager
{
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

    #region master password handling
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

}
