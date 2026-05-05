using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StockTradingRecord
{
    public class SignService
    {
        private static string CreateSignature(Dictionary<string, object> parameters, string secretKey)
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, object> kvp in parameters.OrderBy(r => r.Key))
            {
                list.Add(kvp.Key + "=" + kvp.Value + "&");
            }
            list.Sort(StringComparer.Ordinal);
            StringBuilder sb = new StringBuilder();
            foreach (string param in list)
            {
                sb.Append(param);
            }
            string message = sb.ToString().TrimEnd('&');
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);
            var base64Hash = Convert.ToBase64String(hashBytes);
            string hashString = Uri.EscapeDataString(base64Hash);
            return hashString;
        }
        public static string CalcSignature(string clientId, string secretKey, long nonce, long timestamp, string request)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("clientId", clientId);
            dic.Add("nonce", nonce);
            dic.Add("timestamp", timestamp);
            dic.Add("request", request);

            string sign = CreateSignature(dic, secretKey);
            return sign;
        }
    }
}
