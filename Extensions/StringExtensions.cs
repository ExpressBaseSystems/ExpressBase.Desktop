using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExpressBase.Desktop.Extensions
{
    public static partial class StringExtensions
    {
        public static string ToMD5Hash(this string @this)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(@this));
                var sb = new StringBuilder();
                foreach (byte bytes in hashBytes)
                {
                    sb.Append(bytes.ToString("X2"));
                }

                return sb.ToString().ToLower();
            }
        }
    }
} 