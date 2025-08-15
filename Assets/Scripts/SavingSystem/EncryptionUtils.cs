using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SavingSystem
{
    public abstract class EncryptionUtils
    {
        private static readonly byte[] key = Encoding.UTF8.GetBytes("magofumondeporro"); // AES-128 key
        private static readonly int ivLength = 16;
        public static byte[] Encrypt(byte[] data)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.GenerateIV(); // Random IV each time
            aes.Key = key;
            using var encryptor = aes.CreateEncryptor();
            byte[] cyphered = encryptor.TransformFinalBlock(data, 0, data.Length);
        
            byte[] result = new byte[aes.IV.Length + cyphered.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cyphered, 0, result, aes.IV.Length, cyphered.Length);

            return result;
        }

        public static byte[] Decrypt(byte[] data)
        {

            byte[] iv = new byte[ivLength];
            Buffer.BlockCopy(data, 0, iv, 0, ivLength);

            int cipherLen = data.Length - ivLength;
            byte[] ciphertext = new byte[cipherLen];
            Buffer.BlockCopy(data, ivLength, ciphertext, 0, cipherLen);
        
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
        
            using var decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
        
        }

        public static byte[] Compress(byte[] data)
        {
            using var output = new MemoryStream();
            using (var gzip = new System.IO.Compression.GZipStream(output, System.IO.Compression.CompressionLevel.Optimal))
            {
                gzip.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            using var input = new MemoryStream(data);
            using var gzip = new System.IO.Compression.GZipStream(input, System.IO.Compression.CompressionMode.Decompress);
            using var output = new MemoryStream();
            gzip.CopyTo(output);
            return output.ToArray();
        }
    }
}
