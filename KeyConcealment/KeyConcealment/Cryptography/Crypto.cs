using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KeyConcealment.Cryptography;

public class Crypto : ICrypto
{
    #region attributes
    // AES encryption class
    private AesGcm? _aes;
    // RSA encryption class
    private RSACryptoServiceProvider? _rsa;
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

    // salt length used in hashes calculation
    private const ushort SALT_LEN = 64;
    // Aes password length exressed in bytes
    private const ushort AES_KEY_LEN = 32;

    /* RSA keys lenght in bits. Length was chosen upon NIST security suggestions 
    *  for RSA algorithms after 2031. Relevant NIST pubblications (each one is the most 
    *  updated revision at the moment of writing)
    *  - https://nvlpubs.nist.gov/nistpubs/SpecialPublications/NIST.SP.800-56Br2.pdf 
    *     section 6.3, table 2
    *  - https://nvlpubs.nist.gov/nistpubs/SpecialPublications/NIST.SP.800-78-5.pdf
    *     section 3.1, table 1
    *  3072 was chosen over 4096 as it represents a good compromise between security 
    *  and performance
    */
    private const ushort RSA_KEYS_LEN = 3072;

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

    #region Hash methods
    public string ComputeHash(string input, ref string salt, ushort hashLen = SALT_LEN)
    {
        byte[] saltByte = new byte[SALT_LEN]; 

        saltByte = this.GenerateNewByteArray(SALT_LEN, this._oldSalts);

        salt = Convert.ToBase64String(saltByte);
        // adding the newly generated salt value to the old ones
        this._oldSalts.Add(salt,salt);

        // calculating an hash "hashLen" long and returning it
        return Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(input, saltByte, PBKDF2_WORK_FACTOR, this._PBKDF2HashAlg, hashLen));
    }

    public bool VerifyHash(string str, string hash, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        byte[] hashByte = Convert.FromBase64String(hash);

        return hashByte.SequenceEqual(Rfc2898DeriveBytes.Pbkdf2(str, saltBytes, PBKDF2_WORK_FACTOR, this._PBKDF2HashAlg, hash.Length));
    }
    #endregion 

    #region AES methods
    public string DecryptAES_GMC(string cyphered, string key, string keySalt, string nonce, string tag)
    {
        // byte array that will contain cyphered text
        byte[] cypheredByte;
        // byte array that will contain plain text
        byte[] plainByte;
        // byte array that will contain Aes nonce value
        byte[] nonceByte = Convert.FromBase64String(nonce);
        // byte array that will contain Aes tag value
        byte[] tagByte;

        // checking nonce array size
        if(nonceByte.Length != NONCE_SIZE)
            throw new CryptoArgExc("Nonce array length must be 12 bytes (96 bits)");
        
        tagByte = Convert.FromBase64String(tag);
        // checking tag array size
        if(tagByte.Length != TAG_SIZE)
            throw new CryptoArgExc("Tag array length must be 16 bytes (128 bits)");

        cypheredByte = Convert.FromBase64String(cyphered);
        plainByte = new byte[cypheredByte.Length];

        this._aes = new AesGcm(Rfc2898DeriveBytes.Pbkdf2(key,Convert.FromBase64String(keySalt), PBKDF2_WORK_FACTOR, this._PBKDF2HashAlg, AES_KEY_LEN),TAG_SIZE);
        this._aes.Decrypt(nonceByte, cypheredByte, tagByte, plainByte);

        this._aes.Dispose();
        return Encoding.UTF8.GetString(plainByte);
    }

    public string EncryptAES_GMC(string plain, string key, ref string keySalt, ref string nonce, ref string tag)
    {
        // byte array that will contain plain text
        byte[] plainBytes;
        // byte array that will contain cyphered text 
        byte[] cypheredBytes;
        // byte array used to contain salt value use by KDF algorithm
        byte[] saltBytes;
        // byte array that will contain Aes nonce value
        byte[] nonceByte;
        // byte array that will contain Aes tag value
        byte[] tagByte;


        // checking nonce array size
        if(nonce.Length != NONCE_SIZE)
            throw new CryptoArgExc("Nonce array length must be 12 bytes (96 bits)");
        
        // checking tag array size
        if(tag.Length != TAG_SIZE)
            throw new CryptoArgExc("Tag array length must be 16 bytes (128 bits)");

        // converting plain text in a byte array
        plainBytes = Encoding.UTF8.GetBytes(plain);
        // creating the array that will contain the cyphered text
        cypheredBytes = new byte[plainBytes.Length];
        tagByte = new byte[TAG_SIZE];

        saltBytes = this.GenerateNewByteArray(SALT_LEN, this._oldSalts);
        keySalt = Convert.ToBase64String(saltBytes);
        // adding the newly generated salt value to the old ones
        this._oldSalts.Add(keySalt, keySalt);

        nonceByte = GenerateNewByteArray(NONCE_SIZE, this._oldNonces);
        nonce = Convert.ToBase64String(nonceByte);
        // adding the newly generated nonce value to the old ones
        this._oldNonces.Add(nonce, nonce);

        // extracting a 256 bits (32 bytes) key with this._derBy.GetBytes(32) 
        this._aes = new AesGcm(Rfc2898DeriveBytes.Pbkdf2(key, saltBytes, PBKDF2_WORK_FACTOR, this._PBKDF2HashAlg, AES_KEY_LEN), TAG_SIZE);
        this._aes.Encrypt(nonceByte, plainBytes, cypheredBytes, tagByte);

        this._aes.Dispose();

        return Convert.ToBase64String(cypheredBytes);
    }
    #endregion

    public string EncryptRSA(string plain, string Mod, string Exp)
    {
        throw new NotImplementedException();
    }

    public string DecryptRSA(string cyphered)
    {
        throw new NotImplementedException();
    }

    public string SignRSA(string data)
    {
        throw new NotImplementedException();
    }

    public bool VerifyRSA(string data, string sign, string Mod, string Exp)
    {
        throw new NotImplementedException();
    }

    public List<string> OldNonces {get => this._oldNonces.Values.ToList();}

    public List<string> OldSalts {get => this._oldSalts.Values.ToList();}


    
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
