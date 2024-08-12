using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using KeyConcealment.Domain;

namespace KeyConcealment.Pers;

public class Pers : IPers<string, Credential>
{
    private Dictionary<string, Credential> _creds;
    private readonly string _pattern;

    #region singleton
    private static Pers? _instance;
    private static object _mutex = new object();

    private Pers()
    {
        this._creds = new Dictionary<string, Credential>();
        // the goal is to have an e-mail (you must have @ symbol and a dot after that), 
        // allowing any character beside white space. This constraint is not tight at all 
        // to allow any kind of format used by a service
        this._pattern = @"^[\S]+@[\S]+\.[\S]+$";
    }

    public static Pers Instance
    {
        get 
        {
            lock(_mutex)
            {
                if(_instance == null)
                    _instance = new Pers();

                return _instance;
            }
        }
    }
    #endregion

    #region IPers methods
    public void Create(Credential obj)
    {
        if(this._creds.ContainsKey(obj.Id))
            throw new PersExcDupl("Id " + obj.Id + " already exists. Please, choose a unique Id");
        
        if(!obj.IsComplete())
            throw new PersExc("Passed credential is incomplete");
        
        // checking if obj.Id is an e-mail
        if(!Regex.Match(obj.Id, this._pattern).Success)
            throw new FormatException("Id " + obj.Id + " is not a valid e-mail address");

        this._creds.Add(obj.Id,obj);
    }

    public void Delete(string id)
    {
        if(!this._creds.ContainsKey(id))
            throw new PersExcNotFound("Credential with " + id + " as Id does not exist. Please, insert a valid Id");

        this._creds.Remove(id);
    }

    public List<Credential> ListAll()
    {
        if(this._creds.Count == 0)
            throw new PersExc("Persistence is empty");

        return new List<Credential>(this._creds.Values.ToList());
    }

    public void Load(string path)
    {
        throw new NotImplementedException();
    }

    public Credential Read(string id)
    {
        if(!this._creds.ContainsKey(id))
            throw new PersExcNotFound("Credential with " + id + " as Id does not exist. Please, insert a valid Id");
        
        return new Credential(this._creds[id]);
    }

    public void Save(string path)
    {
        throw new NotImplementedException();
    }

    public void Update(string id, Credential obj)
    {
        if(!this._creds.ContainsKey(id))
            throw new PersExcNotFound("Credential with " + id + " as Id does not exist. Please, insert a valid Id");

        // checking if obj.Id is an e-mail
        if(!Regex.Match(obj.Id, this._pattern).Success)
            throw new FormatException("Id " + obj.Id + " is not a valid e-mail address");

        // checking if the id has been modified
        if(id == obj.Id)
            this._creds[id] = obj;
        else 
        {
            this._creds.Remove(id);
            this._creds.Add(obj.Id, obj);
        }
    }

    #endregion
}
