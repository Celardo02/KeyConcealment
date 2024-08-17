using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia.Controls.Converters;
using KeyConcealment.Domain;

namespace KeyConcealment.Pers;

public class PersMstPwd : IPersMstPwd
{
    #region attributes
    // sha calculator class
    private SHA512 _sha;
    // random number generator class
    private RandomNumberGenerator _rng;
    // master password domain class
    private IMasterPwd? _mPwd;
    // set which contains all previously used salt values as string
    private SortedSet<string> _oldSalts;
    // lenght of the byte array used for the salt
    private const ushort SALT_LEN = 64; 
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
        this._sha = SHA512.Create();
        this._rng = RandomNumberGenerator.Create();
        this._mPwd = null;
    }

    public PersMstPwd Instance
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
        byte[] insPwdHash;
        // checking if a master password exists yet
        if(this._mPwd == null)
            throw new PersExc("Master password does not exist yet. Please, create a new master password");
        
        // calculating the hash of insPwd concatenated with master password salt
        insPwdHash = this._sha.ComputeHash(this.CreateSaltedPwd(Encoding.UTF8.GetBytes(insPwd),this._mPwd.Salt));
        
        // comparing insPwdHash to the master password salted hash
        return this._mPwd.Hash.SequenceEqual(insPwdHash);
    }

    public bool CheckMasterPwdExp()
    {
        if(this._mPwd == null)
            throw new PersExc("Master password does not exist yet. Please, create a new master password");

        return DateTime.Today > this._mPwd.Exp;
    }

    public IMasterPwd? MPwd {get => this._mPwd;}

    public void SetNewMasterPwd(string newPwd)
    {
        // byte array that will contain the new salt value
        byte[] salt = new byte[SALT_LEN];
        
        // checking if the new typed password is equal to the old one only if a master 
        // password already exists
        if( this._mPwd != null && 
            this.VerifyInsertedPwd(newPwd))
                throw new PersExcDupl("Inserted password is equal to the previous one. Please, type a different password");

        // checking all security constraints
        if(!CheckPwdReq(newPwd))
            throw new PersExc("Insert password is too weak. A valid password must contain at least: \n\t - " 
            + LEN + " total characters;\n\t - " 
            + SPEC_CHAR + " special character(s);\n\t - "
            + NUMS + " number(s);\n\t - "
            + UP_CASE + " upper case character(s); \n\t - "
            + LOW_CASE + " lower case character(s).");
        

        // keeps regenerating new values until one has never been used
        do
            this._rng.GetNonZeroBytes(salt);
        while(this._oldSalts.Contains(Encoding.UTF8.GetString(salt)));

        // adding salt value to those generated previously
        this._oldSalts.Add(Encoding.UTF8.GetString(salt));

        // creating new master password object
        this._mPwd = new MasterPwd(this._sha.ComputeHash(this.CreateSaltedPwd(Encoding.UTF8.GetBytes(newPwd),salt)),salt);
             
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

    /// <summary>
    /// Creates a byte array that contains password bytes concatenated with salt bytes
    /// </summary>
    /// <param name="pwd">byte array containing password value</param>
    /// <param name="salt">byte array containing salt value</param>
    /// <returns>
    /// Returns a new byte array which contains <c>pwd</c> concatenated with 
    /// <c>salt</c> in this order 
    /// </returns>
    private byte[] CreateSaltedPwd(byte[] pwd, byte[] salt)
    {
        // temporary variable used to contain the new password concatenated with a 
        // freshly generated salt
        byte[] saltedPwd = new byte[pwd.Length+salt.Length];

        // copying pwd byte array in saltedPwd
        Buffer.BlockCopy(pwd,0,saltedPwd,0,pwd.Length);
        // copying salt byte array in saltedPwd
        Buffer.BlockCopy(salt,0,saltedPwd,0,salt.Length);

        return saltedPwd;
    }
}
