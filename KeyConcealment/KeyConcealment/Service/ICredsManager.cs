using System.Collections.Generic;
using KeyConcealment.Domain;

namespace KeyConcealment.Service;

/// <summary>
/// Interface for a service class that provides credential sets handling logic
/// </summary>
public interface ICredsManager
{
    /// <summary>
    /// Gets all credential sets inside the vault
    /// </summary>
    /// <returns>
    /// Returns a <c>List<ICred<string></c> object that contains all credential sets 
    /// stored inside the vault
    /// </returns>
    List<ICred<string>> GetAllCredentials();


    /// <summary>
    /// Adds new credential set inside the vault generating the password
    /// </summary>
    /// <param name="masterPwd">master password</param>
    /// <param name="id">Credentials id</param>
    /// <param name="usr">Username</param>
    /// <param name="mail">E-mail</param>
    /// <param name="specChars">Special characters to use inside the auto generated password</param>
    /// <param name="pwdLength">Length of the auto generated password</param>
    void AddCredentials(string masterPwd, string id, string usr, string mail, string specChars, int pwdLength);

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
    /// Copies a credential set password inside device clipboard and wait for clean it 
    /// for 15 seconds
    /// </summary>
    /// <param name="masterPwd">master password of the vault</param>
    /// <param name="id">id of the credential set</param>
    void PasswordCopy(string masterPwd, string id);

    /// <summary>
    /// Shows credential set info to the user. Shown pieces of information are:
    /// <list type="bullet">
    /// <item><description>Plain text password</description></item>
    /// <item><description>Expiration date of the password</description></item>
    /// </list>
    /// </summary>
    /// <param name="masterPwd">master password of the vault</param>
    /// <param name="id">id of the credential set</param>
    void PasswordInfo(string masterPwd, string id);

    /// <summary>
    /// Regenerates the password of a credential set
    /// </summary>
    /// <param name="masterPwd">master password of the vault</param>
    /// <param name="id">id of the credential set</param>
    void RegeneratePassword(string masterPwd, string id);

    /// <summary>
    /// Saves changes to a credential set
    /// </summary>
    /// <param name="masterPwd">master password of the vault</param>
    /// <param name="c">credential set object</param>
    void SaveChanges(string masterPwd, ICred<string> c);

}
