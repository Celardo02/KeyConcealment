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
        this.Id = id;
        this.Usr = null;
        this.Mail = mail;
        this.Pwd = pwd;
    }

    public Credential(string id, string mail, string usr, string pwd)
    {
        this.Id = id;
        this.Usr = usr;
        this.Mail = mail;
        this.Pwd = pwd;
    }

    public Credential(Credential c)
    {
        this.Id = c.Id;
        this.Usr = c.Usr;
        this.Mail = c.Mail;
        this._pwd = c.Pwd;
        this.Exp = c.Exp;
    }
    #endregion 

    #region getters and setters
    public string Id {get => _id; set => _id = value;}
    public string? Usr {get => _usr; set => _usr = value;}
    public string Mail {get => _mail; set => _mail = value;}
    public string Pwd { get => _pwd; 
                        set 
                            {
                                _pwd = value;
                                this._exp = DateTime.UtcNow;
                                this._exp = this._exp.AddMonths(EXP_TIME);
                            }
                      }
    public DateTime Exp {get => _exp; set => _exp = value;}
    #endregion

    public bool IsComplete()
    {
        return !string.IsNullOrEmpty(this._id) && !string.IsNullOrEmpty(this._pwd) && !string.IsNullOrEmpty(this._mail);
    }
}
