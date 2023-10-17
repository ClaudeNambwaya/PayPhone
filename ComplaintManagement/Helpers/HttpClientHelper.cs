using Newtonsoft.Json.Linq;
using System.Net;

namespace ComplaintManagement.Helpers
{
    public static class HttpClientHelper
    {
        public class HttpHandler
        {
            public string HttpClientPost(string url, JObject request_data)
            {
                string result = null;

                try
                {
                    var httpRequest = (HttpWebRequest)WebRequest.Create(url)!;
                    httpRequest.Method = "post";
                    httpRequest.ContentType = "application/json";
                    using (var dataStream = new StreamWriter(httpRequest.GetRequestStream()))
                    {
                        dataStream.Write(request_data);
                        dataStream.Flush();
                        dataStream.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    FileLogHelper.log_message_fields("ERROR", "HttpClientPost | Exception ->" + ex.Message);
                }

                return result;
            }
        }
    }
}