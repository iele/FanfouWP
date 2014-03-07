using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hammock;
using System.Net;
using FanfouWP.API.Event;
using System.Runtime.Serialization.Json;
using System.IO;
using Hammock.Authentication.OAuth;

namespace FanfouWP.API
{

    public class FanfouAPI
    {
        private string oauthToken;
        private string oauthSecret;

        private string username;
        private string password;

        public Items.User CurrentUser { get; set; }
        public List<Items.Status> HomeTimeLineStatus { get; set; }
        public List<Items.Status> PublicTimeLineStatus { get; set; }
        public List<Items.Status> MentionTimeLineStatus { get; set; }


        public delegate void LoginSuccessHandler(object sender, EventArgs e);
        public delegate void LoginFailedHandler(object sender, FailedEventArgs e);
        public delegate void VerifyCredentialsSuccessHandler(object sender, EventArgs e);
        public delegate void VerifyCredentialsFailedHandler(object sender, FailedEventArgs e);

        public delegate void HomeTimelineSuccessHandler(object sender, EventArgs e);
        public delegate void HomeTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void PublicTimelineSuccessHandler(object sender, EventArgs e);
        public delegate void PublicTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void MentionTimelineSuccessHandler(object sender, EventArgs e);
        public delegate void MentionTimelineFailedHandler(object sender, FailedEventArgs e);

        public event LoginSuccessHandler LoginSuccess;
        public event LoginFailedHandler LoginFailed;
        public event VerifyCredentialsSuccessHandler VerifyCredentialsSuccess;
        public event VerifyCredentialsFailedHandler VerifyCredentialsFailed;

        public event HomeTimelineSuccessHandler HomeTimelineSuccess;
        public event HomeTimelineFailedHandler HomeTimelineFailed;
        public event PublicTimelineSuccessHandler PublicTimelineSuccess;
        public event PublicTimelineFailedHandler PublicTimelineFailed;
        public event MentionTimelineSuccessHandler MentionTimelineSuccess;
        public event MentionTimelineFailedHandler MentionTimelineFailed;

        private static FanfouAPI instance;
        public static FanfouAPI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FanfouAPI();
                }
                return instance;
            }
        }
        private FanfouAPI() { }

        private RestClient GetClient()
        {
            if (oauthToken != null && oauthSecret != null)
            {
                var client = new Hammock.RestClient
                {
                    Authority = FanfouConsts.API_URL,
                    Credentials = new Hammock.Authentication.OAuth.OAuthCredentials
                    {
                        Type = OAuthType.ProtectedResource,
                        ConsumerKey = FanfouConsts.CONSUMER_KEY,
                        ConsumerSecret = FanfouConsts.CONSUMER_SECRET,
                        SignatureMethod = Hammock.Authentication.OAuth.OAuthSignatureMethod.HmacSha1,
                        ParameterHandling = Hammock.Authentication.OAuth.OAuthParameterHandling.HttpAuthorizationHeader,
                        Version = "1.0",
                        Token = oauthToken,
                        TokenSecret = oauthSecret,
                        ClientUsername = username,
                        ClientPassword = password
                    }
                };
                return client;
            }
            else { return null; }
        }

        public void Login(string username, string password)
        {
            this.username = username;
            this.password = password;

            var client = new Hammock.RestClient
            {
                Authority = FanfouConsts.BASE_URL,
                Credentials = new Hammock.Authentication.OAuth.OAuthCredentials
                {
                    ConsumerKey = FanfouConsts.CONSUMER_KEY,
                    ConsumerSecret = FanfouConsts.CONSUMER_SECRET,
                    SignatureMethod = Hammock.Authentication.OAuth.OAuthSignatureMethod.HmacSha1,
                    ParameterHandling = Hammock.Authentication.OAuth.OAuthParameterHandling.HttpAuthorizationHeader,
                    Version = "1.0"
                }
            };

            client.AddHeader("content-type", "application/x-www-form-urlencoded");

            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.ACCESS_TOKEN,
                Method = Hammock.Web.WebMethod.Post
            };
            restRequest.AddParameter("x_auth_mode", "client_auth");
            restRequest.AddParameter("x_auth_username", username);
            restRequest.AddParameter("x_auth_password", password);

            client.BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string[] content = response.Content.Split(new char[] { '=', '&' });
                    oauthToken = content[1];
                    oauthSecret = content[3];

                    EventArgs e = new EventArgs();
                    LoginSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    LoginFailed(this, e);
                }
            });
        }
        public void VerifyCredentials()
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.VERIFY_CREDENTIALS,
                Method = Hammock.Web.WebMethod.Get
            };

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Items.User user = new Items.User();
                    var ds = new DataContractJsonSerializer(user.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    user = ds.ReadObject(ms) as Items.User;
                    ms.Close();
                    this.CurrentUser = user;
                    EventArgs e = new EventArgs();
                    VerifyCredentialsSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    VerifyCredentialsFailed(this, e);
                }
            });
        }

        public void StatusHomeTimeline()
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.STATUS_HOME_TIMELINE,
                Method = Hammock.Web.WebMethod.Get
            };

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    List<Items.Status> status = new List<Items.Status>();
                    var ds = new DataContractJsonSerializer(status.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    status = ds.ReadObject(ms) as List<Items.Status>;
                    ms.Close();
                    this.HomeTimeLineStatus = status;
                    EventArgs e = new EventArgs();
                    HomeTimelineSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    HomeTimelineFailed(this, e);
                }
            });
        }

        public void StatusPublicTimeline()
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.STATUS_PUBLIC_TIMELINE,
                Method = Hammock.Web.WebMethod.Get
            };


            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    List<Items.Status> status = new List<Items.Status>();

                    var ds = new DataContractJsonSerializer(status.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    status = ds.ReadObject(ms) as List<Items.Status>;
                    ms.Close();

                    this.PublicTimeLineStatus = status;
                    EventArgs e = new EventArgs();
                    PublicTimelineSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    PublicTimelineFailed(this, e);
                }
            });
        }
        public void StatusMentionTimeline()
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.STATUS_MENTION_TIMELINE,
                Method = Hammock.Web.WebMethod.Get
            };


            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    List<Items.Status> status = new List<Items.Status>();

                    var ds = new DataContractJsonSerializer(status.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    status = ds.ReadObject(ms) as List<Items.Status>;
                    ms.Close();

                    this.MentionTimeLineStatus = status;
                    EventArgs e = new EventArgs();
                    MentionTimelineSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    MentionTimelineFailed(this, e);
                }
            });
        }

    }


}
