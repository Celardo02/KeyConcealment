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
    string CalculateHash(string input, ref string salt, ushort hashLen = 64);

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
    bool VerifyString(string str, string hash, string salt);

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
    /// Throws <c>CryptoArgExc</c> exception if nonce or tag array length oes not 
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
    /// Throws <c>CryptoArgExc</c> exception if nonce or tag array length does not 
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
    /// List containing all previously used nonce values as base 64 strings
    /// </summary>
    List<string> OldNonces {get;}

    /// <summary>
    /// List containing all previously used salt values as base 64 strings
    /// </summary>
    List<string> OldSalts {get;}
}
