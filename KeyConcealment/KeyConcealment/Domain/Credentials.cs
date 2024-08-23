using System;

namespace KeyConcealment.Domain;

public class Credentials : ICred<string>
{
    #region attributes
    private string _id;
    private string? _usr;
    private string? _mail;
    private string _pwd;
    private byte[] _hash;
    private string _salt;
    private DateTime _exp;
    // expiration time expressed in months
    private const ushort EXP_TIME = 3; 
    #endregion

    #region constructors

    public Credentials(string id, string pwd, byte[] hash, string salt, string? mail = null, string? usr = null)
    {
        this.Id = id;
        this.Usr = usr;
        this.Mail = mail;
        this.Pwd = pwd;
        this.Hash = hash;
        this.Salt = salt;
    }

    public Credentials(ICred<string> c)
    {
        this.Id = c.Id;
        this.Usr = c.Usr;
        this.Mail = c.Mail;
        this._pwd = c.Pwd;
        this._exp = c.Exp;
        this.Hash = c.Hash;
        this.Salt = c.Salt;
    }
    #endregion 

    #region getters and setters
    public string Id {get => this._id; set => this._id = value;}
    public string? Usr {get => this._usr; set => this._usr = value;}
    public string? Mail {get => this._mail; set => this._mail = value;}
    public string Pwd { get => this._pwd; 
                        set 
                            {
                                this._pwd = value;
                                this._exp = DateTime.Today;
                                this._exp = this._exp.AddMonths(EXP_TIME);
                            }
                      }
    public byte[] Hash {get => this._hash; set => this._hash = value;}
    public string Salt {get => this._salt; set => this._salt = value;}
    public DateTime Exp {get => this._exp;}
    #endregion
}
