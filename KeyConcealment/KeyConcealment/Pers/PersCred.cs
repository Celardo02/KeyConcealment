using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KeyConcealment.Cryptography;
using KeyConcealment.Domain;

namespace KeyConcealment.Pers;

public class PersCred : IPersCred<string, ICred<string>>
{
    #region attributes
    
    private Dictionary<string, ICred<string>> _credSets;
    private readonly string _pattern;
    private readonly ICrypto _crypt;
    
    #endregion 

    #region singleton
    private static PersCred? _instance = null;
    private static object _mutex = new object();

    private PersCred()
    {
        this._credSets = new Dictionary<string, ICred<string>>();
        // the goal is to have an e-mail (you must have @ symbol and a dot after that), 
        // allowing any character beside white space. This constraint is not tight at all 
        // to allow any kind of format used by a service
        this._pattern = @"^[\S]+@[\S]+\.[\S]+$";
        this._crypt = Crypto.Instance;
    }

    public static PersCred Instance
    {
        get 
        {
            lock(_mutex)
            {
                if(_instance == null)
                    _instance = new PersCred();

                return _instance;
            }
        }
    }
    #endregion

    #region IPersCred methods
    public void Create(ICred<string> creds)
    {
        // checking if credentials already exist
        if(this._credSets.ContainsKey(creds.Id))
            throw new PersExcDupl("Id " + creds.Id + " already exists. Please, choose a unique Id");
        
        if(!IsCredComplete(creds))
            throw new PersExc("Given credentials are incomplete");
        
        // checking if creds.Id is an e-mail
        if(!Regex.Match(creds.Id, this._pattern).Success)
            throw new FormatException("Id " + creds.Id + " is not a valid e-mail address");

        this._credSets.Add(creds.Id,creds);
    }

    public void Delete(string id)
    {
        // checking if the id exists
        if(!this._credSets.ContainsKey(id))
            throw new PersExcNotFound("Credentials with " + id + " as Id do not exist. Please, insert a valid Id");

        this._credSets.Remove(id);
    }

    public void DropContent()
    {
        this._credSets.Clear();
    }

    public List<ICred<string>> ListAll()
    {
        return new List<ICred<string>>(this._credSets.Values.ToList());
    }

    public ICred<string> Read(string id)
    {
        // checking if _credSets contains a credential set using "id" as identifier
        if(!this._credSets.ContainsKey(id))
            throw new PersExcNotFound("Credentials with " + id + " as Id do not exist. Please, insert a valid Id");
        
        return new Credentials(this._credSets[id]);
    }

    public void Update(string id, ICred<string> creds)
    {
        // cheching if a credntial set with "id" as identifier exists
        if(!this._credSets.ContainsKey(id))
            throw new PersExcNotFound("Credentials with " + id + " as Id do not exist. Please, insert a valid Id");

        // checking if creds.Id is an e-mail
        if(!Regex.Match(creds.Id, this._pattern).Success)
            throw new FormatException("Id " + creds.Id + " is not a valid e-mail address");

        // checking if the id has been modified
        if(id == creds.Id)
            this._credSets[id] = creds;
        else 
        {
            // removing old credential set 
            this._credSets.Remove(id);
            // replacing it with the new one with the changed id
            this._credSets.Add(creds.Id, creds);
        }
    }

    public static bool IsCredComplete(ICred<string> creds)
    {
        // creds needs to have id, pwd and mail with not null or empty values
        return !string.IsNullOrEmpty(creds.Id) && !string.IsNullOrEmpty(creds.Pwd) && !string.IsNullOrEmpty(creds.Mail);
    }

    public List<string> CheckCredExpiration()
    {
        List<string> expired = new List<string>();

        foreach(ICred<string> c in this._credSets.Values)
            if(DateTime.Today > c.Exp)
                expired.Add(c.Id);

        return expired;
    }

    public void UpdateCredsEncryption(string oldMasterPwd, string newMasterPwd)
    {
        ICred<string> c;
        string nonce = "";
        string tag = "";
        string salt = "";

        foreach (string key in this._credSets.Keys)
        {
            // getting the credential set corresponding to key
            c = this._credSets[key];

            // updating password encryption with the new key
            c.Pwd = this._crypt.DecryptAES_GMC(c.Pwd,oldMasterPwd,c.EncSalt,c.EncNonce,c.EncTag);
            c.Pwd = this._crypt.EncryptAES_GMC(c.Pwd,newMasterPwd,ref salt, ref nonce, ref tag);
            c.EncSalt = salt;
            c.EncNonce = nonce;
            c.EncTag = tag;

            // storing the updates
            this._credSets[key] = c;
        }
    }
    #endregion
}
