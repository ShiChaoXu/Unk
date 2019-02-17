using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Unk.Core
{
    public static class Utils
    {
        public static string SqlConnectionString = GetAppSetting("SqlConnection");
        #region app config

        public static string GetAppSetting(string key)
        {
            var setting = WebConfigurationManager.AppSettings[key];
            setting = setting ?? string.Empty;
            return setting;
        }

        /// <summary>
        /// return ex: http://gpmp.lenovo.com/
        /// </summary> 
        /// <returns></returns>
        public static string GetGPMPWebRoot()
        {
            var refUrl = HttpContext.Current.Request.UrlReferrer;
            if (refUrl == null)
            {
                switch (GetAppSetting("SystemEnv"))
                {
                    default:
                    case "DIT":
                        return "http://t.gpmp.lenovo.com/";
                    case "UAT":
                        return "http://10.38.24.30:3030/";
                    case "PROD":
                        return "https://gpmp.lenovo.com/";
                }

            }

            return refUrl.Scheme + "://" + refUrl.Authority + "/";
        }

        public static string GetGPMPWebApiRoot()
        {
            var root = GetAppSetting("WebApiRoot");
            if (string.IsNullOrEmpty(root))
            {
                root = "https://gpmpwebapi.lenovo.com/";

            }
            return root;
        }

        public static string GoGPMPApproveForm(int pid)
        {
            return string.Format(GetGPMPWebRoot() + "pipeline/approve/?formid={0}", pid);
        }
        #endregion

        #region Number converter
        public static int ToInt(object obj, int def = 0)
        {
            int result = def;
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out result);
            }

            return result;
        }

        public static decimal ToDecimal(object obj, decimal def = 0)
        {
            decimal result = def;
            if (obj != null)
            {
                decimal.TryParse(obj.ToString(), out result);
            }

            return result;
        }

        public static double CNY2USD(double amount, double rate)
        {
            return ((int)((amount / rate / 1000) * 100)) / 100;
        }


        #endregion

        #region date converter

        public static DateTime ToDate(string dateStr, DateTime def)
        {
            DateTime rv = def;
            DateTime.TryParse(dateStr, out rv);
            return rv;
        }

        public static bool IsDate(object value)
        {
            if (value == null)
            {
                return false;
            }
            DateTime rv = DateTime.UtcNow;
            return DateTime.TryParse(value.ToString(), out rv);
        }

        #endregion

        #region list
        public static bool NotContains<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var success = false;

            foreach (var l1 in list1)
            {
                success = list2.Where(l2 => l2.Equals(l1)).Count() > 0;
                if (success)
                    break;
            }

            return !success;
        }


        #endregion

        #region string 
        public static List<string> Split(string strs, string sign = ",")
        {
            if (string.IsNullOrEmpty(strs))
            {
                return new List<string>();
            }
            var list = strs.Split(new string[] { sign }, StringSplitOptions.RemoveEmptyEntries);
            return list.ToList();
        }

        public static List<string> RegexMatchValues(string str, string regex)
        {
            List<string> values = new List<string>();
            foreach (Match m in Regex.Matches(str, regex, RegexOptions.IgnoreCase))
            {
                values.Add(m.Value);

            }
            return values;
        }

        public static string PreventSqlInjection<T>(T value)
        {
            if (value == null)
            {
                return "NULL";
            }

            //value type
            var type = value.GetType();

            #region excluedeValueTypes
            var excludeValueTypes = new List<Type> {
                typeof(DateTime),
                typeof(bool),
                typeof(string),
                typeof(Guid)
            };
            var isInExcludeValueTypes = false;

            foreach (var excludeType in excludeValueTypes)
            {
                if (type.IsSubclassOf(excludeType) || type == excludeType)
                {
                    isInExcludeValueTypes = true;
                    break;
                }
            }

            if (isInExcludeValueTypes)
            {
                return $"N'{value.ToString().Replace("'", "''")}'";
            }

            #endregion

            if (type.GetInterface(nameof(IConvertible)) != null)
            {
                var result = Convert.ToString(Convert.ChangeType(value, type));
                if (result == null)
                {
                    return "NULL";
                }

                if (value is Enum)
                {
                    return Convert.ToInt32(value).ToString();
                }
            }

            return $"{value}";
        }
        #endregion


        #region json

        public static string JsonSerialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T JsonDeserialize<T>(string json) where T : new()
        {
            if (string.IsNullOrEmpty(json))
            {
                json = string.Empty;
            }
            var rv = JsonConvert.DeserializeObject<T>(json);
            if (rv == null)
            {
                rv = new T();
            }
            return rv;
        }

        public static object JsonDeserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                json = string.Empty;
            }
            return JsonConvert.DeserializeObject(json);
        }

        public static dynamic JsonDeserialize(string json, Type type)
        {
            if (string.IsNullOrEmpty(json))
            {
                return type;
            }

            dynamic result = JsonConvert.DeserializeObject(json, type);
            return result;
        }

        public static T JsonDeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            if (string.IsNullOrEmpty(json))
            {
                return anonymousTypeObject;
            }
            return JsonConvert.DeserializeAnonymousType<T>(json, anonymousTypeObject);
        }
        #endregion

        #region class

        public static object GetTypeValue<T>(T obj, string field)
        {
            var value = new object();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.Name == field);
            if (properties.Count() > 0)
            {
                value = properties.First().GetValue(obj);
            }

            return value;
        }

        #endregion

        #region copyTo

        static string PUBLIC_KEY;
        static string PRIVATE_KEY;
        static Utils()
        {
            //RSACryptoServiceProvider oRSA = new RSACryptoServiceProvider();
            //PUBLIC_KEY = oRSA.ToXmlString(false);
            //PRIVATE_KEY = oRSA.ToXmlString(true);
            PUBLIC_KEY = "<RSAKeyValue><Modulus>uRmNxaIDAK6z9aSkMzwRWJlSYkzHfaJ7M/gEEaO48bRf3tLqmc0yNseS3gj0HfEWD08Y88ENcXdaqpR6L4JgwozTHZXO5CGwBt6ZtzfKirENa2JhgClSzLeMPW4aAVnld2vph+6ko0EwWix29Fic1rRTqd2abSS8sRc5oacgX38=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            PRIVATE_KEY = "<RSAKeyValue><Modulus>uRmNxaIDAK6z9aSkMzwRWJlSYkzHfaJ7M/gEEaO48bRf3tLqmc0yNseS3gj0HfEWD08Y88ENcXdaqpR6L4JgwozTHZXO5CGwBt6ZtzfKirENa2JhgClSzLeMPW4aAVnld2vph+6ko0EwWix29Fic1rRTqd2abSS8sRc5oacgX38=</Modulus><Exponent>AQAB</Exponent><P>zEx5EFFM9CzueEdV+LzIpZuFNxIzlUyWBThel74QHz5yZoQwkPbRa9Mf9KISCp1AwGVyHz9CGa7ZOgsPsMBTiQ==</P><Q>5/FG2NDWoSq59OLUK6o+mqRKWWp9nHKt1baJPdpIuMfTIni/O/Dnk+bg/IBaY94w6a5CTeWI0MYIKbpxYhfwxw==</Q><DP>m0d0rOZeayjLiDgQLCKxDs6KDjWTZ2Lyk70oiIU5k8XPBgRrNYOj4SRzIWkd9VtYn+N7PizCfOrcyLUnk9xaoQ==</DP><DQ>4QQ2U0nnr1ugJG+anvH+4k/YwX6KdijbdKYt5w/J1Vom/x5diG1ifR5TzyNGjfSVR1+De8bfQIueh70VGrFXKQ==</DQ><InverseQ>wNeqieer675Vhg/BJCXXTPQ8tUHlHUl+0LQAy8f7qbiKKAtUd3Y9oE1yNTF4G4EDetQ67F9oEo0fiFjrXIebmg==</InverseQ><D>UK1/qrWb429CRv4VB7PVx61ESE444VyxoIwokduvn8JDyyZZVOIoUdIZBKgYZviO6etK7+ukRWGFZjZDL3P/Ye7E48iAn4hMeBWvTmk2BbNkGteYiFFGv++QsB9B3EWHGAJVp1WFNwGDaQ9AH+F/3r9h80OqMiR+6lvIcO7QovE=</D></RSAKeyValue>";
        }

        public static string Encrypt(string pString)
        {
            try
            {
                RSACryptoServiceProvider oRSA = new RSACryptoServiceProvider(32);
                oRSA.FromXmlString(PUBLIC_KEY);
                byte[] bString = Encoding.UTF8.GetBytes(pString);
                byte[] bCrypt = oRSA.Encrypt(bString, false);
                return Convert.ToBase64String(bCrypt);
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string Decrypt(string pString)
        {
            try
            {
                byte[] bString = Convert.FromBase64String(pString);
                RSACryptoServiceProvider oRSA = new RSACryptoServiceProvider(32);
                oRSA.FromXmlString(PRIVATE_KEY);
                byte[] bCrypt = oRSA.Decrypt(bString, false);
                return Encoding.UTF8.GetString(bCrypt);
            }
            catch
            {
                return string.Empty;
            }
        }


        #endregion

        #region  Prevent injection attacks

        public static string NoAttacks(string str)
        {
            string rv = str == null ? string.Empty : str;

            // html
            //rv = HttpUtility.HtmlEncode(str);
            //sql 
            var sqlArr = new string[] { "exec ", "insert ", "delete "
            , "truncate ", "char ", "declare ", "update ", "chr ", "mid ", "master "};
            foreach (var sql in sqlArr)
            {
                rv = Regex.Replace(rv, sql, string.Empty, RegexOptions.IgnoreCase);
            }
            return rv.Trim();
        }

        /// <summary>
        /// only allowChars can contain in str
        /// return true is not attack sql
        /// </summary>
        /// <param name="str"></param>
        /// <param name="allowChars"></param>
        /// <returns></returns>
        public static bool AttackAllowSql(string str, List<string> allowChars)
        {
            bool success = true;

            var sqlArr = new List<string> { "select ", "exec ", "insert ", "delete ", "where ", "truncate ", "char ", "declare ", "update ", "chr ", "mid ", "master " };

            sqlArr.RemoveAll(m => allowChars.Select(c => c.ToLower()).Contains(m));
            foreach (var sql in sqlArr)
            {
                if (Regex.IsMatch(str, sql, RegexOptions.IgnoreCase))
                {
                    success = false;
                    break;
                }
            }
            return success;
        }

        #endregion

        #region exception        
        /// <summary>
        ///  Description:Resolve Exception
        ///  Author: MWJ
        ///  Date: 2016-12-27
        /// </summary>
        public static string ResolveException(Exception exception)
        {
            var sbMessage = new StringBuilder();
            var innerException = exception.InnerException;
            if (innerException == null)
            {
                sbMessage.AppendLine(exception.Message);
            }
            while (innerException != null)
            {
                sbMessage.AppendLine(innerException.Message);
                sbMessage.AppendLine();

                innerException = innerException.InnerException;
            }

            return sbMessage.ToString();
        }
        #endregion
        

        #region Base64
        public static string ToBase64(string pStr)
        {
            string encode = string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(pStr);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = pStr;
            }
            return encode;
        }

        public static string Base64ToString(string pStr)
        {
            string decode = string.Empty;
            try
            {
                byte[] bytes = Convert.FromBase64String(pStr);
                decode = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                decode = "";
            }
            return decode;
        }
        #endregion

        #region Match
        public static double ConvertDouble(string pValue)
        {
            if (pValue.IndexOf("-") == -1)
            {
                return double.Parse(pValue);
            }
            else
            {
                return double.Parse("-" + pValue.Replace("-", ""));
            }
        }

        #endregion
        public static string GetITCode(string executor)
        {
            return executor.Substring(executor.IndexOf("(") + 1).Replace(")", "");
        }
        public static string DownFile(string FileID)
        {
            return string.Format(GetGPMPWebApiRoot() + "FileUpload/Download?FileId={0}", FileID);
        }
        public static DateTime ticksToDateTime(string ticks)
        {
            int _ticks = 0;
            int.TryParse(ticks, out _ticks);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long tt = dt.Ticks + _ticks * 10000;
            return new DateTime(tt);
        }

    }
}
