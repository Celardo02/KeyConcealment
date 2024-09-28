﻿using System;
using System.Collections.Generic;
using KeyConcealment.Domain;

namespace KeyConcealment.Pers;

/// <summary>
/// Interface of a persistence class that handles ICred Domain type objects
/// </summary>
/// <typeparam name="K">Id used in the domain class</typeparam>
/// <typeparam name="V">Concrete class that inherits from <c>ICred<K></c></typeparam>
public interface IPersCred<K,V> where V : ICred<K>
{
    /// <summary>
    /// Inserts a new object inside the persitence class
    /// </summary>
    /// <param name="cred">object to be inserted. Credentials password must not be encrypted yet</param>
    /// <param name="masterPwd">master password to encrypt credential set password</param>
    /// <exception cref="PersExcDupl">
    /// Throws a <c>PersExcDupl</c> exception if <c>cred</c> already exists
    /// </exception>
    /// <exception cref="PersExc">
    /// Throws a <c>PersExc</c> exception if <c>cred</c> does not meet the requirements 
    /// imposed by <c>IsCredComplete</c> persistence method
    /// </exception>
    /// <exception cref="FormatException">
    /// Throws a <c>FormatException</c> exception if <c>cred</c> hasn't got an email as 
    /// id
    /// </exception>
    void Create(V cred, string masterPwd);

    /// <summary>
    /// Looks up for an object inside the persitence class
    /// </summary>
    /// <param name="id">id of the searched object</param>
    /// <returns>Returns the required object if exists</returns>
    /// <exception cref="PersExcNotFound">
    /// Throws a <c>PersExcNotFound</c> exception if an object with <c>id</c> as 
    /// identifier does not exist
    /// </exception>
    V Read(K id);

    /// <summary>
    /// Updates attributes values of an object
    /// </summary>
    /// <param name="id">id of the object to be updated. Credential password must not be encrypted yet</param>
    /// <param name="cred">object to be updated</param>
    /// <param name="masterPwd">master password to encrypt credential set password</param>
    /// <exception cref="PersExcNotFound">
    /// Throws a <c>PersExcNotFound</c> exception if an object with <c>id</c> as 
    /// identifier does not exist
    /// </exception>
    /// <exception cref="FormatException">
    /// Throws a <c>FormatException</c> exception if <c>cred</c> hasn't got an email as 
    /// id
    /// </exception>
    void Update(K id, V cred, string masterPwd);

    /// <summary>
    /// Deletes an object from the persistence class
    /// </summary>
    /// <param name="id">Id of the object to be deleted</param>
    /// <exception cref="PersExcNotFound">
    /// Throws a <c>PersExcNotFound</c> exception if an object with <c>id</c> as 
    /// identifier does not exist
    /// </exception>
    void Delete(K id);

    /// <summary>
    /// Deletes whole persistence content
    /// </summary>
    void DropContent();

    /// <summary>
    /// Lists all the objects inside the persistence class
    /// </summary>
    /// <returns>Returns a <c>List</c> with all the object inside the presistence class</returns>
    List<V> ListAll();

    /// <summary>
    /// Checks if <c>cred</c> has the minimum pieces of information required to  
    /// be meaningful
    /// </summary>
    /// <param name="cred">object to be checked</param>
    /// <returns> Returns <c>true</c> if <c>cred</c> has the minimum pieces of information required, <c>false</c> otherwise</returns>
    static abstract bool IsCredComplete(ICred<string> cred);

    /// <summary>
    /// Checks if there are expired credentials inside the persistence layer or not
    /// </summary>
    /// <returns>Returns an empty list if expired credential sets does NOT exist, a list with all 
    /// the expired credentials ids otherwise </returns>
    List<K> CheckCredExpiration();

    /// <summary>
    /// Updates each credential set password encryption using a new master password
    /// </summary>
    /// <param name="newMasterPwd">new master password</param>
    void UpdateCredsEncryption(string oldMasterPwd, string newMasterPwd);
}
