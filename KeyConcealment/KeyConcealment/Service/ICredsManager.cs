namespace KeyConcealment.Service;

/// <summary>
/// Interface for a service class that provides credential sets handling logic
/// </summary>
public interface ICredsManager
{
    /// <summary>
    /// Adds new credential set inside the vault generating the password
    /// </summary>
    /// <param name="masterPwd">master password</param>
    /// <param name="id">Credentials id</param>
    /// <param name="usr">Username</param>
    /// <param name="mail">E-mail</param>
    /// <param name="speChars">Special characters to use inside the auto generated password</param>
    /// <param name="pwdLength">Length of the auto generated password</param>
    void AddCredentials(string masterPwd, string id, string usr, string mail, string speChars, int pwdLength);

    /// <summary>
    /// Adds new credential set inside the vault 
    /// </summary>
    /// <param name="masterPwd">master password</param>
    /// <param name="id">Credentials id</param>
    /// <param name="usr">Username</param>
    /// <param name="mail">E-mail</param>
    /// <param name="pwd">Password</param>
    void AddCredentials(string masterPwd, string id, string usr, string mail, string pwd);

    /// <summary>
    /// Regenerates the password of a credential set
    /// </summary>
    /// <param name="masterPwd">master password of the vault</param>
    /// <param name="id">id of the credential set</param>
    void RegeneratePassword(string masterPwd, string id);

}
