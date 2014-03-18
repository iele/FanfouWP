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
using System.Collections.ObjectModel;
using FanfouWP.Storage;

namespace FanfouWP.API
{
    public class FanfouAPI
    {
        private string oauthToken;
        private string oauthSecret;

        private string username;
        private string password;

        public enum RefreshMode { New, Behind, Back };

        private TimelineStorage<Items.Status> storage = new TimelineStorage<Items.Status>();

        public Items.User CurrentUser { get; set; }
        public ObservableCollection<Items.Status> HomeTimeLineStatus { get; set; }
        public string firstHomeTimeLineStatusId { get { return HomeTimeLineStatus.First().id; } private set { } }
        public string lastHomeTimeLineStatusId { get { return HomeTimeLineStatus.Last().id; } private set { } }
        public ObservableCollection<Items.Status> PublicTimeLineStatus { get; set; }
        public string firstPublicTimeLineStatusId { get { return PublicTimeLineStatus.First().id; } private set { } }
        public string lastPublicTimeLineStatussId { get { return PublicTimeLineStatus.Last().id; } private set { } }
        public ObservableCollection<Items.Status> MentionTimeLineStatus { get; set; }
        public string firstMentionTimeLineStatusId { get { return MentionTimeLineStatus.First().id; } private set { } }
        public string lastMentionTimeLineStatusId { get { return MentionTimeLineStatus.Last().id; } private set { } }


        public delegate void RestoreDataSuccessHandler(object sender, EventArgs e);
        public delegate void RestoreDataFailedHandler(object sender, FailedEventArgs e);
        public event RestoreDataSuccessHandler RestoreDataSuccess;
        public event RestoreDataFailedHandler RestoreDataFailed;


        public delegate void LoginSuccessHandler(object sender, EventArgs e);
        public delegate void LoginFailedHandler(object sender, FailedEventArgs e);
        public delegate void VerifyCredentialsSuccessHandler(object sender, EventArgs e);
        public delegate void VerifyCredentialsFailedHandler(object sender, FailedEventArgs e);
        public delegate void StatusUpdateSuccessHandler(object sender, EventArgs e);
        public delegate void StatusUpdateFailedHandler(object sender, FailedEventArgs e);

        public event LoginSuccessHandler LoginSuccess;
        public event LoginFailedHandler LoginFailed;
        public event VerifyCredentialsSuccessHandler VerifyCredentialsSuccess;
        public event VerifyCredentialsFailedHandler VerifyCredentialsFailed;
        public event StatusUpdateSuccessHandler StatusUpdateSuccess;
        public event StatusUpdateFailedHandler StatusUpdateFailed;

        public delegate void HomeTimelineSuccessHandler(object sender, EventArgs e);
        public delegate void HomeTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void PublicTimelineSuccessHandler(object sender, EventArgs e);
        public delegate void PublicTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void MentionTimelineSuccessHandler(object sender, EventArgs e);
        public delegate void MentionTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void UserTimelineSuccessHandler(object sender, UserTimelineEventArgs<Items.Status> e);
        public delegate void UserTimelineFailedHandler(object sender, FailedEventArgs e);

        public event HomeTimelineSuccessHandler HomeTimelineSuccess;
        public event HomeTimelineFailedHandler HomeTimelineFailed;
        public event PublicTimelineSuccessHandler PublicTimelineSuccess;
        public event PublicTimelineFailedHandler PublicTimelineFailed;
        public event MentionTimelineSuccessHandler MentionTimelineSuccess;
        public event MentionTimelineFailedHandler MentionTimelineFailed;
        public event UserTimelineSuccessHandler UserTimelineSuccess;
        public event UserTimelineFailedHandler UserTimelineFailed;

        public delegate void FavoritesCreateSuccessHandler(object sender, EventArgs e);
        public delegate void FavoritesCreateFailedHandler(object sender, FailedEventArgs e);
        public delegate void FavoritesDestroySuccessHandler(object sender, EventArgs e);
        public delegate void FavoritesDestroyFailedHandler(object sender, FailedEventArgs e);
        public delegate void FavoritesSuccessHandler(object sender, EventArgs e);
        public delegate void FavoritesFailedHandler(object sender, FailedEventArgs e);

