using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using KeyConcealment.Cryptography;
using KeyConcealment.Domain;

namespace KeyConcealment.Pers;

public class PersMstPwd : IPersMstPwd
{
    #region attributes
    // master password domain class
    private IMasterPwd? _mPwd;
    // cryptography class
    private ICrypto _crypt;
    #endregion

    #region password requirements constants
    // minimum master password lenght  
    private const byte LEN = 10;
    // minimum number of special character that has to be inside the master password
    private const byte SPEC_CHAR = 1; 
    // minimum number of numbers character that has to be inside the master password
    private const byte NUMS = 1;
    // minimum number of upper case character that has to be inside the master password
    private const byte UP_CASE = 1;
    // minimum number of lower case character that has to be inside the master password
    private const byte LOW_CASE = 1;
    // special characters allowed inside the password
    private const string ALLOWED_SPEC_CHAR = "-+_&@%$£#!";
    #endregion

    #region singleton
    private static PersMstPwd? _instance = null;
    private static object _mutex = new object();

    private PersMstPwd()
    {
        this._mPwd = null;
        this._crypt = Crypto.Instance;
    }

    public static PersMstPwd Instance
    {
        get 
        {
            lock(_mutex)
            {
                if(_instance == null)
                    _instance = new PersMstPwd();
                
                return _instance;
            }
        }
    }
    #endregion

    #region IPersMstPwd methods
    public bool VerifyInsertedPwd(string insPwd)
    {
        // checking if a master password exists yet
        if(this._mPwd == null)
            throw new PersExc("Master password does not exist yet. Please, create a new vault");
        
        // computing insPwd hash and comparing it with this._mPwd.Hash 
        return this._crypt.VerifyHash(insPwd,this._mPwd.Hash, this._mPwd.Salt);
    }

    public bool CheckMasterPwdExp()
    {
        if(this._mPwd == null)
            throw new PersExc("Master password does not exist yet. Please, create a new vault");

        return DateTime.Today > this._mPwd.Exp;
    }

    public IMasterPwd? MPwd {get => this._mPwd;}

    public void SetNewMasterPwd(string newPwd)
    {
        // string that will contain the new salt value
        string salt = "";
        // base 64 string that will contain newPwd hash value
        string newPwdHash;
        
        // checking if the new typed password is equal to the old one only if a master 
        // password already exists
        if( this._mPwd != null && 
            this.VerifyInsertedPwd(newPwd))
                throw new PersExcDupl("Inserted password is equal to the previous one. Please, type a different password");

        // checking all security constraints
        if(!CheckPwdReq(newPwd))
            throw new PersExc("Insert password is too weak. A valid password must contain at least: \n\t - " 
            + LEN + " total characters;\n\t - " 
            + SPEC_CHAR + " special character(s) between : "+ALLOWED_SPEC_CHAR+";\n\t - "
            + NUMS + " number(s);\n\t - "
            + UP_CASE + " upper case character(s); \n\t - "
            + LOW_CASE + " lower case character(s).");

        // calculating the hash and updating salt value
        newPwdHash = this._crypt.ComputeHash(newPwd, ref salt);
        // creating new master password object
        this._mPwd = new MasterPwd(newPwdHash,salt);
             
    }
    #endregion

    /// <summary>
    /// Checks all security requirements that the master password needs to meet
    /// </summary>
    /// <param name="pwd">password to be tested</param>
    /// <returns>
    /// Returns <c>true</c> if the password meets all the requirements, <c>false</c> 
    /// otherwise
    /// </returns>
    private bool CheckPwdReq(string pwd)
    {
        /* pattern of a regular expression that checks if a string contains at 
        * least LOW_CASE lower case, UP_CASE upper case, NUMS numbers, SPEC_CHAR 
        * special characters.
        * The idea is to specify some zero-width positive lookahead assertions (the arguments in 
        * between round parenthesis that starts with "?=") that will be compared with 
        * the regex ^[-a-zA-Z\d+_&@%$£#!]+: this way, we have one or more iteration of 
        * those characters (which are the allowed ones) and this iteration must contain 
        * a certain number of each zero-width lookahead
        */
        string pattern =  @"^(?=.*[a-z]{"
                    + LOW_CASE + @",})(?=.*[A-Z]{"
                    + UP_CASE + @",})(?=.*\d{"
                    + NUMS + @",})(?=.*["+ALLOWED_SPEC_CHAR+@"]{"
                    + SPEC_CHAR + @",})["+ALLOWED_SPEC_CHAR+@"a-zA-Z\d]+$";

        if(pwd.Length < LEN)
            return false;
        
        if(!Regex.Match(pwd,pattern).Success)
            return false;

        return true;
    }
}
