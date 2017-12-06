using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Security;

namespace Northwind.Security
{
    public class UserAccount
    {
        public static string HashSHA1(string value)
        {
            //Instance of Encryption
            var sha1 = System.Security.Cryptography.SHA1.Create();
            //String -> Byte conversion
            var inputBytes = Encoding.ASCII.GetBytes(value);
            //Byte Array -> Hash
            var hash = sha1.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static Int32 GetUserID()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            return Convert.ToInt32(ticket.Name);
        }
    }
}