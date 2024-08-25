﻿using System;

namespace KeyConcealment.Domain;

public class Credentials : ICred<string>
{
    #region attributes
    private string _id;
    private string? _usr;
    private string? _mail;
    private string _pwd;
    private byte[] _encNonce;
    private byte[] _encTag;
    private string _encSalt;
    private DateTime _exp;
    // expiration time expressed in months
    private const ushort EXP_TIME = 3; 
    #endregion

    #region constructors

    public Credentials(string id, string pwd, byte[] encNonce, byte[] encTag, string encSalt, string? mail = null, string? usr = null)
    {
        this.Id = id;
        this.Usr = usr;
        this.Mail = mail;
        this.Pwd = pwd;
        this.EncNonce = encNonce;
        this.EncTag = encTag;
        this.EncSalt = encSalt;
    }

    public Credentials(ICred<string> c)
    {
        this.Id = c.Id;
        this.Usr = c.Usr;
        this.Mail = c.Mail;
        this._pwd = c.Pwd;
        this._exp = c.Exp;
        this.EncNonce = c.EncNonce;
        this.EncTag = c.EncTag;
        this.EncSalt = c.EncSalt;
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
    public byte[] EncNonce {get => this._encNonce; set => this._encNonce = value;}
    public byte[] EncTag {get => this._encTag; set => this._encTag = value;}
    public string EncSalt {get => this._encSalt; set => this._encSalt = value;}
    public DateTime Exp {get => this._exp;}
    #endregion
}
