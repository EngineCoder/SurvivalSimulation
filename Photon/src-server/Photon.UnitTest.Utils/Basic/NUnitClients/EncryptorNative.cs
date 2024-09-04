// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptorNative.cs" company="Exit Games GmbH">
//   Protocol & Photon Client Lib - Copyright (C) 2010 Exit Games GmbH
// </copyright>
// <summary>
//   Datagram Encryption implementation.
// </summary>
// <author>developer@photonengine.com</author>
// --------------------------------------------------------------------------------------------------------------------


#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif

#if (UNITY_IOS && !UNITY_EDITOR) || __IOS__
#define STATIC_IMPORT
#endif


namespace ExitGames.Client.Photon.Encryption
{
    using System;
    using System.Runtime.InteropServices;
    using System.Diagnostics;


    /// <summary>
    /// Implementation of encryption for "Datagram Encryption".
    /// </summary>
    public class EncryptorNative : IPhotonEncryptor
    {
        #if STATIC_IMPORT
        private const string LibName = "__Internal";
        #else
        private const string LibName = "PhotonEncryptorPlugin";
#endif
        // The classes which use DllImport, should always use public (!) extern method.This allows a PrelinkAll() check if the dll is loaded.
#if NATIVE_ENCRYPTOR_API1
        [DllImport(LibName)]
        public static extern IntPtr egconstructEncryptor(byte[] pEncryptSecret, byte[] pHmacSecret);
        [DllImport(LibName)]
        public static extern void egdestructEncryptor(IntPtr pEncryptor);
        [DllImport(LibName)]
        public static extern void egencrypt(IntPtr pEncryptor, byte[] pIn, int inSize, byte[] pOut, ref int outSize, ref int outOffset);
        [DllImport(LibName)]
        public static extern void egdecrypt(IntPtr pEncryptor, byte[] pIn, int inSizem, int inOffset, byte[] pOut, ref int outSize);
        [DllImport(LibName)]
        public static extern void egHMAC(IntPtr pEncryptor, byte[] pIn, int inSize, int inOffset, byte[] pOut, ref int outSize);
        [DllImport(LibName)]
        public static extern int eggetBlockSize();
        [DllImport(LibName)]
        public static extern int eggetIVSize();
        [DllImport(LibName)]
        public static extern int eggetHMACSize();
#else
        public enum ChainingMode : byte
        {
            CBC = 0,
            GCM = 1,
        }
        [DllImport(LibName)]
        public static extern IntPtr egconstructEncryptor2(byte[] pEncryptSecret, byte[] pHMACSecret, ChainingMode chainingMode, int mtu);
        [DllImport(LibName)]
        public static extern void egdestructEncryptor2(IntPtr pEncryptor);
        [DllImport(LibName)]
        public static extern void egencrypt2(IntPtr pEncryptor, byte[] pIn, int inSize, byte[] pHeader, byte[] pOut, int outOffset, ref int outSize);
        [DllImport(LibName)]
	    public static extern void egdecrypt2(IntPtr pEncryptor, byte[] pIn, int inSize, int inOffset, byte[] pHeader, byte[] pOut, out int outSize);
        [DllImport(LibName)]
	    public static extern int egcalculateEncryptedSize(IntPtr pEncryptor, int unencryptedSize);
        [DllImport(LibName)]
        public static extern int eggetFragmentLength(IntPtr pEncryptor);
#endif
        #if !NETFX_CORE
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LogCallbackDelegate(IntPtr userData, int level, [MarshalAs(UnmanagedType.LPStr)] string msg);
        [DllImport(LibName)]
        public static extern void egsetEncryptorLoggingCallback(IntPtr userData, LogCallbackDelegate callback);
        #endif
        [DllImport(LibName)]
        public static extern bool egsetEncryptorLoggingLevel(int level);

        [DllImport(LibName)]
        public static extern int eggetEncryptorPluginVersion();

        private enum egDebugLevel
        {
            OFF = 0,
            ERRORS = 1,
            WARNINGS = 2,
            INFO = 3,
            ALL = 4,
        }

        // Constant values instead of EnetPeer protected constant fields allow to use this module as a standalone native lib wrapper in Unity
#if NATIVE_ENCRYPTOR_API1
        /// <summary>
        /// Defines block size for encryption/decryption algorithm
        /// </summary>
        public static readonly int BLOCK_SIZE = 16; // = EnetPeer.BLOCK_SIZE
        /// <summary>
        /// Defines IV size for encryption/decryption algorithm
        /// </summary>
        public static readonly int IV_SIZE = 16; // = EnetPeer.IV_SIZE
        /// <summary>
        /// Defines HMAC size for packet authentication algorithm
        /// </summary>
        public static readonly int HMAC_SIZE = 32; // = EnetPeer.HMAC_SIZE

        protected byte[] hmacHash = new byte[HMAC_SIZE];
#endif

        protected IntPtr encryptor;

        ~EncryptorNative()
        {
            this.Dispose(false);
        }

