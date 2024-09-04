using System.Security.Cryptography;
using System.Text;
using System;

/// <summary>
/// Data Encryption Standard
/// </summary>
public class Manager_DataEncryption
{
    //散列算法    MD5、SHA1、SHA256、HMACMD5、HMACSHA1、HMACSHA256
    #region HashAlgorithm 

    #endregion


    //对称加密算法    AES(Rijndael)、DES、RC2、TripleDES
    #region SymmetricAlgorithm

    /// <summary>
    /// Advanced Encryption Standard
    /// </summary>
    /// <param name="key"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string Encrypt_AES(string key, string content)
    {
        byte[] keyArray = Encoding.UTF8.GetBytes(key);

        RijndaelManaged encryption = new RijndaelManaged();
        encryption.Key = keyArray;

        //1.电码本模式（Electronic Codebook Book(ECB)）
        //2.密码分组链接模式（Cipher Block Chaining(CBC)）
        //3.计算器模式（Counter(CTR)）
        //4.密码反馈模式（Cipher FeedBack(CFB)）
        //5.输出反馈模式（Output FeedBack(OFB)）
        encryption.Mode = CipherMode.ECB;
        encryption.Padding = PaddingMode.PKCS7;

        ICryptoTransform cryptoTransform = encryption.CreateEncryptor();
        byte[] encryptArray = Encoding.UTF8.GetBytes(content);
        byte[] resultArray = cryptoTransform.TransformFinalBlock(encryptArray, 0, encryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }


    /// <summary>
    /// Advanced Encryption Standard
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="sKey"></param>
    /// <returns></returns>
    public static string Decrypt_AES(string key, string content)
    {
        byte[] keyArray = Encoding.UTF8.GetBytes(key);

        RijndaelManaged decryption = new RijndaelManaged();
        decryption.Key = keyArray;

        //1.电码本模式（Electronic Codebook Book(ECB)）
        //2.密码分组链接模式（Cipher Block Chaining(CBC)）
        //3.计算器模式（Counter(CTR)）
        //4.密码反馈模式（Cipher FeedBack(CFB)）
        //5.输出反馈模式（Output FeedBack(OFB)）
        decryption.Mode = CipherMode.ECB;
        decryption.Padding = PaddingMode.PKCS7;

        ICryptoTransform cryptoTransform = decryption.CreateDecryptor();
        byte[] decryotArray = Convert.FromBase64String(content);
        byte[] resultArray = cryptoTransform.TransformFinalBlock(decryotArray, 0, decryotArray.Length);
        return Encoding.UTF8.GetString(resultArray, 0, resultArray.Length);
    }
    #endregion


    //非对称加密算法   RSA、DSA
    #region AsymmetricAlgorithm 
    #endregion
}