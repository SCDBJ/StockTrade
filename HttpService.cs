using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StockTradingRecord
{
    public class HttpService
    {
        public static string HttpPost(string url, Dictionary<string, string> headerDic = null, string param = "")
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.Timeout = 40000;
            request.ContentType = "application/json";
            if (headerDic != null)
            {
                foreach (var d in headerDic)
                {
                    request.Headers.Add(d.Key, d.Value);
                }
            }

            //request.ServicePoint.Expect100Continue = false;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //ServicePointManager.DefaultConnectionLimit = 50;

            try
            {
                if (!string.IsNullOrWhiteSpace(param))
                {
                    byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(param);
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                HttpWebResponse res = (HttpWebResponse)request.GetResponse();
                //System.GC.Collect();
                StreamReader streamReader = new StreamReader(res.GetResponseStream());
                string responseContent = streamReader.ReadToEnd();

                streamReader.Close();
                request.Abort();
                res.Close();
                return responseContent;
            }
            catch (Exception ex)
            {
                //LogLib.LogHelper.Error("[HttpClientPost]Exception:" + ex.Message);
            }
            return null;
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
