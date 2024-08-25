using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KeyConcealment.Cryptography;

public class Crypto : ICrypto
{
    #region attributes
    // class used to calculate hashes and as key derivation function (KDF)
    Rfc2898DeriveBytes? _derBy;
    // AES calculator
    private AesGcm? _aes;
    private RandomNumberGenerator _rng;

    // dictionary used to keep memory of which values have already been used as 
    // nonce/initialization vector with AES encryption/decryption
    private Dictionary<string,string> _oldNonces;
    // dictionary used to keep memory of which values have already been used as 
    // salt in password hashes calculation
    private Dictionary<string,string> _oldSalts;
    #endregion

    #region Constants and readonly attributes

    // nonce size that will be used by AES
    private const ushort NONCE_SIZE = 12;
    // tag size that will be used by AES
    private const ushort TAG_SIZE = 16;

    // the above two constants are public to allow other classes to create byte arrays with 
    // proper length

    // salt length used in hashes calculation
    private const ushort SALT_LEN = 64;
    // Aes password length exressed in bytes
    private const ushort AES_PWD_LEN = 32;

    // OWASP recommended = 210000 with sha512
    private const int  PBKDF2_WORK_FACTOR = 300000;
    private readonly HashAlgorithmName  _PBKDF2HashAlg;

    // side note: I chose to use PBKDF2 algorithm as is the only one officially 
    // supported by Microsoft now (22° August 2024).
    // At the moment of writing, Argon2 and Bcrypt are only supported by non-official 
    // NuGet packages, which is not ideal at all  
    #endregion


    #region singleton
    private static Crypto? _instance = null;
    private static object mutex = new object();

    private Crypto()
    {
        this._rng = RandomNumberGenerator.Create();
        this._oldNonces = new Dictionary<string, string>();
        this._oldSalts = new Dictionary<string, string>();
        // IMPORTANT: 
        // remember to change PBKDF2_WORK_FACTOR value when changing the hash algorithm
        // As a good advice, OWASP provides a cheat sheet for that
        this._PBKDF2HashAlg = HashAlgorithmName.SHA3_512;
    }

    public static Crypto Instance
    {
        get 
        {
            lock(mutex)
            {
                if(_instance == null)
                    _instance = new Crypto();

                return _instance;
            }
        }
    }

    #endregion

    #region ICripto methods
    public string CalculateHash(string input, ref string salt, ushort hashLen = 64)
    {
        byte[] saltByte = new byte[SALT_LEN]; 

        saltByte = this.GenerateNewByteArray(SALT_LEN, this._oldSalts);

        salt = Convert.ToBase64String(saltByte);
        // adding the newly generated salt value to the old ones
        this._oldSalts.Add(salt,salt);

        // creating the object that will provide the hash
        this._derBy = new Rfc2898DeriveBytes(input, saltByte, PBKDF2_WORK_FACTOR, this._PBKDF2HashAlg);

        // returning the hash 
        return Convert.ToBase64String(this._derBy.GetBytes(hashLen));
    }

    public string DecryptAES_GMC(string cyphered, string key, string keySalt, string nonce, string tag)
    {
        byte[] cypheredByte = Convert.FromBase64String(cyphered);
        // byte array that will contain plain text
        byte[] plainByte = new byte[cypheredByte.Length];
        // byte array that will contain Aes nonce value
        byte[] nonceByte = Convert.FromBase64String(nonce);
        // byte array that will contain Aes tag value
        byte[] tagByte = Convert.FromBase64String(tag);

        // checking nonce array size
        if(nonceByte.Length != NONCE_SIZE)
            throw new CryptoArgExc("Nonce array length must be 12 bytes (96 bits)");
        
        // checking tag array size
        if(tagByte.Length != TAG_SIZE)
            throw new CryptoArgExc("Tag array length must be 16 bytes (128 bits)");

        // initializing the KDF algorithm
        this._derBy = new Rfc2898DeriveBytes(key,Convert.FromBase64String(keySalt), PBKDF2_WORK_FACTOR, this._PBKDF2HashAlg);

        this._aes = new AesGcm(this._derBy.GetBytes(AES_PWD_LEN),TAG_SIZE);
        this._aes.Decrypt(nonceByte, cypheredByte, tagByte, plainByte);

        return Encoding.UTF8.GetString(plainByte);
    }

    public string EncryptAES_GMC(string plain, string key, ref string keySalt, ref string nonce, ref string tag)
    {
        // converting plain text in a byte array
        byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
        // creating the array that will contain the cyphered text
        byte[] cypheredBytes = new byte[plainBytes.Length];
        // byte array used to contain salt value use by KDF algorithm
        byte[] saltBytes;
        // byte array that will contain Aes nonce value
        byte[] nonceByte;
        // byte array that will contain Aes tag value
        byte[] tagByte = new byte[TAG_SIZE];

        saltBytes = this.GenerateNewByteArray(SALT_LEN, this._oldSalts);
        keySalt = Convert.ToBase64String(saltBytes);
        // adding the newly generated salt value to the old ones
        this._oldSalts.Add(keySalt, keySalt);


        // initializing the KDF algorithm
        this._derBy = new Rfc2898DeriveBytes(key, saltBytes, PBKDF2_WORK_FACTOR, this._PBKDF2HashAlg); 

        // checking nonce array size
        if(nonce.Length != NONCE_SIZE)
            throw new CryptoArgExc("Nonce array length must be 12 bytes (96 bits)");
        
        // checking tag array size
        if(tag.Length != TAG_SIZE)
            throw new CryptoArgExc("Tag array length must be 16 bytes (128 bits)");

        nonceByte = GenerateNewByteArray(NONCE_SIZE, this._oldNonces);
        nonce = Convert.ToBase64String(nonceByte);
        // adding the newly generated nonce value to the old ones
        this._oldNonces.Add(nonce, nonce);

        // extracting a 256 bits (32 bytes) key with this._derBy.GetBytes(32) 
        this._aes = new AesGcm(this._derBy.GetBytes(AES_PWD_LEN), TAG_SIZE);
        this._aes.Encrypt(nonceByte, plainBytes, cypheredBytes, tagByte);

        return Convert.ToBase64String(cypheredBytes);
    }

    public List<string> OldNonces {get => this._oldNonces.Values.ToList();}

    public List<string> OldSalts {get => this._oldSalts.Values.ToList();}


    public bool VerifyString(string str, string hash, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        byte[] hashByte = Convert.FromBase64String(hash);

        this._derBy = new Rfc2898DeriveBytes(str, saltBytes, PBKDF2_WORK_FACTOR, this._PBKDF2HashAlg);

        return hashByte.SequenceEqual(this._derBy.GetBytes(hash.Length));
    }
    #endregion

    /// <summary>
    /// Generates a fresh value for a byte array avoiding duplicates
    /// </summary>
    /// <param name="len">length of the byte array to be generated</param>
    /// <param name="d">
    /// dictionary which stores all previously generated arrays as base 64 strings
    /// </param>
    /// <returns>Returns the newly generated byte array</returns>
    /// <remarks>
    /// Both the key and the value of each item in <c>d</c> must be the same base 64 
    /// string
    /// </remarks>
    private byte[] GenerateNewByteArray(ushort len, Dictionary<string,string> d)
    {
        byte[] ba = new byte[len];

        // keeps regenerating new values until one has never been used
        do
            this._rng.GetNonZeroBytes(ba);
        while(d.ContainsKey(Convert.ToBase64String(ba)));

        return ba;
    }
}
