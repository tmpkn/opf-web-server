//
// opf-web additions / tompaw@tompaw.net / 2023-01-01
// https://tompaw.net/opf-web/
//

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OPFService
{
    public class PwnedClient
    {
        private static string pwdPwnedUrl = ConfigurationManager.AppSettings.Get("pwnedApiEndpoint");   // WEB APP URL for Password Approval (arg = pw hash)
        private static string resetVendorPwdUrl = ConfigurationManager.AppSettings.Get("resetVendorPwdEndpoint");   // WEB APP URL for User Notify & Corporate Post-Processing (args = user + telephone number)
        private static readonly HttpClient client = new HttpClient();
        public static bool IsPwned(string pwd)
        {
            var hashedPwd = Hash(pwd);
            try
            {
                Helpers.Log($"Pwned API Hashed Pwd: {hashedPwd}", EventLogEntryType.Information, 40);
                var response = Get($"{pwdPwnedUrl}{hashedPwd.ToUpper()}");
                if (response.ToLower() == "true")
                    return true;
                else if (response.ToLower() == "false")
                    return false;
                else
                    Helpers.Log($"Pwned API Error: {response}", EventLogEntryType.Error, 40);
                return true;
            }
            catch (Exception e)
            {
                Helpers.Log($"Pwned API Exception: {e.Message}", EventLogEntryType.Error, 40);
                return false;
            }
        }
        public static string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
        static string Hash(string input)
        {
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        public static string UpdateExternalProviderPasswordsAsync(string logon, string phone)
        {
            var result = "";
            try
            {
                var url = $"{resetVendorPwdUrl}{logon}/{phone}";
                var task = Task.Run(async () => await client.PostAsync(url, null));
                Helpers.Log($"Reset Vendor Password Api Returned StatusCode: {task.Result.StatusCode}. URL: {url}", EventLogEntryType.Information, 45);
            }
            catch (Exception ex)
            {
                Helpers.Log($"Reset Vendor Password Api Exception: {ex.Message}", EventLogEntryType.Error, 45);
            }
            return result;
        }
    }
}
