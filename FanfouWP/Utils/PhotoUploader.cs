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
    class PhotoUploader
    {
        private static byte[] fs;

        private static Parameters parameters;

        private static Random RndSeed = new Random();

        public delegate void PhotosUploadSuccessHandler(object sender, EventArgs e);
        public delegate void PhotosUploadFailedHandler(object sender, FailedEventArgs e);
        public static event PhotosUploadSuccessHandler PhotosUploadSuccess;
        public static event PhotosUploadFailedHandler PhotosUploadFailed;

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
        public static void uploadPhoto(byte[] fs, string status = "", string location = "")
        {
            var request = (HttpWebRequest)WebRequest.Create("http://api.fanfou.com/photos/upload.json");

            request.AllowReadStreamBuffering = false;
            request.Method = "Post";

            parameters = new Parameters();
            parameters.Add("status", status);
            if (location != "")
                parameters.Add("location", location);

            PhotoUploader.fs = fs;

            //  request.Headers["Authorization"] = AuthHeader;

            request.BeginGetRequestStream(GetPostFileRequestStreamCallback, request);

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
        private static void GetPostFileRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;

            Stream stream = request.EndGetRequestStream(asynchronousResult);

            var oParameters = new Parameters();
            oParameters.Add("oauth_consumer_key", FanfouWP.API.FanfouConsts.CONSUMER_KEY);
            oParameters.Add("oauth_token", FanfouWP.API.FanfouAPI.Instance.oauthToken);
            oParameters.Add("oauth_signature_method", "HMAC-SHA1");
            oParameters.Add("oauth_timestamp", GenerateTimestamp(DateTime.Now).ToString());
            oParameters.Add("oauth_nonce", GenerateRndNonce());
            oParameters.Add("oauth_version", "1.0");

            oParameters.Add("oauth_signature", GenerateSignature(FanfouWP.API.FanfouConsts.CONSUMER_SECRET, FanfouWP.API.FanfouAPI.Instance.oauthSecret, "POST", "http://api.fanfou.com/photos/upload.json", oParameters));

            string oauth = "";
            for (int i = 0; i < oParameters.Items.Count - 1; i++)
            {
                oauth += oParameters.Items[i].Key + "=\"" + oParameters.Items[i].Value + '"' + ",";
            }
            oauth += oParameters.Items[oParameters.Items.Count - 1].Key + "=\"" + oParameters.Items[oParameters.Items.Count - 1].Value + '"';
            request.Headers["Authorization"] = "OAuth " + oauth;


            string boundary = string.Concat("-----------------------------", GenerateRndNonce());
            request.ContentType = string.Concat("multipart/form-data; boundary=", boundary);

            using (var ms = new MemoryStream())
            {
                byte[] boundaryData = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");

                const string parameterData = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                foreach (var p in parameters.Items)
                {
                    string item = string.Format(parameterData, p.Key, p.Value);
                    byte[] data = Encoding.UTF8.GetBytes(item);
                    ms.Write(boundaryData, 0, boundaryData.Length);
                    ms.Write(data, 0, data.Length);
                }
                const string fileData =
                     "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";

                string item2 = string.Format(fileData, "photo", "photo.jpeg", "image/jpeg");
                byte[] data2 = Encoding.UTF8.GetBytes(item2);
                ms.Write(boundaryData, 0, boundaryData.Length);
                ms.Write(data2, 0, data2.Length);

                ms.Write(fs, 0, (int)fs.Length);

                boundaryData = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                ms.Write(boundaryData, 0, boundaryData.Length);
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
                streamResponse.Close();
                streamRead.Close();
                response.Close();

                FanfouWP.API.Items.Status s = new FanfouWP.API.Items.Status();
                var ds = new DataContractJsonSerializer(s.GetType());
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(responseString));
                s = ds.ReadObject(ms) as FanfouWP.API.Items.Status;
                ms.Close();
                var e = new EventArgs();
                PhotosUploadSuccess(s, e);
            }
            catch (Exception)
            {
                PhotosUploadFailed(null, new FailedEventArgs());
            }
        }
    }

    /// </summary>
    internal class Parameters
    {
        /// <summary>
        ///
        /// </summary>
        public Parameters()
        {
            this.Items = new List<KeyValuePair<string, string>>(10);
        }

        /// <summary>
        /// 参数
        /// </summary>
        public List<KeyValuePair<string, string>> Items
        {
            get;
            private set;
        }
        /// <summary>
        /// 清空参数
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            this.Items.Clear();
        }

        /// <summary>
        /// 排序
        /// </summary>
        public void Sort()
        {
            this.Items.Sort(new Comparison<KeyValuePair<string, string>>((x1, x2) =>
            {
                if (x1.Key == x2.Key)
                {
                    return string.Compare(x1.Value, x2.Value);
                }
                else
                {
                    return string.Compare(x1.Key, x2.Key);
                }
            }));
        }

        /// <summary>
        /// 添加查询参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            this.Add(key, (value == null ? string.Empty : value.ToString()));
        }
        /// <summary>
        /// 添加查询参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            this.Items.Add(new KeyValuePair<string, string>(key, value));
        }

        /// <summary>
        /// 构造查询参数字符串
        /// </summary>
        /// <param name="encodeValue">是否对值进行编码</param>
        /// <returns></returns>
        public string BuildQueryString(bool encodeValue)
        {
            StringBuilder buffer = new StringBuilder();
            foreach (var p in this.Items)
            {
                if (buffer.Length != 0) buffer.Append("&");
                buffer.AppendFormat("{0}={1}", encodeValue ? PhotoUploader.UrlEncode(p.Key) : p.Key, encodeValue ? PhotoUploader.UrlEncode(p.Value) : p.Value);
            }
            return buffer.ToString();
        }
    }
}
