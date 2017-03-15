using System;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.IO.Compression;
using System.IO;

namespace SafeWebApi
{
    public class TokenTool
    {
        /// <summary>
        /// 定义两个key
        /// </summary>
        public const string key1 = "Iqx5L5ps", key2 = "5ou6Wo6W", iv = "5ouYp656";

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetToken(Guid userId)
        {
            //过期时间1天
            var content = new TokenContent() { User = new User() { Id = userId }, Expires = DateTime.Now.AddDays(1) };
            var temp = JsonConvert.SerializeObject(content);
            var es1 = Encrypt.EncryptDES(temp, key1);
            var es2 = Encrypt.EncryptDES(es1, key2);

            return es2;
        }

        /// <summary>
        /// 取出Token内容
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static TokenContent GetTokenContent(string token)
        {
            TokenContent content = null;
            try
            {
                var es2 = Encrypt.DecryptDES(token, key2);
                var es1 = Encrypt.DecryptDES(es2, key1);
                content = JsonConvert.DeserializeObject<TokenContent>(es1);
            }
            catch (Exception) { }

            return content;
        }

        /// <summary>
        /// 检查Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Result CheckToken(string token)
        {
            Result result = new Result();
            var content = GetTokenContent(token);
            //检验失败
            if (content == null)
            {
                result.Status = RStatus.S0003;
            }
            //过期
            else if (content.Expires < DateTime.Now)
            {
                result.Status = RStatus.S0004;
            }
            else
            {
                result.Status = RStatus.S0001;
            }

            return result;
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static User GetUser(string token)
        {
            return GetTokenContent(token).User;
        }

        /// <summary>
        /// Token内容
        /// </summary>
        public class TokenContent
        {
            public User User { get; set; }
            public DateTime Expires { get; set; }
        }
    }


    /// <summary>
    /// 加密解密相关
    /// </summary>
    /// <remarks>author:lorne date:2016-07-19</remarks>
    public class Encrypt
    {
        private Encrypt() { }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="ConvertString">待加密字符串</param>
        /// <returns></returns>
        public static string Md5(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)));
            t2 = t2.Replace("-", "");
            return t2;
        }

        /// <summary>
        /// 以默认密钥进行DES加密
        /// </summary>
        /// <param name="Text">待加密字符串</param>
        /// <returns></returns>
        public static string EncryptDES(string Text)
        {
            string sKey = "1C6099AF44D3B4233A8710E3";
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(Md5(sKey).Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(Md5(sKey).Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        /// <summary>
        /// 以默认密钥进行DES解密
        /// </summary>
        /// <param name="Text">待加密字符串</param>
        /// <returns></returns>
        public static string DecryptDES(string Text)
        {
            string sKey = "1C6099AF44D3B4233A8710E3";
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(Md5(sKey).Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(Md5(sKey).Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        /// <summary>
        /// DES加密字符串，密钥无长度限制
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,无长度限制，内部将用MD5加密此字符后截取作为密钥</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(encryptString);
            des.Key = ASCIIEncoding.ASCII.GetBytes(Md5(encryptKey).Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(Md5(encryptKey).Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        /// <summary>
        /// DES解密字符串，密钥无长度限制
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = decryptString.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(decryptString.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(Md5(decryptKey).Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(Md5(decryptKey).Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns></returns>
        public static string EncryptString(string encryptString, string encryptKey)
        {
            try
            {
                string sIV = "xwdwngdj";
                byte[] data = Encoding.UTF8.GetBytes(encryptString);

                DESCryptoServiceProvider DES = new DESCryptoServiceProvider();

                DES.Key = ASCIIEncoding.ASCII.GetBytes(encryptKey);

                DES.IV = ASCIIEncoding.ASCII.GetBytes(sIV);

                ICryptoTransform desencrypt = DES.CreateEncryptor();

                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);

                return BitConverter.ToString(result);
            }
            catch { }

            return "转换出错！";
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns></returns>
        public static string DecryptString(string encryptString, string encryptKey)
        {
            try
            {
                string sIV = "xwdwngdj";
                string[] sInput = encryptString.Split("-".ToCharArray());

                byte[] data = new byte[sInput.Length];

                for (int i = 0; i < sInput.Length; i++)
                {
                    data[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
                }

                DESCryptoServiceProvider DES = new DESCryptoServiceProvider();

                DES.Key = ASCIIEncoding.ASCII.GetBytes(encryptKey);

                DES.IV = ASCIIEncoding.ASCII.GetBytes(sIV);

                ICryptoTransform desencrypt = DES.CreateDecryptor();

                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);

                return Encoding.UTF8.GetString(result);
            }
            catch { }

            return "解密出错！";
        }
    }
}