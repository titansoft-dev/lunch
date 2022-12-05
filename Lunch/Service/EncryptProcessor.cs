using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Lunch.Service
{
    public class EncryptProcessor
    {
        protected string saltStr = "!sr45ce(t)4g";       // 鹽巴字串 
        protected string encryptedStr = null;  // 要被加密的字串

        // 加密某字串並取得加密後的結果
        public string getEncryptedStr(string encryptedStr)
        {
            this.encryptedStr = encryptedStr;
            encryptStr();
            return this.encryptedStr;
        }

        // 加密某個字串(加鹽 & 加密)
        protected virtual void encryptStr()
        {
            strToSalt();      // 加鹽
            SHA512EnCrypt();  // 加密
        }

        // 加鹽
        protected virtual void strToSalt()
        {
            encryptedStr = string.Concat(encryptedStr, saltStr);
        }

        // 加密
        protected virtual void SHA512EnCrypt()
        {
            SHA512 sha512 = new SHA512CryptoServiceProvider();//建立一個SHA512
            byte[] source = Encoding.Default.GetBytes(encryptedStr);//將字串轉為Byte[]
            byte[] crypto = sha512.ComputeHash(source);//進行SHA512加密
            string result = Convert.ToBase64String(crypto).Substring(0, 8);//把加密後的字串從Byte[]轉為字串
            this.encryptedStr = result;
        }
    }
}