        public event FavoritesCreateSuccessHandler FavoritesCreateSuccess;
        public event FavoritesCreateFailedHandler FavoritesCreateFailed;
        public event FavoritesDestroySuccessHandler FavoritesDestroySuccess;
        public event FavoritesDestroyFailedHandler FavoritesDestroyFailed;
        public event FavoritesSuccessHandler FavoritesSuccess;
        public event FavoritesFailedHandler FavoritesFailed;

        public delegate void SearchTimelineSuccessHandler(object sender, UserTimelineEventArgs<Items.Status> e);
        public delegate void SearchTimelineFailedHandler(object sender, FailedEventArgs e);

        public event SearchTimelineSuccessHandler SearchTimelineSuccess;
        public event SearchTimelineFailedHandler SearchTimelineFailed;

        public delegate void TrendsListSuccessHandler(object sender,TrendsListEventArgs e);
        public delegate void TrendsListFailedHandler(object sender, FailedEventArgs e);

        public event TrendsListSuccessHandler TrendsListSuccess;
        public event TrendsListFailedHandler TrendsListFailed;

        public delegate void TagListSuccessHandler(object sender, ListEventArgs<string> e);
        public delegate void TagListFailedHandler(object sender, FailedEventArgs e);
        public delegate void TaggedSuccessHandler(object sender, UserTimelineEventArgs<Items.User> e);
        public delegate void TaggedFailedHandler(object sender, FailedEventArgs e);

        public event TagListSuccessHandler TagListSuccess;
        public event TagListFailedHandler TagListFailed;
        public event TaggedSuccessHandler TaggedSuccess;
        public event TaggedFailedHandler TaggedFailed;


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
        private FanfouAPI()
        {
            this.HomeTimeLineStatus = new ObservableCollection<Items.Status>();
            this.PublicTimeLineStatus = new ObservableCollection<Items.Status>();
            this.MentionTimeLineStatus = new ObservableCollection<Items.Status>();

           storage.WriteDataSuccess += JsonStorage_WriteDataSuccess;
           storage.WriteDataFailed += JsonStorage_WriteDataFailed;
           storage.ReadDataSuccess += JsonStorage_ReadDataSuccess;
           storage.ReadDataFailed += JsonStorage_ReadDataFailed;
        }
        
        void JsonStorage_ReadDataSuccess(object sender, UserTimelineEventArgs<Items.Status> e)
        {
            switch (sender as string) { 
                case "home":
                    this.HomeTimeLineStatus = e.UserStatus;
                    break;
                case "public":
                    this.PublicTimeLineStatus = e.UserStatus;
                    break;
                case "mention":
                    this.MentionTimeLineStatus = e.UserStatus;
                    break;            
            }
        }

        void JsonStorage_WriteDataFailed(object sender, FailedEventArgs e)
        {
        }

        void JsonStorage_ReadDataFailed(object sender, FailedEventArgs e)
        {
        }

        void JsonStorage_WriteDataSuccess(object sender, EventArgs e)
        {
        }

        private void HomeTimeLineStatusChanged()
        {
           storage.SaveDataToIsolatedStorage("home", this.HomeTimeLineStatus);
        }
        private void PublicTimeLineStatusChanged()
        {
           storage.SaveDataToIsolatedStorage("public", this.PublicTimeLineStatus);
        }

        private void MentionTimeLineStatusChanged()
        {
           storage.SaveDataToIsolatedStorage("mention", this.MentionTimeLineStatus);
        }
        public void TryRestoreData()
        {
            settings.RestoreSettings();
            if (settings.username != null && settings.password != null)
            {
                this.username = settings.username;
                this.password = settings.password;
                this.oauthToken = settings.oauthToken;
                this.oauthSecret = settings.oauthSecret;

                this.LoginSuccess += (o, e) => { };
                this.LoginFailed += (o, e) => { };
                this.Login(username, password);
               storage.ReadDataFromIsolatedStorage("home");
               storage.ReadDataFromIsolatedStorage("public");
               storage.ReadDataFromIsolatedStorage("mention");

               RestoreDataSuccess(this, new EventArgs());
            }
            RestoreDataFailed(this, new FailedEventArgs());
        }

