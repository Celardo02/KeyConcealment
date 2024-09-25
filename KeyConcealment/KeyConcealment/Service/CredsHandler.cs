using System;
using KeyConcealment.Cryptography;
using KeyConcealment.Domain;
using KeyConcealment.Pers;

namespace KeyConcealment.Service;

public class CredsHandler : ICredsManager
{
    #region attributes
    private readonly IPersMstPwd _persMastPwd;
    private readonly IPersCred<string,ICred<string>> _persCreds;
    private readonly ICrypto _crypto;
    #endregion


    #region singleton
    private static CredsHandler? _instance = null;
    private static object _mutex = new object();

    private CredsHandler()
    {
        this._persMastPwd = PersMstPwd.Instance;
        this._persCreds = PersCred.Instance;
        this._crypto = Crypto.Instance;
    }

    public static CredsHandler Instance {
        get
        {
            lock(_mutex)
            {
                _instance ??= new CredsHandler();

                return _instance;
            }
        }
    }

    #endregion

    public void AddCredentials(string masterPwd, string id, string usr, string mail, string speChars, int pwdLength)
    {
        throw new NotImplementedException();
    }

    public void AddCredentials(string masterPwd, string id, string usr, string mail, string pwd)
    {
        throw new NotImplementedException();
    }

}
