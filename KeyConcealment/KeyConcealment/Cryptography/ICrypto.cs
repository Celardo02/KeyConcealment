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
    /// Returns a byte array containing the password hash
    /// </returns>
    byte[] CalculateHash(string input, ref string salt, ushort hashLen = 64);

    /// <summary>
    /// Checks if a string hash correspond to a previosly computed hash
    /// </summary>
    /// <param name="str">string to be compared with the hash</param>
    /// <param name="hash">previously computed hash</param>
    /// <param name="salt">
    /// base 64 string representing the salt value used to compute the hash
    /// </param>
    /// <returns></returns>
    bool VerifyString(string str, byte[] hash, string salt);

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
    /// <param name="nonce">byte array to be filled with inizialization vector value</param>
    /// <param name="tag">byte array to be filled with AES GMC tag value</param>
    /// <returns>
    /// Returns a base 64 string containing <c>plain</c> encrypted
    /// </returns>
    /// <remarks>
    /// Nonce array must be 12 bytes long and tag array must be 16 bytes long
    /// </remarks>
    string encryptAES_GMC(string plain, string key, ref string keySalt, ref byte[] nonce, ref byte[] tag);

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
    string decryptAES_GMC(string cyphered, string key, string keySalt, byte[] nonce, byte[] tag);
    // byte[] encryptRSA();
    // byte[] decryptRSA();
}
