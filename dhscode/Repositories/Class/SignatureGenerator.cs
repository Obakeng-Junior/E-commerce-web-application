// SignatureGenerator.cs
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Web;

namespace DHSOnlineStore
{
    public class SignatureGenerator
    {
        public static string GenerateSignature(Dictionary<string, string> data, string passphrase = null)
        {
            // Create parameter string
            StringBuilder pfOutput = new StringBuilder();

            foreach (var kvp in data)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    pfOutput.Append(kvp.Key)
                            .Append('=')
                            .Append(Uri.EscapeDataString(kvp.Value.Trim()))
                            .Append('&');
                }
            }

            // Remove the last ampersand
            string getString = pfOutput.ToString().TrimEnd('&');

            // Append passphrase if it exists
            if (!string.IsNullOrEmpty(passphrase))
            {
                getString += "&passphrase=" + Uri.EscapeDataString(passphrase.Trim());
            }

            // Generate MD5 hash of the string
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(getString));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
