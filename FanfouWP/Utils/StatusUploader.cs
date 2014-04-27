using FanfouWP.API.Event;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP.Utils
{
    public class StatusUploader
    {
        private static Parameters parameters;

        private static Random RndSeed = new Random();

        public delegate void StatusUploadSuccessHandler(object sender, EventArgs e);
        public delegate void StatusUploadFailedHandler(object sender, FailedEventArgs e);
        public static event StatusUploadSuccessHandler StatusUploadSuccess;
        public static event StatusUploadFailedHandler StatusUploadFailed;

        public static string UrlEncode(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            StringBuilder buffer = new StringBuilder(text.Length);
            byte[] data = Encoding.UTF8.GetBytes(text);
            foreach (byte b in data)
            {
                char c = (char)b;
                if (!(('0' <= c && c <= '9') || ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z'))
                    && "-_.~".IndexOf(c) == -1)
                {
                    buffer.Append('%' + Convert.ToString(c, 16).ToUpper());
                }
                else
                {
                    buffer.Append(c);
                }
            }
            return buffer.ToString();
        }
        public static string GenerateRndNonce()
        {
            return string.Concat(
            RndSeed.Next(1, 99999999).ToString("00000000"),
            RndSeed.Next(1, 99999999).ToString("00000000"),
            RndSeed.Next(1, 99999999).ToString("00000000"),
            RndSeed.Next(1, 99999999).ToString("00000000"));
        }
        public static void updateStatus(string status, string in_reply_to_status_id = "", string in_reply_to_user_id = "", string repost_status_id = "", string location = "")
        {
            var request = (HttpWebRequest)WebRequest.Create("http://api.fanfou.com/Status/upload.json");

            request.AllowReadStreamBuffering = false;
            request.Method = "Post";

            parameters = new Parameters();
            parameters.Add("status", UrlEncode(status));
            if (in_reply_to_status_id != "")
                parameters.Add("in_reply_to_status_id",UrlEncode( in_reply_to_status_id));
            if (in_reply_to_user_id != "")
                parameters.Add("in_reply_to_user_id",UrlEncode (in_reply_to_user_id));
            if (repost_status_id != "")
                parameters.Add("repost_status_id",UrlEncode(repost_status_id));
            if (location != "")
                parameters.Add("location",UrlEncode(location));

            request.BeginGetRequestStream(GetPostRequestStreamCallback, request);

        }

        private static DateTime UnixTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static long GenerateTimestamp(DateTime time)
        {
            return (long)(time.ToUniversalTime() - UnixTimestamp).TotalSeconds;
        }

        private static string GenerateSignature(string secret, string tokenSecret, string requestMethod, string requestUrl, Parameters parameters)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.UTF8.GetBytes(string.Format("{0}&{1}", UrlEncode(secret), UrlEncode(tokenSecret)));

            StringBuilder data = new StringBuilder(100);
            data.AppendFormat("{0}&{1}&", requestMethod.ToUpper(), UrlEncode(requestUrl));
            //处理参数
            if (parameters != null)
            {
                parameters.Sort();
                data.Append(UrlEncode(parameters.BuildQueryString(true)));
            }


            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(data.ToString());
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

            return UrlEncode(Convert.ToBase64String(hashBytes));
        }
        private static void GetPostRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;

            Stream stream = request.EndGetRequestStream(asynchronousResult);

            Parameters oParameters = new Parameters();
            oParameters.Add("oauth_consumer_key", FanfouWP.API.FanfouConsts.CONSUMER_KEY);
            oParameters.Add("oauth_token", FanfouWP.API.FanfouAPI.Instance.oauthToken);
            oParameters.Add("oauth_signature_method", "HMAC-SHA1");
            oParameters.Add("oauth_timestamp", GenerateTimestamp(DateTime.Now).ToString());
            oParameters.Add("oauth_nonce", GenerateRndNonce());
            oParameters.Add("oauth_version", "1.0");

            foreach (var p in parameters.Items)
                oParameters.Add(p.Key, p.Value);

            oParameters.Add("oauth_signature", GenerateSignature(FanfouWP.API.FanfouConsts.CONSUMER_SECRET, FanfouWP.API.FanfouAPI.Instance.oauthSecret, "POST", "http://api.fanfou.com/Status/upload.json", oParameters));

            foreach (var p in parameters.Items)
                oParameters.Items.Remove(p);

            string oauth = "";
            for (int i = 0; i < oParameters.Items.Count - 1; i++)
            {
                oauth += oParameters.Items[i].Key + "=\"" + oParameters.Items[i].Value + '"' + ",";
            }
            oauth += oParameters.Items[oParameters.Items.Count - 1].Key + "=\"" + oParameters.Items[oParameters.Items.Count - 1].Value + '"';
            request.Headers["Authorization"] = "OAuth " + oauth;

            request.ContentType = "application/x-www-form-urlencoded";

            using (var ms = new MemoryStream())
            {
                foreach (var p in parameters.Items)
                {
                    string item = "";
                    if (p.Key != parameters.Items.Last().Key)
                        item = string.Format("{0}={1}&", p.Key, p.Value);
                    else
                        item = string.Format("{0}={1}", p.Key, p.Value);
                    byte[] data = Encoding.UTF8.GetBytes(item);
                    ms.Write(data, 0, data.Length);
                }

                ms.Position = 0;
                ms.WriteTo(stream);
                stream.Close();
            }


            request.BeginGetResponse(AsyncResponseCallback, request);
        }

        private static void AsyncResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                var request = (HttpWebRequest)asynchronousResult.AsyncState;
                var response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                Stream streamResponse = response.GetResponseStream();
                var streamRead = new StreamReader(streamResponse);
                string responseString = streamRead.ReadToEnd();
                response.Close();

                FanfouWP.API.Items.Status s = new FanfouWP.API.Items.Status();
                var ds = new DataContractJsonSerializer(s.GetType());
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(responseString));
                s = ds.ReadObject(ms) as FanfouWP.API.Items.Status;
                ms.Close();
                var e = new EventArgs();
                StatusUploadSuccess(s, e);
            }
            catch (Exception)
            {
                StatusUploadFailed(null, new FailedEventArgs());
            }
        }

    }
}
