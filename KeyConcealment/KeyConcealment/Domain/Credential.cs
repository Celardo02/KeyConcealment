using System;

namespace KeyConcealment.Domain;

public class Credential : IDom<string>
{
    #region attributes
    // unuique id. It's a label assigned by the user to understand which service is  
    // related to the class instance
    private string _id;
    // username. It is nullable as an account may use the e-mail as username
    private string? _usr;
    // e-mail 
    private string _mail;
    // password 
    private string _pwd;
    // expiration date: date after which the app advise the user to change the password
    private DateTime _exp;
    // expiration time expressed in months
    private const ushort EXP_TIME = 3; 
    #endregion

    #region constructors
    public Credential(string id, string mail, string pwd)
    {
        this._id = id;
        this._mail = mail;
        this._pwd = pwd;
        this._exp = DateTime.UtcNow;
        this._exp = this._exp.AddMonths(EXP_TIME);
    }

    public Credential(string id, string mail, string usr, string pwd)
    {
        this._id = id;
        this._usr = usr; 
        this._mail = mail;
        this._pwd = pwd;
        this._exp = DateTime.UtcNow;
        this._exp = this._exp.AddMonths(EXP_TIME);
    }
    #endregion 

    #region getters and setters
    public string Id {get => _id; set => _id = value;}
    public string? Usr {get => _usr; set => _usr = value;}
    public string Mail {get => _mail; set => _mail = value;}
    public string Pwd {get => _pwd; set => _pwd = value;}
    public DateTime Exp {get => _exp; set => _exp = value;}
    #endregion
}
