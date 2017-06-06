using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Linq;
using System.Threading;

namespace Tools
{
    public static class ToolFunctions
    {
        public static bool Not(bool predicate)
        {
            return !predicate;
        }

        public static IEnumerable<TResult> SelectTwo<TSource, TResult>(this IEnumerable<TSource> source,
                                                                        Func<TSource, TSource, TResult> selector)
        {
            return Enumerable.Zip(source, source.Skip(1), selector);
        }

        public static bool IsEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }
        public static bool NonEmpty(this string text)
        {
            return !(string.IsNullOrEmpty(text));
        }

        public static bool IsFalse(this bool predicate)
        {
            return !predicate;
        }

        public static string GetRightPartOf(this string inStr, string partToFind)
        {
            if (null == partToFind || null== inStr) return String.Empty;
            int pos = inStr.IndexOf(partToFind, StringComparison.OrdinalIgnoreCase);
            return pos > -1 ? inStr.Substring(pos + partToFind.Length)
                            : String.Empty;
        }

        public static string LeftChars(this string str, int countFromLeft)
        {
            if (null == str || countFromLeft < 1)
                return String.Empty;

            return str.Substring(0, Math.Min(str.Length, countFromLeft));
        }
        public static bool SplitInTwoPartsFirst(string inStr, string separator, out string leftStr, out string rightStr)
        {
            bool retOk;
            int sepLen = separator.Length;
            int pos = inStr.IndexOf(separator, System.StringComparison.OrdinalIgnoreCase);
            if (pos > -1)
            {
                rightStr = inStr.Substring(pos + sepLen, inStr.Length - pos - sepLen);
                leftStr = inStr.Substring(0, pos);
                retOk = true;
            }
            else
            {
                leftStr = inStr;
                rightStr = "";
                retOk = false;
            }

            return retOk;
        }
        // ReSharper disable once UnusedMember.Local
        public static bool SplitInTwoPartsLast(string inStr, string separator, out string leftStr, out string rightStr)
        {
            bool retOk;
            int sepLen = separator.Length;
            int pos = inStr.LastIndexOf(separator, System.StringComparison.OrdinalIgnoreCase);
            if (pos > -1)
            {
                rightStr = inStr.Substring(pos + sepLen, inStr.Length - pos - sepLen);
                leftStr = inStr.Substring(0, pos);
                retOk = true;
            }
            else
            {
                leftStr = inStr;
                rightStr = "";
                retOk = false;
            }

            return retOk;
        }


    }

    public static class CollectionUtils
    {
        public static string GetValueOrDefault(this Dictionary<string,string> dict, string key)
        {
            if (null == dict)
                return String.Empty;
            string readValue;
            //dict.TryGetValue(key ?? "", out readValue);
            return dict.TryGetValue(key ?? String.Empty, out readValue) ? readValue : "";
        }


        //public static TValue GetValueOrMyDefault2<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultVal)
        //    where TValue : ValueType
        //{
        //    if (null == dict)
        //        return defaultVal;

        //    string readValue;
        //    return dict.TryGetValue(TKey, out readValue) ? readValue : defaultVal;
        //}

        public static TValue GetValueOrMyDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultVal)
            where TValue: class
        {
            if (null == dict) // possible only, because == is static function AND extension method is static function which allows dict to be null, what would not be the case if it would be an instance method
                return defaultVal;

            TValue readValue;
            return dict.TryGetValue(key, out readValue) ? readValue : defaultVal;
        }

        public static string GetValueOrMyDefault(this Dictionary<string, string> dict, string key, string defaultVal)
        {
            // Policy used here: null is changed to String.Empty
            string newDefaultVal = defaultVal ?? String.Empty;
            if (null == dict)
                return newDefaultVal;

            string readValue;
            return dict.TryGetValue(key ?? String.Empty, out readValue) ? readValue : newDefaultVal;
        }

    }

    public static class WindowsFunctions
    {
        public static bool FatalExceptionsFlag;

        #region DllImports
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern uint GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            uint nSize,
            string lpFileName);
        #endregion DllImports

        // ****** Helper functions *************
        public static bool IsAdmin()
        {
            bool bIsAdmin = false;
            try
            {

                //Check whether the user is part of the administrator group
                AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
                //WindowsIdentity identity = (WindowsIdentity)principal.Identity;

                bIsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (SecurityException ex)
            {
                string exNewMsg = $"Fehler beim Auslesen des Usernamens! \r\n\r\nSystemtext: \r\n{ex.Message}";
                if (FatalExceptionsFlag)
                    throw new Exception(exNewMsg);
            }
            return bIsAdmin;
        }

        public static string GetIniValue(string iniFile, string iniSection, string iniKey)
        {
            uint maxlenValue = 2048;
            string iniValueToReturn = "";
            StringBuilder returnTmpSb = new StringBuilder((int)maxlenValue);
            uint iret = GetPrivateProfileString(iniSection, iniKey, "", returnTmpSb, maxlenValue, iniFile);
            if (iret > 0 && iret < maxlenValue - 2)
            {
                // Correct value found
                iniValueToReturn = returnTmpSb.ToString();
            }
            return iniValueToReturn.ToUpperInvariant();
        }

        public static string GetIniValueCaseSignificant(string iniFile, string iniSection, string iniKey)
        {
            uint maxlenValue = 2048;
            string iniValueToReturn = "";
            StringBuilder returnTmpSb = new StringBuilder((int)maxlenValue);
            uint iret = GetPrivateProfileString(iniSection, iniKey, "", returnTmpSb, maxlenValue, iniFile);
            if (iret > 0 && iret < maxlenValue - 2)
            {
                // Correct patch found
                iniValueToReturn = returnTmpSb.ToString();
            }

            return iniValueToReturn;
        }

    }
}
