using System;
using System.Collections.Generic;
using KeyConcealment.Domain;

namespace KeyConcealment.Pers;

public interface IPers<K,V> where V : IDom<K>
{
    /// <summary>
    /// Inserts a new object inside the persitence class
    /// </summary>
    /// <param name="obj">object to be inserted</param>
    /// <exception cref="PersExcDupl">
    /// Throws a <c>PersExcDupl</c> exception if <c>obj</c> already exists
    /// </exception>
    /// <exception cref="PersExc">
    /// Throws a <c>PersExc</c> exception if <c>obj</c> does not meet the requirements 
    /// imposed by <c>IsComplete</c> domain method
    /// </exception>
    /// <exception cref="FormatException">
    /// Throws a <c>FormatException</c> exception if <c>obj</c> hasn't got an email as 
    /// id
    /// </exception>
    void Create(V obj);

    /// <summary>
    /// Looks up for an object inside the persitence class
    /// </summary>
    /// <param name="id">Id of the searched object</param>
    /// <returns>Returns the required object</returns>
    /// <exception cref="PersExcNotFound">
    /// Throws a <c>PersExcNotFound</c> exception if an object with <c>id</c> as 
    /// identifier does not exist
    /// </exception>
    V Read(K id);

    /// <summary>
    /// Updates attributes values of an object
    /// </summary>
    /// <param name="id">Id of the object to be updated</param>
    /// <param name="obj">object to be updated</param>
    /// <exception cref="PersExcNotFound">
    /// Throws a <c>PersExcNotFound</c> exception if an object with <c>id</c> as 
    /// identifier does not exist
    /// </exception>
    /// <exception cref="FormatException">
    /// Throws a <c>FormatException</c> exception if <c>obj</c> hasn't got an email as 
    /// id
    /// </exception>
    void Update(K id, V obj);

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
    /// Lists all the objects inside the persistence class
    /// </summary>
    /// <returns>Returns a <c>List</c> with all the object inside the presistence class</returns>
    /// <exception cref="PersExc">
    /// Throws a <c>PersExc</c> exception if the persistence is empty
    /// </exception>
    List<V> ListAll();

    /// <summary>
    /// Saves the current persistence content inside a file
    /// </summary>
    /// <param name="path">path where to save the file</param>
    void Save(string path);

    /// <summary>
    /// Load the persistence content from a file 
    /// </summary>
    /// <param name="path">path where to find the file</param>
    void Load(string path);
}
