using System.Security.Cryptography;
using System.Text;

namespace ComplaintManagement.Helpers
{
    public static class CryptoHelper
    {
        public class FinpayiSecurity
        {
            private const string hashProvider = "hashprovider";

            private const string symmProvider = "symprovider";

            private const string symmKeyFileName = "SymmetricKeyFile.txt";

            public interface ICrypto
            {
                int BlockSize();
                int KeySize();

                string Encrypt(string data);
                string Decrypt(string data);
                string Base64Encode(string data);
                string Base64Decode(string data);
            }

            public class CryptoFactory
            {
                public ICrypto MakeCryptographer(string type)
                {
                    switch (type.ToLower())
                    {
                        case "des":
                            //Return New DES()
                            return new Rijndael();
                        case "tripledes":
                            //Return New TripleDES()
                            return new Rijndael();
                        case "rijndael":
                            return new Rijndael();
                        default:
                            //Return New TripleDES()
                            return new Rijndael();
                    }
                }
            }
            public class Rijndael : ICrypto
            {

                // The key and initialization vector : change them for your application
                private byte[] _key = {
            132,
            42,
            53,
            124,
            75,
            56,
            87,
            38,
            9,
            10,
            161,
            132,
            183,
            91,
            105,
            16,
            117,
            218,
            149,
            230,
            221,
            212,
            235,
            64
        };
                private byte[] _iv = {
            83,
            71,
            26,
            58,
            54,
            35,
            22,
            11,
            83,
            71,
            26,
            58,
            54,
            35,
            22,
            11

        };
                // returns the default size, in bits of the iv
                public int BlockSize()
                {
                    RijndaelManaged aes = new RijndaelManaged();

                    return aes.BlockSize;
                }

                // returns the default size, in bits of the key
                public int KeySize()
                {
                    RijndaelManaged aes = new RijndaelManaged();

                    return aes.KeySize;
                }

                // decrypts a string that was encrypted using the Encrypt method
                public string Decrypt(string data)
                {
                    try
                    {
                        byte[] inBytes = Convert.FromBase64String(data);
                        MemoryStream mStream = new MemoryStream(inBytes, 0, inBytes.Length);
                        // instead of writing the decrypted text

                        RijndaelManaged aes = new RijndaelManaged();
                        CryptoStream cs = new CryptoStream(mStream, aes.CreateDecryptor(_key, _iv), CryptoStreamMode.Read);

                        StreamReader sr = new StreamReader(cs);

                        return sr.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                // Encrypts a given string
                public string Encrypt(string data)
                {
                    try
                    {
                        UTF8Encoding utf8 = new UTF8Encoding();
                        byte[] inBytes = utf8.GetBytes(data);
                        // ascii encoding uses 7 
                        //bytes for characters whereas the encryption uses 8 bytes, thus we use utf8
                        MemoryStream ms = new MemoryStream();
                        //instead of writing the encrypted 
                        //string to a filestream, I will use a memorystream

                        RijndaelManaged aes = new RijndaelManaged();
                        CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(_key, _iv), CryptoStreamMode.Write);

                        cs.Write(inBytes, 0, inBytes.Length);
                        // encrypt
                        cs.FlushFinalBlock();

                        return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                public string Base64Encode(string plainText)
                {
                    var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                    return Convert.ToBase64String(plainTextBytes);
                }

                public string Base64Decode(string base64EncodedData)
                {
                    var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                    return Encoding.UTF8.GetString(base64EncodedBytes);
                }
            }
        }
    }
}