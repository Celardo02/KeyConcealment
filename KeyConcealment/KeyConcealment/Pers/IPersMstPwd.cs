using System;
using System.Security.Cryptography;
using KeyConcealment.Domain;

namespace KeyConcealment.Pers;

/// <summary>
/// Interface of a persistence class that handles IMasterPwd Domain type objects
/// </summary>
public interface IPersMstPwd
{
    /// <summary>
    /// Compares <c>insPwd</c> with the master password
    /// </summary>
    /// <param name="insPwd">Password inserted by the user</param>
    /// <returns>Returns <c>true</c> if the inserted password correspond to the 
    /// master password, <c>false</c> otherwise
    /// </returns>
    /// <exception cref="PersExc">
    /// Throws a <c>PersExc</c> if the master password does NOT exist yet
    /// </exception>
    bool VerifyInsertedPwd(string insPwd);

    /// <summary>
    /// Checks if the master password is expired or not
    /// </summary>
    /// <returns>Returns <c>true</c> if the master password is expired, 
    /// <c>false</c> otherwise
    /// </returns>
    /// <exception cref="PersExc">
    /// Throws a <c>PersExc</c> if the master password does NOT exist yet
    /// </exception>
    bool CheckMasterPwdExp();

    /// <summary>
    /// Sets the new master password
    /// </summary>
    /// <param name="newPwd">new value for the master password</param>
    /// <exception cref="PersExcDupl">
    /// Throws a <c>PersExcDupl</c> if the new password is equal to the old one
    /// </exception>
    /// <exception cref="PersExc">
    /// Throws a <c>PersExc</c> if the new password does not meet one or more  
    /// security constraint
    /// </exception>
    void SetNewMasterPwd(string newPwd);

    // setter is realised with another method because it has to take a string as 
    // input instead of an IMasterPwd? object
    IMasterPwd? MPwd {get;}
}
