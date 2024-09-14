using System;
using System.Security.Cryptography;

namespace KeyConcealment.Domain;

public class MasterPwd : IMasterPwd
{
    #region attributes
    private string _hash;
    private DateTime _exp;
    // expiration time expressed in months
    private const ushort EXP_TIME = 3; 
    private string _salt;
    #endregion

    #region constructors
    public MasterPwd(string hash, string salt)
    {
        this.Hash = hash;
        this.Salt = salt;
    }

    public MasterPwd(string hash, string salt, DateTime exp)
    {
        this._hash = hash;
        this._exp = exp;
        this.Salt = salt;
    }
    public MasterPwd(IMasterPwd mp)
    {
        this._hash = mp.Hash;
        this._exp = mp.Exp;
        this.Salt = mp.Salt;
    }
    #endregion
    public string Hash { get => this._hash; 
                        set
                            {
                                this._hash = value;
                                this._exp = DateTime.Today;
                                this._exp = this._exp.AddMonths(EXP_TIME);
                            } 
                        }
    public string Salt{get => this._salt; set => this._salt = value;}
    public DateTime Exp { get => this._exp; set => this._exp = value;}

    public override string ToString()
    {
        return this.Hash + "," + this.Salt + "," + this.Exp.ToString("dd/MM/yyyy");
    }
}
