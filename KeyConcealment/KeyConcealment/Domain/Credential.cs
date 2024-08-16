using System;

namespace KeyConcealment.Domain;

public class Credential : ICred<string>
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

    public Credential(ICred<string> c)
    {
        this.Id = c.Id;
        this.Usr = c.Usr;
        this.Mail = c.Mail;
        this._pwd = c.Pwd;
        this._exp = c.Exp;
    }
    #endregion 

    #region getters and setters
    public string Id {get => this._id; set => this._id = value;}
    public string? Usr {get => this._usr; set => this._usr = value;}
    public string Mail {get => this._mail; set => this._mail = value;}
    public string Pwd { get => this._pwd; 
                        set 
                            {
                                this._pwd = value;
                                this._exp = DateTime.Today;
                                this._exp = this._exp.AddMonths(EXP_TIME);
                            }
                      }
    public DateTime Exp {get => this._exp;}
    #endregion
}