        private SettingManager settings = SettingManager.GetInstance();

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
            else
            {
                return null;
            }
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

                    settings.oauthToken = oauthToken;
                    settings.oauthSecret = oauthSecret;
                    settings.username = username;
                    settings.password = password;
                    settings.SaveSettings();

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

        public void StatusUpdate(string status, string in_reply_to_status_id = "", string in_reply_to_user_id = "", string repost_status_id = "", string location = "")
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.STATUS_UPDATE,
                Method = Hammock.Web.WebMethod.Post
            };

            var client = GetClient();
            client.AddHeader("content-type", "application/x-www-form-urlencoded");
            restRequest.AddParameter("status", status);
            if (in_reply_to_status_id != "")
                restRequest.AddParameter("in_reply_to_status_id", in_reply_to_status_id);
            if (in_reply_to_user_id != "")
                restRequest.AddParameter("in_reply_to_user_id", in_reply_to_user_id);
            if (repost_status_id != "")
                restRequest.AddParameter("repost_status_id", repost_status_id);
            if (location != "")
                restRequest.AddParameter("location", location);

            client.BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Items.Status s = new Items.Status();
                    var ds = new DataContractJsonSerializer(s.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    s = ds.ReadObject(ms) as Items.Status;
                    ms.Close();
                    var l = new ObservableCollection<Items.Status>();
                    l.Add(s);
                    foreach (var item in HomeTimeLineStatus)
                        l.Add(item);
                    this.HomeTimeLineStatus = l;
                    HomeTimeLineStatusChanged();
                    EventArgs e = new EventArgs();
                    StatusUpdateSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    StatusUpdateFailed(this, e);
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
        #region status
        public void StatusUserTimeline(string user_id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.STATUS_USER_TIMELINE,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("user_id", user_id);

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ObservableCollection<Items.Status> status = new ObservableCollection<Items.Status>();
                    var ds = new DataContractJsonSerializer(status.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    status = ds.ReadObject(ms) as ObservableCollection<Items.Status>;
                    ms.Close();

                    UserTimelineEventArgs<Items.Status> e = new UserTimelineEventArgs<Items.Status>();
                    e.UserStatus = status;
                    UserTimelineSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    UserTimelineFailed(this, e);
                }
            });
        }
        public void StatusHomeTimeline(RefreshMode mode = RefreshMode.New)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.STATUS_HOME_TIMELINE,
                Method = Hammock.Web.WebMethod.Get
            };
            switch (mode)
            {
                case RefreshMode.New:
                    break;
                case RefreshMode.Behind:
                    restRequest.AddParameter("since_id", firstHomeTimeLineStatusId);
                    break;
                case RefreshMode.Back:
                    restRequest.AddParameter("max_id", lastHomeTimeLineStatusId);
                    break;
                default: break;
            }

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ObservableCollection<Items.Status> status = new ObservableCollection<Items.Status>();
                    var ds = new DataContractJsonSerializer(status.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    status = ds.ReadObject(ms) as ObservableCollection<Items.Status>;
                    ms.Close();

                    var l = new ObservableCollection<Items.Status>();
                    switch (mode)
                    {
                        case RefreshMode.New:
                            l = status;
                            break;
                        case RefreshMode.Behind:
                            foreach (var item in status)
                                l.Add(item);
                            foreach (var item in HomeTimeLineStatus)
                                l.Add(item);
                            break;
                        case RefreshMode.Back:
                            foreach (var item in HomeTimeLineStatus)
                                l.Add(item);
                            foreach (var item in status)
                                l.Add(item);
                            break;
                        default: break;
                    }
                    this.HomeTimeLineStatus = l;
                    HomeTimeLineStatusChanged();
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

        public void StatusPublicTimeline(RefreshMode mode = RefreshMode.New)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.STATUS_PUBLIC_TIMELINE,
                Method = Hammock.Web.WebMethod.Get
            };
            switch (mode)
            {
                case RefreshMode.New:
                    break;
                case RefreshMode.Behind:
                    restRequest.AddParameter("since_id", firstPublicTimeLineStatusId);
                    break;
                case RefreshMode.Back:
                    restRequest.AddParameter("max_id", lastPublicTimeLineStatussId);
                    break;
                default: break;
            }
            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ObservableCollection<Items.Status> status = new ObservableCollection<Items.Status>();

                    var ds = new DataContractJsonSerializer(status.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    status = ds.ReadObject(ms) as ObservableCollection<Items.Status>;
                    ms.Close();

                    var l = new ObservableCollection<Items.Status>();
                    switch (mode)
                    {
                        case RefreshMode.New:
                            l = status;
                            break;
                        case RefreshMode.Behind:
                            foreach (var item in status)
                                l.Add(item);
                            foreach (var item in PublicTimeLineStatus)
                                l.Add(item);
                            break;
                        case RefreshMode.Back:
                            foreach (var item in PublicTimeLineStatus)
                                l.Add(item);
                            foreach (var item in status)
                                l.Add(item);
                            break;
                        default: break;
                    }
                    this.PublicTimeLineStatus = l;
                    PublicTimeLineStatusChanged();
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
        public void StatusMentionTimeline(RefreshMode mode = RefreshMode.New)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.STATUS_MENTION_TIMELINE,
                Method = Hammock.Web.WebMethod.Get
            };
            switch (mode)
            {
                case RefreshMode.New:
                    break;
                case RefreshMode.Behind:
                    restRequest.AddParameter("since_id", firstMentionTimeLineStatusId);
                    break;
                case RefreshMode.Back:
                    restRequest.AddParameter("max_id", lastMentionTimeLineStatusId);
                    break;
                default: break;
            }
            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ObservableCollection<Items.Status> status = new ObservableCollection<Items.Status>();

                    var ds = new DataContractJsonSerializer(status.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    status = ds.ReadObject(ms) as ObservableCollection<Items.Status>;
                    ms.Close();
                    var l = new ObservableCollection<Items.Status>();
                    switch (mode)
                    {
                        case RefreshMode.New:
                            l = status;
                            break;
                        case RefreshMode.Behind:
                            foreach (var item in status)
                                l.Add(item);
                            foreach (var item in MentionTimeLineStatus)
                                l.Add(item);
                            break;
                        case RefreshMode.Back:
                            foreach (var item in MentionTimeLineStatus)
                                l.Add(item);
                            foreach (var item in status)
                                l.Add(item);
                            break;
                        default: break;
                    }
                    this.MentionTimeLineStatus = l;
                    MentionTimeLineStatusChanged();
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
        #endregion

        #region favorites

        public void FavoritesId(string id, int page = 1)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.FAVORITES_ID + HttpUtility.UrlEncode(id) + ".json",
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("page", page.ToString());

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ObservableCollection<Items.Status> status = new ObservableCollection<Items.Status>();
                    var ds = new DataContractJsonSerializer(status.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    status = ds.ReadObject(ms) as ObservableCollection<Items.Status>;
                    ms.Close();

                    UserTimelineEventArgs<Items.Status> e = new UserTimelineEventArgs<Items.Status>();
                    e.UserStatus = status;
                    FavoritesSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    FavoritesFailed(this, e);
                }
            });
        }
        public void FavoritesCreate(string id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.FAVORITES_CREATE_ID + id + ".json",
                Method = Hammock.Web.WebMethod.Post
            };

            var client = GetClient();
            client.AddHeader("content-type", "application/x-www-form-urlencoded");

            client.BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Items.Status s = new Items.Status();
                    var ds = new DataContractJsonSerializer(s.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    s = ds.ReadObject(ms) as Items.Status;
                    ms.Close();

                    if (this.HomeTimeLineStatus != null)
                    {
                        var items = from status in this.HomeTimeLineStatus where status.id == s.id select status;
                        foreach (var item in items)
                        {
                            item.favorited = true;
                        }
                    }
                    if (this.MentionTimeLineStatus != null)
                    {
                        var items = from status in this.MentionTimeLineStatus where status.id == s.id select status;
                        foreach (var item in items)
                        {
                            item.favorited = true;
                        }
                    }
                    if (this.PublicTimeLineStatus != null)
                    {

                        var items = from status in this.PublicTimeLineStatus where status.id == s.id select status;
                        foreach (var item in items)
                        {
                            item.favorited = true;
                        }
                    }
                    EventArgs e = new EventArgs();
                    FavoritesCreateSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    FavoritesCreateFailed(this, e);
                }
            });
        }

        public void FavoritesDestroy(string id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.FAVORITES_DESTROY_ID + id + ".json",
                Method = Hammock.Web.WebMethod.Post
            };

            var client = GetClient();
            client.AddHeader("content-type", "application/x-www-form-urlencoded");

            client.BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Items.Status s = new Items.Status();
                    var ds = new DataContractJsonSerializer(s.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    s = ds.ReadObject(ms) as Items.Status;
                    ms.Close();

                    if (this.HomeTimeLineStatus != null)
                    {
                        var items = from status in this.HomeTimeLineStatus where status.id == s.id select status;
                        foreach (var item in items)
                        {
                            item.favorited = false;
                        }
                    }
                    if (this.MentionTimeLineStatus != null)
                    {
                        var items = from status in this.MentionTimeLineStatus where status.id == s.id select status;
                        foreach (var item in items)
                        {
                            item.favorited = false;
                        }
                    }
                    if (this.PublicTimeLineStatus != null)
                    {

                        var items = from status in this.PublicTimeLineStatus where status.id == s.id select status;
                        foreach (var item in items)
                        {
                            item.favorited = false;
                        }
                    }

                    EventArgs e = new EventArgs();
                    FavoritesDestroySuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    FavoritesDestroyFailed(this, e);
                }
            });
        }

        public void SearchTimeline(string q)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.SEARCH_PUBLIC_TIMELINE,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("q", q);
            restRequest.AddParameter("count", "60");

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ObservableCollection<Items.Status> status = new ObservableCollection<Items.Status>();
                    var ds = new DataContractJsonSerializer(status.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    status = ds.ReadObject(ms) as ObservableCollection<Items.Status>;
                    ms.Close();

                    UserTimelineEventArgs<Items.Status> e = new UserTimelineEventArgs<Items.Status>();
                    e.UserStatus = status;
                    SearchTimelineSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    SearchTimelineFailed(this, e);
                }
            });
        }

        public void TrendsList()
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.TRENDS_LIST,
                Method = Hammock.Web.WebMethod.Get
            };
       
            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Items.TrendsList trends = new Items.TrendsList();
                    var ds = new DataContractJsonSerializer(trends.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    trends = ds.ReadObject(ms) as Items.TrendsList;
                    ms.Close();

                    TrendsListEventArgs e = new TrendsListEventArgs();
                    e.trendsList = trends;
                    TrendsListSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    TrendsListFailed(this, e);
                }
            });
        }

        public void TaggedList(string id )
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.USER_TAG_LIST,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("id",id);

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    List<string> tags = new List<string>();
                    var ds = new DataContractJsonSerializer(tags.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    tags = ds.ReadObject(ms) as List<string>;
                    ms.Close();

                    ListEventArgs<string> e = new ListEventArgs<string>();
                    e.Result = tags;
                    TagListSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    TagListFailed(this, e);
                }
            });
        }

        public void Tagged(string tag)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.USERS_TAGGED,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("tag", tag);

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                   ObservableCollection<Items.User> users = new ObservableCollection<Items.User>();
                    var ds = new DataContractJsonSerializer(users.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    users = ds.ReadObject(ms) as ObservableCollection<Items.User>;
                    ms.Close();

                    var e = new UserTimelineEventArgs<Items.User>();
                    e.UserStatus = users;
                    TaggedSuccess(this, e);
                }
                else
                {
                    FailedEventArgs e = new FailedEventArgs();
                    TaggedFailed(this, e);
                }
            });
        }
        #endregion



    }
}
