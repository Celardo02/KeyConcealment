using System;
using System.Security.Cryptography;

namespace KeyConcealment.Domain;

public class MasterPwd : IMasterPwd
{
    #region attributes
    private byte[] _hash;
    private DateTime _exp;
    // expiration time expressed in months
    private const ushort EXP_TIME = 3; 
    private byte[] _salt;
    #endregion

    #region constructors
    public MasterPwd(byte[] hash, byte[] salt)
    {
        this.Hash = hash;
        this.Salt = salt;
    }
    public MasterPwd(IMasterPwd mp)
    {
        this._hash = mp.Hash;
        this._exp = mp.Exp;
        this._salt = mp.Salt;
    }
    #endregion
    public byte[] Hash { get => this._hash; 
                        set
                            {
                                this._hash = value;
                                this._exp = DateTime.Today;
                                this._exp = this._exp.AddMonths(EXP_TIME);
                            } 
                        }
    public byte[] Salt{get => this._salt; set => this._salt = value;}
    public DateTime Exp { get => this._exp;}
}
