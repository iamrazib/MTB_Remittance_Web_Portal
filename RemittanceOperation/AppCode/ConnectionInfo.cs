using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace RemittanceOperation.AppCode
{
    public class ConnectionInfo
    {
        string connectionStringNrbWork = ConfigurationManager.ConnectionStrings["connectionStringNrbWork"].ConnectionString;
        string connectionStringLv = ConfigurationManager.ConnectionStrings["connectionStringLv"].ConnectionString;
        string connectionStringDR = ConfigurationManager.ConnectionStrings["connectionStringDR"].ConnectionString;
        string connectionStringOld = ConfigurationManager.ConnectionStrings["connectionStringOld"].ConnectionString;

        string connectionStringRemitUAT = ConfigurationManager.ConnectionStrings["connectionStringUAT"].ConnectionString;

        string _key = "ABCDEFFEDCBAABCDEFFEDCBAABCDEFFEDCBAABCDEFFEDCBA";
        string _vector = "ABCDEFFEDCBABCDE";

        public string getConnStringDR()
        {
            return DecryptString(connectionStringDR);
        }

        public string getConnStringRemitLv()
        {
            return DecryptString(connectionStringLv);
        }

        public string getOldConnString()
        {
            return DecryptString(connectionStringOld);
        }

        public string getNrbWorkConnString()
        {
            return DecryptString(connectionStringNrbWork);
        }

        public string getRemitUATConnString()
        {
            return DecryptString(connectionStringRemitUAT);
        }

        public string getDecrypt(string stringToDecrypt)
        {
            return DecryptString(stringToDecrypt);
        }

        public string getEncrypt(string stringToEncrypt)
        {
            return EncryptString(stringToEncrypt);
        }

        private string EncryptString(string stringToEncrypt)
        {
            if (stringToEncrypt == null || stringToEncrypt.Length == 0)
            {
                return "";
            }

            TripleDESCryptoServiceProvider _cryptoProvider = new TripleDESCryptoServiceProvider();
            try
            {
                _cryptoProvider.Key = HexToByte(_key);
                _cryptoProvider.IV = HexToByte(_vector);

                byte[] valBytes = Encoding.Unicode.GetBytes(stringToEncrypt);
                ICryptoTransform transform = _cryptoProvider.CreateEncryptor();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
                cs.Write(valBytes, 0, valBytes.Length);
                cs.FlushFinalBlock();
                byte[] returnBytes = ms.ToArray();
                cs.Close();
                return Convert.ToBase64String(returnBytes);
            }
            catch
            {
                return "";
            }
        }

        private string DecryptString(string stringToDecrypt)
        {
            if (stringToDecrypt == null || stringToDecrypt.Length == 0)
            {
                return "";
            }

            TripleDESCryptoServiceProvider _cryptoProvider = new TripleDESCryptoServiceProvider();

            try
            {
                _cryptoProvider.Key = HexToByte(_key);
                _cryptoProvider.IV = HexToByte(_vector);

                byte[] valBytes = Convert.FromBase64String(stringToDecrypt);
                ICryptoTransform transform = _cryptoProvider.CreateDecryptor();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
                cs.Write(valBytes, 0, valBytes.Length);
                cs.FlushFinalBlock();
                byte[] returnBytes = ms.ToArray();
                cs.Close();
                return Encoding.Unicode.GetString(returnBytes);
            }
            catch
            {
                return "";
            }
        }

        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] =
                Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
    

    }
}