        #if !NETFX_CORE && (UNITY || SUPPORTED_UNITY)
        [AOT.MonoPInvokeCallback(typeof(LogCallbackDelegate))]
        #endif
        private static void OnNativeLog(IntPtr userData, int debugLevel, string message)
        {
            #if UNITY || SUPPORTED_UNITY
            switch ((egDebugLevel)debugLevel)
            {
                case egDebugLevel.ERRORS:
                    UnityEngine.Debug.LogError("EncryptorNative: " + message);
                    break;
                case egDebugLevel.WARNINGS:
                    UnityEngine.Debug.LogWarning("EncryptorNative: " + message);
                    break;
                case egDebugLevel.INFO:
                case egDebugLevel.ALL:
                case egDebugLevel.OFF:
                    UnityEngine.Debug.Log("EncryptorNative: " + message);
                    break;
            }
            #else
            Debug.WriteLine("EncryptorNative: " + message);
            #endif
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="encryptionSecret"></param>
        /// <param name="hmacSecret"></param>
        /// <param name="ivBytes"></param>
        public void Init(byte[] encryptionSecret, byte[] hmacSecret, byte[] ivBytes = null, bool chainingModeGCM = false, int mtu = 1200)
        {
            #if !NETFX_CORE && (!ENABLE_MONO || !UNITY_XBOXONE)
            egsetEncryptorLoggingCallback(IntPtr.Zero, OnNativeLog);
            #endif
            egsetEncryptorLoggingLevel((int)egDebugLevel.ERRORS);
#if NATIVE_ENCRYPTOR_API1
            this.encryptor = egconstructEncryptor(encryptionSecret, hmacSecret);
#else
            this.encryptor = egconstructEncryptor2(encryptionSecret, hmacSecret, chainingModeGCM ? ChainingMode.GCM : ChainingMode.CBC, mtu);
#endif
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // ReSharper disable once UnusedParameter.Local
        private void Dispose(bool dispose)
        {
            if (this.encryptor != IntPtr.Zero)
            {
#if NATIVE_ENCRYPTOR_API1
                egdestructEncryptor(this.encryptor);
#else
                egdestructEncryptor2(this.encryptor);
#endif
                this.encryptor = IntPtr.Zero;
            }
        }

#if NATIVE_ENCRYPTOR_API1
        /// <summary>
        /// Encrypts data. puts them into output buffer and prepends with IV
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <param name="output"></param>
        /// <param name="offset"></param>
        /// <param name="ivPrefix"></param>
        /// <returns></returns>
        public void Encrypt(byte[] data, int len, byte[] output, ref int offset, bool ivPrefix = true)
        {
            int outSize = output.Length;
            egencrypt(this.encryptor, data, len, output, ref outSize, ref offset);
        }


        /// <summary>
        /// Finishes current HMAC
        /// </summary>
        /// <returns></returns>
        public byte[] CreateHMAC(byte[] data, int offset, int count)
        {
            lock (this.hmacHash)
            {
                var outLen = this.hmacHash.Length;
                egHMAC(this.encryptor, data, count, offset, this.hmacHash, ref outLen);
            }

            return this.hmacHash;
        }



        /// <summary>
        /// Decrypts buffer containing HMAC
        /// </summary>
        /// <param name="data">encrypted data prepened by IV</param>
        /// <param name="offset">offset in the buffer</param>
        /// <param name="len">len of data to decrypt</param>
        /// /// <param name="outLen">len of decrypted data</param>
        /// <param name="ivPrefix"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] data, int offset, int len, out int outLen, bool ivPrefix = true)
        {
            outLen = data.Length;
            egdecrypt(encryptor, data, len, offset, data, ref outLen);
            return data;
        }

        /// <summary>
        /// Checkes wheter data have corect HMAC or not
        /// </summary>
        /// <param name="data">buffer with data and HMAC</param>
        /// <param name="len">len of data including HMAC</param>
        /// <returns>true if check pass, false otherwise</returns>
        public bool CheckHMAC(byte[] data, int len)
        {
            lock (this.hmacHash)
            {
                var outLen = this.hmacHash.Length;
                egHMAC(encryptor, data, len - HMAC_SIZE, 0, this.hmacHash, ref outLen);

                var hash = this.hmacHash;

                var result = true;
                for (var i = 0; i < 4 && result; ++i)
                {
                    var dataStartIndex = len - HMAC_SIZE + i * 8;
                    var hashStartIndex = i * 8;
                    result =
                        data[dataStartIndex] == hash[hashStartIndex]
                        && data[dataStartIndex + 1] == hash[hashStartIndex + 1]
                        && data[dataStartIndex + 2] == hash[hashStartIndex + 2]
                        && data[dataStartIndex + 3] == hash[hashStartIndex + 3]
                        && data[dataStartIndex + 4] == hash[hashStartIndex + 4]
                        && data[dataStartIndex + 5] == hash[hashStartIndex + 5]
                        && data[dataStartIndex + 6] == hash[hashStartIndex + 6]
                        && data[dataStartIndex + 7] == hash[hashStartIndex + 7];
                }
                return result;
            }
        }

#else
        public void Encrypt2(byte[] data, int len, byte[] header, byte[] output, int outOffset, ref int outSize)
        {
            egencrypt2(this.encryptor, data, len, header, output, outOffset, ref outSize);
        }

        public byte[] Decrypt2(byte[] data, int offset, int len, byte[] header, out int outLen)
        {
            outLen = data.Length;
            egdecrypt2(encryptor, data, len, offset, header, data, out outLen);
            return data;
        }

        public int CalculateEncryptedSize(int unencryptedSize)
        {
            return egcalculateEncryptedSize(encryptor, unencryptedSize);
        }

        public int CalculateFragmentLength()
        {
            return eggetFragmentLength(encryptor);
        }

#endif
    }
}
