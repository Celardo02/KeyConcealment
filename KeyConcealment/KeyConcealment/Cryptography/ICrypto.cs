using System.Collections.Generic;
using System.Security.Cryptography;
using System;

namespace KeyConcealment.Cryptography;

public interface ICrypto
{
    /// <summary>
    /// Calculates the hash value of the input string
    /// </summary>
    /// <param name="input">string that needs to be hased</param>
    /// <param name="salt">string that will be filled with a salt value (base64 encoded string)</param>
    /// <param name="hashLen">
    /// length of the returned hash expressed in bytes. Default = 64 (512 bits)
    /// </param>
    /// <returns>
    /// Returns a base 64 string containing the password hash
    /// </returns>
    string ComputeHash(string input, ref string salt, ushort hashLen = 64);

    /// <summary>
    /// Checks if a string hash correspond to a previosly computed hash
    /// </summary>
    /// <param name="str">string to be compared with the hash</param>
    /// <param name="hash">base 64 string containing previously computed hash</param>
    /// <param name="salt">
    /// base 64 string representing the salt value used to compute the hash
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if <c>str</c> hash and <c>hash</c> are the same; <c>false</c> otherwise
    /// </returns>
    bool VerifyHash(string str, string hash, string salt);

    /// <summary>
    /// Encrypts a plain text string using a given key. Nonce and tag will be filled 
    /// with values used during the encryptionz 
    /// </summary>
    /// <param name="plain">plain text string</param>
    /// <param name="key">encryption key</param>
    /// <param name="keySalt">
    /// string that will be filled with the salt value used by the encryption algorithm. 
    /// It will be encoded as base 64
    /// </param>
    /// <param name="nonce">base 64 string to be filled with inizialization vector value</param>
    /// <param name="tag">base 64 string to be filled with AES GMC tag value</param>
    /// <returns>
    /// Returns a base 64 string containing <c>plain</c> encrypted
    /// </returns>
    /// <remarks>
    /// Nonce array must be 12 bytes long and tag array must be 16 bytes long
    /// </remarks>
    /// <exception cref="CryptoArgExc">
    /// Throws <c>CryptoArgExc</c> exception if <c>plain</c> is null or empty or 
    /// if at least one between <c>nonce</c> and <c>tag</c> array length does not 
    /// match the right dimension. Respectively, 12 bytes and 16 bytes
    /// </exception>
    /// <exception cref="CryptographicException">
    /// Throws <c>CryptographicException</c> if encryption operations failed
    /// </exception>
    string EncryptAES_GMC(string plain, string key, ref string keySalt, ref string nonce, ref string tag);

    /// <summary>
    /// Decrypts a cyphered base 64 encoded string using a given key
    /// </summary>
    /// <param name="cyphered">cyphered text encoded as base 64 string</param>
    /// <param name="key">decryption key</param>
    /// <param name="keySalt">
    /// base 64 string containing the salt used by the key derivation algorithm during 
    /// encryption
    /// </param>
    /// <param name="nonce">nonce value used in encryption phase</param>
    /// <param name="tag">tag value calculated by encryption phase</param>
    /// <returns>
    /// Returns a UTF-8 encoded string containing the decyphered text
    /// </returns>
    /// <remarks>
    /// Nonce array must be 12 bytes long and tag array must be 16 bytes long
    /// </remarks>
    /// <exception cref="CryptoArgExc">
    /// Throws <c>CryptoArgExc</c> exception if <c>plain</c> is null or empty or 
    /// if at least one between <c>nonce</c> and <c>tag</c> array length does not 
    /// match the right dimension. Respectively, 12 bytes and 16 bytes
    /// </exception>
    /// <exception cref="CryptographicException">
    /// Throws <c>CryptographicException</c> if the tag value could not be verified 
    /// or the decryption operation otherwise failed
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws <c>ArgumentNullException</c> if any argument between <c>cyphered</c>, 
    /// <c>nonce</c>, <c>keySalt</c> or <c>tag</c> is null
    /// </exception>
    string DecryptAES_GMC(string cyphered, string key, string keySalt, string nonce, string tag);

    /// <summary>
    /// Encrypt a string using given RSA public key
    /// </summary>
    /// <param name="plain">plain text to be encrypted</param>
    /// <param name="Mod">base 64 string containing modulus of the RSA public key</param>
    /// <param name="Exp">base 64 string containing public exponent of the RSA public key</param>
    /// <returns>
    /// Returns a base 64 string with the encyphered text
    /// </returns>
    /// <exception cref="CryptographicException">
    /// Throws <c>CryptographicException</c> exception if <c>plain</c> is longer than 
    /// maximum allowed length
    /// </exception>
    /// <exception cref="CryptoArgExc">
    /// Throws <c>CryptoArgExc</c> exception if at least one between <c>plain</c>, 
    /// <c>mod</c> and <c>exp</c> is null or empty
    /// </exception>
    string EncryptRSA(string plain, string mod, string exp);

    /// <summary>
    /// Decrypts a base 64 string containing pieces of information encrypted with 
    /// current device public key
    /// </summary>
    /// <param name="cyphered">cyphered base 64 string</param>
    /// <returns>
    /// Returns a plain text string
    /// </returns>
    /// <exception cref="CryptographicException">
    /// Throws <c>CryptographicException</c> exception if private key does not match the 
    /// encrypted data
    /// </exception>
    /// <exception cref="CryptoArgExc">
    /// Throws <c>CryptoArgExc</c> exception if <c>cyphered</c> is null or empty
    /// </exception>
    /// <exception cref="CryptoExc">
    /// Throws <c>CryptoExc</c> exception if RSA service hasn't been initilized yet
    /// </exception>
    string DecryptRSA(string cyphered);

    /// <summary>
    /// Signs a base 64 string with current device RSA private key
    /// </summary>
    /// <param name="data">base 64 string containing data to be signed</param>
    /// <returns>
    /// Returns a base 64 string containing a signed SHA 512 hash of <c>data</c>
    /// </returns>
    /// <exception cref="CryptoArgExc">
    /// Throws <c>CryptoArgExc</c> exception if <c>data</c> is null or empty
    /// </exception>
    /// <exception cref="CryptoExc">
    /// Throws <c>CryptoExc</c> exception if RSA service hasn't been initilized yet
    /// </exception>
    string SignRSA(string data);

    /// <summary>
    /// Checks whether an RSA digital signature is valid or not
    /// </summary>
    /// <param name="data">base 64 string to be checked against the sign</param>
    /// <param name="sign">base 64 string containing sign of <c>data</c></param>
    /// <param name="Mod">base 64 string containing modulus of the RSA public key</param>
    /// <param name="Exp">base 64 string containing public exponent of the RSA public key</param>
    /// <returns>
    /// Returns <c>True</c> if the signature is valid; otherwise, <c>False</c>
    /// </returns>
    /// <exception cref="CryptoArgExc">
    /// Throws <c>CryptoArgExc</c> exception if at least one between <c>data</c>, 
    /// <c>sign</c>, <c>mod</c> and <c>exp</c> is null or empty
    /// </exception>
    /// <exception cref="CryptoExc">
    /// Throws <c>CryptoExc</c> exception if RSA service hasn't been initilized yet
    /// </exception>
    bool VerifyRSA(string data, string sign, string Mod, string Exp);

    /// <summary>
    /// List containing all previously used nonce values as base 64 strings
    /// </summary>
    List<string> OldNonces {get;}

    /// <summary>
    /// List containing all previously used salt values as base 64 strings
    /// </summary>
    List<string> OldSalts {get;}
}
