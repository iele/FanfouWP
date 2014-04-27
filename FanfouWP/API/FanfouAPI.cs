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
using System.Windows.Media.Imaging;
using FanfouWP.API.Items;
using System.Windows;

namespace FanfouWP.API
{
    public class FanfouAPI
    {
        public string oauthToken;
        public string oauthSecret;

        public string username;
        public string password;

        public enum RefreshMode { New, Behind, Back };

        private TimelineStorage<Items.Status> storage = new TimelineStorage<Items.Status>();

        public Items.User CurrentUser { get; set; }

        public bool HomeTimeLineEnded = false;
        public bool MentionTimeLineEnded = false;

        public int HomeTimeLineStatusCount = 0;
        public int MentionTimeLineStatusCount = 0;

        public ObservableCollection<Items.Status> HomeTimeLineStatus = new ObservableCollection<Items.Status>();
        public ObservableCollection<Items.Status> PublicTimeLineStatus = new ObservableCollection<Items.Status>();
        public ObservableCollection<Items.Status> MentionTimeLineStatus = new ObservableCollection<Items.Status>();

        public void ResetManager()
        {
            oauthToken = null;
            oauthSecret = null;
            username = null;
            password = null;
            CurrentUser = null;
            FanfouWP.API.FanfouAPI.Instance.HomeTimeLineStatus = new ObservableCollection<Items.Status>();
            FanfouWP.API.FanfouAPI.Instance.PublicTimeLineStatus = new ObservableCollection<Items.Status>();
            FanfouWP.API.FanfouAPI.Instance.MentionTimeLineStatus = new ObservableCollection<Items.Status>();
        }

        public string firstHomeTimeLineStatusId
        {
            get
            {
                try
                {
                    return this.HomeTimeLineStatus.First(s => (s.user.id != this.CurrentUser.id)).id;
                }
                catch (Exception)
                {
                    return "";
                }
                ;
            }
            private set { }
        }
        public string lastHomeTimeLineStatusId
        {
            get
            {
                try
                {
                    return HomeTimeLineStatus.Last(s => (s.user.id != this.CurrentUser.id)).id;
                }
                catch (Exception)
                {
                    return "";
                }
            }
            private set { }
        }
        public string firstPublicTimeLineStatusId
        {
            get
            {
                try
                {
                    return PublicTimeLineStatus.First(s => (s.user.id != this.CurrentUser.id)).id;
                }
                catch (Exception)
                {
                    return "";
                }
            }
            private set { }
        }
        public string lastPublicTimeLineStatussId
        {
            get
            {
                try
                {
                    return PublicTimeLineStatus.Last(s => (s.user.id != this.CurrentUser.id)).id;
                }
                catch (Exception)
                {
                    return "";
                }
                ;
            }
            private set { }
        }
        public string firstMentionTimeLineStatusId
        {
            get
            {
                try
                {
                    return MentionTimeLineStatus.First(s => (s.user.id != this.CurrentUser.id)).id;
                }
                catch (Exception)
                {
                    return "";
                }
                ;
            }
            private set { }
        }
        public string lastMentionTimeLineStatusId
        {
            get
            {
                try
                {
                    return MentionTimeLineStatus.Last(s => (s.user.id != this.CurrentUser.id)).id;
                }
                catch (Exception)
                {
                    return "";
                }
                ;
            }
            private set { }
        }

        public ObservableCollection<Items.DirectMessageItem> DirectMessageConversations { get; set; }

        public delegate void RestoreDataSuccessHandler(object sender, EventArgs e);
        public delegate void RestoreDataFailedHandler(object sender, FailedEventArgs e);
        public event RestoreDataSuccessHandler RestoreDataSuccess;
        public event RestoreDataFailedHandler RestoreDataFailed;

        public delegate void RestoreDataCompletedHandler(object sender, EventArgs e);
        public event RestoreDataCompletedHandler RestoreDataCompleted;


        public delegate void LoginSuccessHandler(object sender, EventArgs e);
        public delegate void LoginFailedHandler(object sender, FailedEventArgs e);
        public delegate void VerifyCredentialsSuccessHandler(object sender, EventArgs e);
        public delegate void VerifyCredentialsFailedHandler(object sender, FailedEventArgs e);
        public delegate void AccountNotificationSuccessHandler(object sender, EventArgs e);
        public delegate void AccountNotificationFailedHandler(object sender, FailedEventArgs e);

        public event LoginSuccessHandler LoginSuccess;
        public event LoginFailedHandler LoginFailed;
        public event VerifyCredentialsSuccessHandler VerifyCredentialsSuccess;
        public event VerifyCredentialsFailedHandler VerifyCredentialsFailed;
        public event AccountNotificationSuccessHandler AccountNotificationSuccess;
        public event AccountNotificationFailedHandler AccountNotificationFailed;

        public delegate void StatusUpdateSuccessHandler(object sender, EventArgs e);
        public delegate void StatusUpdateFailedHandler(object sender, FailedEventArgs e);
        public delegate void StatusDestroySuccessHandler(object sender, EventArgs e);
        public delegate void StatusDestroyFailedHandler(object sender, FailedEventArgs e);

        public event StatusUpdateSuccessHandler StatusUpdateSuccess;
        public event StatusUpdateFailedHandler StatusUpdateFailed;
        public event StatusDestroySuccessHandler StatusDestroySuccess;
        public event StatusDestroyFailedHandler StatusDestroyFailed;

        public delegate void HomeTimelineSuccessHandler(object sender, ModeEventArgs e);
        public delegate void HomeTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void PublicTimelineSuccessHandler(object sender, ModeEventArgs e);
        public delegate void PublicTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void MentionTimelineSuccessHandler(object sender, ModeEventArgs e);
        public delegate void MentionTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void UserTimelineSuccessHandler(object sender, UserTimelineEventArgs<Items.Status> e);
        public delegate void UserTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void ContextTimelineSuccessHandler(object sender, UserTimelineEventArgs<Items.Status> e);
        public delegate void ContextTimelineFailedHandler(object sender, FailedEventArgs e);

        public event HomeTimelineSuccessHandler HomeTimelineSuccess;
        public event HomeTimelineFailedHandler HomeTimelineFailed;
        public event PublicTimelineSuccessHandler PublicTimelineSuccess;
        public event PublicTimelineFailedHandler PublicTimelineFailed;
        public event MentionTimelineSuccessHandler MentionTimelineSuccess;
        public event MentionTimelineFailedHandler MentionTimelineFailed;
        public event UserTimelineSuccessHandler UserTimelineSuccess;
        public event UserTimelineFailedHandler UserTimelineFailed;
        public event ContextTimelineSuccessHandler ContextTimelineSuccess;
        public event ContextTimelineFailedHandler ContextTimelineFailed;

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
        public delegate void SearchUserSuccessHandler(object sender, UserTimelineEventArgs<Items.User> e);
        public delegate void SearchUserFailedHandler(object sender, FailedEventArgs e);

        public event SearchTimelineSuccessHandler SearchTimelineSuccess;
        public event SearchTimelineFailedHandler SearchTimelineFailed;
        public event SearchUserSuccessHandler SearchUserSuccess;
        public event SearchUserFailedHandler SearchUserFailed;

        public delegate void TrendsListSuccessHandler(object sender, TrendsListEventArgs e);
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

        public delegate void UsersShowSuccessHandler(object sender, EventArgs e);
        public delegate void UsersShowFailedHandler(object sender, FailedEventArgs e);
        public delegate void UsersFollowersSuccessHandler(object sender, UserTimelineEventArgs<Items.User> e);
        public delegate void UsersFollowersFailedHandler(object sender, FailedEventArgs e);
        public delegate void UsersFriendsSuccessHandler(object sender, UserTimelineEventArgs<Items.User> e);
        public delegate void UsersFriendsFailedHandler(object sender, FailedEventArgs e);

        public event UsersShowSuccessHandler UsersShowSuccess;
        public event UsersShowFailedHandler UsersShowFailed;
        public event UsersFollowersSuccessHandler UsersFollowersSuccess;
        public event UsersFollowersFailedHandler UsersFollowersFailed;
        public event UsersFriendsSuccessHandler UsersFriendsSuccess;
        public event UsersFriendsFailedHandler UsersFriendsFailed;

        public delegate void PhotosUploadSuccessHandler(object sender, EventArgs e);
        public delegate void PhotosUploadFailedHandler(object sender, FailedEventArgs e);
        public delegate void PhotosUserTimelineSuccessHandler(object sender, UserTimelineEventArgs<Items.Status> e);
        public delegate void PhotosUserTimelineFailedHandler(object sender, FailedEventArgs e);

        public event PhotosUploadSuccessHandler PhotosUploadSuccess;
        public event PhotosUploadFailedHandler PhotosUploadFailed;
        public event PhotosUserTimelineSuccessHandler PhotosUserTimelineSuccess;
        public event PhotosUserTimelineFailedHandler PhotosUserTimelineFailed;

        public delegate void FriendshipsCreateSuccessHandler(object sender, EventArgs e);
        public delegate void FriendshipsCreateFailedHandler(object sender, FailedEventArgs e);
        public delegate void FriendshipsDestroySuccessHandler(object sender, EventArgs e);
        public delegate void FriendshipsDestroyFailedHandler(object sender, FailedEventArgs e);
        public delegate void FriendshipsRequestsSuccessHandler(object sender, UserTimelineEventArgs<Items.User> e);
        public delegate void FriendshipsRequestsFailedHandler(object sender, FailedEventArgs e);
        public delegate void FriendshipsAcceptSuccessHandler(object sender, EventArgs e);
        public delegate void FriendshipsAcceptFailedHandler(object sender, FailedEventArgs e);
        public delegate void FriendshipsDenySuccessHandler(object sender, EventArgs e);
        public delegate void FriendshipsDenyFailedHandler(object sender, FailedEventArgs e);

        public event FriendshipsCreateSuccessHandler FriendshipsCreateSuccess;
        public event FriendshipsCreateFailedHandler FriendshipsCreateFailed;
        public event FriendshipsDestroySuccessHandler FriendshipsDestroySuccess;
        public event FriendshipsDestroyFailedHandler FriendshipsDestroyFailed;
        public event FriendshipsRequestsSuccessHandler FriendshipsRequestsSuccess;
        public event FriendshipsRequestsFailedHandler FriendshipsRequestsFailed;
        public event FriendshipsAcceptSuccessHandler FriendshipsAcceptSuccess;
        public event FriendshipsAcceptFailedHandler FriendshipsAcceptFailed;
        public event FriendshipsDenySuccessHandler FriendshipsDenySuccess;
        public event FriendshipsDenyFailedHandler FriendshipsDenyFailed;

        public delegate void DirectMessageConversationListSuccessHandler(object sender, UserTimelineEventArgs<Items.DirectMessageItem> e);
        public delegate void DirectMessageConversationListFailedHandler(object sender, FailedEventArgs e);
        public delegate void DirectMessageConversationSuccessHandler(object sender, UserTimelineEventArgs<Items.DirectMessage> e);
        public delegate void DirectMessageConversationFailedHandler(object sender, FailedEventArgs e);
        public delegate void DirectMessageNewSuccessHandler(object sender, EventArgs e);
        public delegate void DirectMessageNewFailedHandler(object sender, FailedEventArgs e);

        public event DirectMessageConversationListSuccessHandler DirectMessageConversationListSuccess;
        public event DirectMessageConversationListFailedHandler DirectMessageConversationListFailed;
        public event DirectMessageConversationSuccessHandler DirectMessageConversationSuccess;
        public event DirectMessageConversationFailedHandler DirectMessageConversationFailed;
        public event DirectMessageNewSuccessHandler DirectMessageNewSuccess;
        public event DirectMessageNewFailedHandler DirectMessageNewFailed;


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
            set
            {
                instance = value;
            }
        }
        public FanfouAPI()
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
            switch (sender as string)
            {
                case FanfouConsts.STATUS_HOME_TIMELINE:
                    this.HomeTimeLineStatus = e.UserStatus;
                    this.HomeTimeLineStatusCount = this.HomeTimeLineStatus.Count;
                    break;
                case FanfouConsts.STATUS_PUBLIC_TIMELINE:
                    this.PublicTimeLineStatus = e.UserStatus;
                    break;
                case FanfouConsts.STATUS_MENTION_TIMELINE:
                    this.MentionTimeLineStatus = e.UserStatus;
                    this.MentionTimeLineStatusCount = this.MentionTimeLineStatus.Count;
                    break;
            }
            if (RestoreDataCompleted != null)
                RestoreDataCompleted(sender, e);
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

        private int[] CountIndex = { 100, 300, 500, 1000 };
        private void HomeTimeLineStatusChanged()
        {
            if (this.HomeTimeLineStatus.Count > CountIndex[settings.cacheSize])
            {
                var l = new ObservableCollection<Items.Status>();
                for (var i = 0; i < CountIndex[settings.cacheSize]; i++)
                    l.Add(this.HomeTimeLineStatus[i]);
                storage.SaveDataToIsolatedStorage(FanfouConsts.STATUS_HOME_TIMELINE, this.CurrentUser.id, l);
            }
            else
                storage.SaveDataToIsolatedStorage(FanfouConsts.STATUS_HOME_TIMELINE, this.CurrentUser.id, this.HomeTimeLineStatus);
        }
        private void PublicTimeLineStatusChanged()
        {
            if (this.PublicTimeLineStatus.Count > CountIndex[settings.cacheSize])
            {
                var l = new ObservableCollection<Items.Status>();
                for (var i = 0; i < CountIndex[settings.cacheSize]; i++)
                    l.Add(this.PublicTimeLineStatus[i]);
                storage.SaveDataToIsolatedStorage(FanfouConsts.STATUS_PUBLIC_TIMELINE, this.CurrentUser.id, l);
            }
            else
                storage.SaveDataToIsolatedStorage(FanfouConsts.STATUS_PUBLIC_TIMELINE, this.CurrentUser.id, this.PublicTimeLineStatus);
        }

        private void MentionTimeLineStatusChanged()
        {
            if (this.MentionTimeLineStatus.Count > CountIndex[settings.cacheSize])
            {
                var l = new ObservableCollection<Items.Status>();
                for (var i = 0; i < CountIndex[settings.cacheSize]; i++)
                    l.Add(this.MentionTimeLineStatus[i]);
                storage.SaveDataToIsolatedStorage(FanfouConsts.STATUS_MENTION_TIMELINE, this.CurrentUser.id, l);
            }
            else

                storage.SaveDataToIsolatedStorage(FanfouConsts.STATUS_MENTION_TIMELINE, this.CurrentUser.id, this.MentionTimeLineStatus);
        }
        public async Task TryRestoreData()
        {
            settings.RestoreSettings();
            if (settings.username != null && settings.password != null && settings.currentUser != null)
            {
                this.username = settings.username;
                this.password = settings.password;
                this.oauthToken = settings.oauthToken;
                this.oauthSecret = settings.oauthSecret;

                this.CurrentUser = settings.currentUser;

                this.LoginSuccess += (o, e) => { };
                this.LoginFailed += (o, e) => { };
                this.Login(username, password);

                await storage.ReadDataFromIsolatedStorage(FanfouConsts.STATUS_HOME_TIMELINE, this.CurrentUser.id);
                await storage.ReadDataFromIsolatedStorage(FanfouConsts.STATUS_MENTION_TIMELINE, this.CurrentUser.id);

                RestoreDataSuccess(this, new EventArgs());
            }
            else
            {
                RestoreDataFailed(this, new FailedEventArgs());
            }
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
                        ClientPassword = password,
                    }
                };
                return client;
            }
            else
            {
                return null;
            }
        }

        #region account
        public void AccountNotification()
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
                {
                    Path = FanfouConsts.ACCOUNT_NOTIFICATION,
                    Method = Hammock.Web.WebMethod.Get
                };

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Items.Notifications i = new Items.Notifications();
                        var ds = new DataContractJsonSerializer(i.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        i = ds.ReadObject(ms) as Items.Notifications;
                        ms.Close();
                        AccountNotificationSuccess(i, new EventArgs());
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        AccountNotificationFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    AccountNotificationFailed(this, e);
                }
            });
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
                },
                Encoding = UnicodeEncoding.Unicode
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
                    settings.currentUser = user;
                    settings.SaveSettings();
                    this.CurrentUser = user;

                    EventArgs e = new EventArgs();
                    VerifyCredentialsSuccess(this, e);
                }
                else
                {
                    Items.Error er = new Items.Error();
                    var ds = new DataContractJsonSerializer(er.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    er = ds.ReadObject(ms) as Items.Error;
                    ms.Close();
                    FailedEventArgs e = new FailedEventArgs(er);
                    VerifyCredentialsFailed(this, e);
                }
            });
        }
        #endregion
        #region status
        public void StatusUpdate(string status, string in_reply_to_status_id = "", string in_reply_to_user_id = "", string repost_status_id = "", string location = "")
        {
            Utils.StatusUploader.StatusUploadSuccess += StatusUploader_StatusUploadSuccess;
            Utils.StatusUploader.StatusUploadFailed += StatusUploader_StatusUploadFailed;
            Utils.StatusUploader.updateStatus(status, in_reply_to_status_id, in_reply_to_user_id, repost_status_id, location);
        }

        void StatusUploader_StatusUploadFailed(object sender, FailedEventArgs e)
        {
            StatusUpdateFailed(this, new FailedEventArgs(new Error()));
        }

        void StatusUploader_StatusUploadSuccess(object sender, EventArgs e)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.HomeTimeLineStatus.Insert(0, sender as Status);
                    HomeTimeLineStatusChanged();
                    StatusUpdateSuccess(this, e);
                });
            }
            catch (Exception)
            {
                FailedEventArgs ex = new FailedEventArgs();
                StatusUpdateFailed(this, ex);
            }
        }

        public void StatusDestroy(string id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.STATUS_DESTROY,
                Method = Hammock.Web.WebMethod.Post
            };

            var client = GetClient();
            client.AddHeader("content-type", "application/x-www-form-urlencoded");
            restRequest.AddParameter("id", id);

            client.BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            var a = from s in this.HomeTimeLineStatus where s.id == id select s;
                            if (a.Count() != 0)
                                foreach (var item in a.ToList())
                                    this.HomeTimeLineStatus.Remove(item);
                            var b = from s in this.MentionTimeLineStatus where s.id == id select s;
                            if (b.Count() != 0)
                                foreach (var item in b.ToList())
                                    this.MentionTimeLineStatus.Remove(item);
                        });
                        EventArgs e = new EventArgs();
                        StatusDestroySuccess(this, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error; ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        StatusDestroyFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    StatusDestroyFailed(this, e);
                }
            });
        }

        public void StatusContextTimeline(string id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.STATUSES_CONTEXT_TIMELINE,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("id", id);

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
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
                        ContextTimelineSuccess(this, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        ContextTimelineFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    ContextTimelineFailed(this, e);
                }
            });
        }

        public void StatusUserTimeline(int count, string user_id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.STATUS_USER_TIMELINE,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("user_id", user_id);
            restRequest.AddParameter("count", count.ToString());

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
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
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        UserTimelineFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    UserTimelineFailed(this, e);
                }
            });
        }
        public void StatusHomeTimeline(int count = 20, RefreshMode mode = RefreshMode.New)
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
            restRequest.AddParameter("count", count.ToString());

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ObservableCollection<Items.Status> status = new ObservableCollection<Items.Status>();
                        var ds = new DataContractJsonSerializer(status.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        status = ds.ReadObject(ms) as ObservableCollection<Items.Status>;
                        ms.Close();

                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            int c = 0;
                            if (status != null)
                            {
                                switch (mode)
                                {
                                    case RefreshMode.New:
                                    case RefreshMode.Behind:
                                        foreach (var i in status.Reverse())
                                        {
                                            var ss = from h in this.HomeTimeLineStatus where h.id == i.id select h;
                                            if (ss.Count() == 0)
                                                HomeTimeLineStatus.Insert(0, i);
                                            else
                                            {
                                                c++;
                                            }
                                        }

                                        break;
                                    case RefreshMode.Back:
                                        foreach (var i in status)
                                        {
                                            var ss = from h in this.HomeTimeLineStatus where h.id == i.id select h;
                                            if (ss.Count() == 0)
                                                HomeTimeLineStatus.Add(i);
                                            else
                                            {
                                                c++;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            HomeTimeLineStatusCount = status.Count - c;
                            HomeTimeLineStatusChanged();
                            ModeEventArgs e = new ModeEventArgs(mode);
                            HomeTimelineSuccess(this, e);
                        });
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        HomeTimelineFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    HomeTimelineFailed(this, e);
                }
            });
        }

        public void StatusPublicTimeline(int count, RefreshMode mode = RefreshMode.New)
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
                default:
                    break;
            }
            restRequest.AddParameter("count", count.ToString());
            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ObservableCollection<Items.Status> status = new ObservableCollection<Items.Status>();

                        var ds = new DataContractJsonSerializer(status.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        status = ds.ReadObject(ms) as ObservableCollection<Items.Status>;
                        ms.Close();
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            var l = new ObservableCollection<Items.Status>();
                            if (status != null)
                            {
                                switch (mode)
                                {
                                    case RefreshMode.New:
                                    case RefreshMode.Behind:
                                        foreach (var i in status != null ? status.Reverse() : status)
                                        {
                                            var ss = from h in this.PublicTimeLineStatus where h.id == i.id select h;
                                            if (ss.Count() == 0)
                                                PublicTimeLineStatus.Insert(0, i);
                                        }

                                        break;
                                    case RefreshMode.Back:
                                        foreach (var i in status)
                                        {
                                            var ss = from h in this.PublicTimeLineStatus where h.id == i.id select h;
                                            if (ss.Count() == 0)
                                                PublicTimeLineStatus.Add(i);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            PublicTimeLineStatusChanged();
                            ModeEventArgs e = new ModeEventArgs(mode);
                            PublicTimelineSuccess(this, e);
                        });
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        PublicTimelineFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    PublicTimelineFailed(this, e);
                }
            });
        }
        public void StatusMentionTimeline(int count, RefreshMode mode = RefreshMode.New)
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
                default:
                    break;
            }
            restRequest.AddParameter("count", count.ToString());
            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ObservableCollection<Items.Status> status = new ObservableCollection<Items.Status>();

                        var ds = new DataContractJsonSerializer(status.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        status = ds.ReadObject(ms) as ObservableCollection<Items.Status>;
                        ms.Close();
                        var l = new ObservableCollection<Items.Status>();
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                       {
                           int c = 0;
                           if (status != null)
                           {
                               switch (mode)
                               {
                                   case RefreshMode.New:
                                   case RefreshMode.Behind:
                                       foreach (var i in status != null ? status.Reverse() : status)
                                       {
                                           var ss = from h in this.MentionTimeLineStatus where h.id == i.id select h;
                                           if (ss.Count() == 0)
                                               MentionTimeLineStatus.Insert(0, i);
                                           else
                                           {
                                               c++;
                                           }
                                       }

                                       break;
                                   case RefreshMode.Back:
                                       foreach (var i in status)
                                       {
                                           var ss = from h in this.MentionTimeLineStatus where h.id == i.id select h;
                                           if (ss.Count() == 0)
                                               MentionTimeLineStatus.Add(i);
                                           else
                                           {
                                               c++;
                                           }
                                       }
                                       break;
                                   default:
                                       break;
                               }
                           }
                           MentionTimeLineStatusCount = status.Count - c;
                           MentionTimeLineStatusChanged();
                           ModeEventArgs e = new ModeEventArgs(mode);
                           MentionTimelineSuccess(this, e);
                       });
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        MentionTimelineFailed(this, e);
                    }
                }
                catch (Exception)
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
                Path = FanfouConsts.FAVORITES_ID,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("id", id);
            restRequest.AddParameter("page", page.ToString());

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
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
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        FavoritesFailed(this, e);
                    }
                }
                catch (Exception)
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
                try
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
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        FavoritesCreateFailed(this, e);
                    }
                }
                catch (Exception)
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
                try
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
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        FavoritesDestroyFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    FavoritesDestroyFailed(this, e);
                }
            });
        }

        #endregion
        #region search

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
                try
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
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        SearchTimelineFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    SearchTimelineFailed(this, e);
                }
            });
        }

        public void SearchUser(string q)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.SEARCH_USER,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("q", q);
            restRequest.AddParameter("count", "60");

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        UserList user = new UserList();
                        var ds = new DataContractJsonSerializer(user.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        user = ds.ReadObject(ms) as UserList;
                        ms.Close();

                        UserTimelineEventArgs<Items.User> e = new UserTimelineEventArgs<Items.User>();
                        e.UserStatus = user.users;
                        SearchUserSuccess(this, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        SearchUserFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    SearchUserFailed(this, e);
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
                try
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
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        TrendsListFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    TrendsListFailed(this, e);
                }
            });
        }

        public void TaggedList(string id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.USERS_TAG_LIST,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("id", id);

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
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
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        TagListFailed(this, e);
                    }
                }
                catch (Exception)
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
                try
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
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        TaggedFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    TaggedFailed(this, e);
                }
            });
        }
        #endregion
        #region user
        public void UsersShow(string id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.USERS_SHOW,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("id", id);

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Items.User user = new Items.User();
                        var ds = new DataContractJsonSerializer(user.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        user = ds.ReadObject(ms) as Items.User;
                        ms.Close();

                        EventArgs e = new EventArgs();
                        UsersShowSuccess(user, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        UsersShowFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    UsersShowFailed(this, e);
                }
            });
        }
        public void UsersFollowers(string id, int count = 60, int page = 1)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.USERS_FOLLOWERS,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("id", id);
            restRequest.AddParameter("count", count.ToString());
            restRequest.AddParameter("page", page.ToString());

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ObservableCollection<Items.User> user = new ObservableCollection<Items.User>();
                        var ds = new DataContractJsonSerializer(user.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        user = ds.ReadObject(ms) as ObservableCollection<Items.User>;
                        ms.Close();

                        UserTimelineEventArgs<Items.User> e = new UserTimelineEventArgs<Items.User>();
                        e.UserStatus = user;
                        UsersFollowersSuccess(this, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        UsersFollowersFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    UsersFollowersFailed(this, e);
                }
            });


        }

        public void UsersFriends(string id, int count = 60, int page = 1)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.USERS_FRIENDS,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("id", id);
            restRequest.AddParameter("count", count.ToString());
            restRequest.AddParameter("page", page.ToString());

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ObservableCollection<Items.User> user = new ObservableCollection<Items.User>();
                        var ds = new DataContractJsonSerializer(user.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        user = ds.ReadObject(ms) as ObservableCollection<Items.User>;
                        ms.Close();

                        UserTimelineEventArgs<Items.User> e = new UserTimelineEventArgs<Items.User>();
                        e.UserStatus = user;
                        UsersFriendsSuccess(this, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        UsersFriendsFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    UsersFriendsFailed(this, e);
                }
            });
        }
        #endregion
        #region friendship
        public void FriendshipCreate(string id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.FRIENDSHIPS_CREATE,
                Method = Hammock.Web.WebMethod.Post
            };

            var client = GetClient();
            client.AddHeader("content-type", "application/x-www-form-urlencoded");
            restRequest.AddParameter("id", id);

            client.BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Items.User s = new Items.User();
                        var ds = new DataContractJsonSerializer(s.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        s = ds.ReadObject(ms) as Items.User;
                        ms.Close();

                        EventArgs e = new EventArgs();
                        FriendshipsCreateSuccess(s, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        FriendshipsCreateFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    FriendshipsCreateFailed(this, e);
                }
            });
        }
        public void FriendshipDestroy(string id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.FRIENDSHIPS_DESTROY,
                Method = Hammock.Web.WebMethod.Post
            };

            var client = GetClient();
            client.AddHeader("content-type", "application/x-www-form-urlencoded");
            restRequest.AddParameter("id", id);

            client.BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Items.User s = new Items.User();
                        var ds = new DataContractJsonSerializer(s.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        s = ds.ReadObject(ms) as Items.User;
                        ms.Close();

                        EventArgs e = new EventArgs();
                        FriendshipsDestroySuccess(s, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        FriendshipsDestroyFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    FriendshipsDestroyFailed(this, e);
                }
            });
        }
        public void FriendshipRequests(int page = 1)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.FRIENDSHIPS_REQUESTS,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("page", page.ToString());

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ObservableCollection<Items.User> user = new ObservableCollection<Items.User>();
                        var ds = new DataContractJsonSerializer(user.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        user = ds.ReadObject(ms) as ObservableCollection<Items.User>;
                        ms.Close();

                        UserTimelineEventArgs<Items.User> e = new UserTimelineEventArgs<Items.User>();
                        e.UserStatus = user;
                        FriendshipsRequestsSuccess(this, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        FriendshipsRequestsFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    FriendshipsRequestsFailed(this, e);
                }
            });
        }

        public void FriendshipAccept(string id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.FRIENDSHIPS_ACCEPT,
                Method = Hammock.Web.WebMethod.Post
            };

            var client = GetClient();
            client.AddHeader("content-type", "application/x-www-form-urlencoded");
            restRequest.AddParameter("id", id);

            client.BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Items.User s = new Items.User();
                        var ds = new DataContractJsonSerializer(s.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        s = ds.ReadObject(ms) as Items.User;
                        ms.Close();

                        EventArgs e = new EventArgs();
                        FriendshipsAcceptSuccess(s, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        FriendshipsAcceptFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    FriendshipsAcceptFailed(this, e);
                }
            });
        }

        public void FriendshipDeny(string id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.FRIENDSHIPS_DENY,
                Method = Hammock.Web.WebMethod.Post
            };

            var client = GetClient();
            client.AddHeader("content-type", "application/x-www-form-urlencoded");
            restRequest.AddParameter("id", id);

            client.BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Items.User s = new Items.User();
                        var ds = new DataContractJsonSerializer(s.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        s = ds.ReadObject(ms) as Items.User;
                        ms.Close();

                        EventArgs e = new EventArgs();
                        FriendshipsDenySuccess(s, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        FriendshipsDenyFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    FriendshipsDenyFailed(this, e);
                }
            });
        }
        #endregion
        #region photo
        public void PhotosUserTimeline(string id, int count = 60)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.PHOTOS_USER_TIMELINE,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("id", id);
            restRequest.AddParameter("count", count.ToString());

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
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
                        PhotosUserTimelineSuccess(this, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        PhotosUserTimelineFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    PhotosUserTimelineFailed(this, e);
                }
            });
        }
        public void PhotoUpload(string status, WriteableBitmap photo, string location = "")
        {
            var stream = new MemoryStream();
            var q = settings.imageQuality;

            var b = photo.PixelWidth > photo.PixelHeight ? true : false;
            var width = 0;
            var height = 0;
            var quality = 0;
            switch (q)
            {
                case 0:
                    width = b ? 600 : (int)(photo.PixelWidth * 600 / photo.PixelHeight);
                    height = b ? (int)(photo.PixelHeight * 600 / photo.PixelWidth) : 600;
                    if (b == true && photo.PixelWidth < 600)
                    {
                        width = photo.PixelWidth;
                        height = photo.PixelHeight;
                    }
                    else if (b == false && photo.PixelHeight < 600)
                    {
                        width = photo.PixelWidth;
                        height = photo.PixelHeight;
                    }

                    quality = 80;
                    break;
                case 1:
                    width = b ? 800 : (int)(photo.PixelWidth * 800 / photo.PixelHeight);
                    height = b ? (int)(photo.PixelHeight * 800 / photo.PixelWidth) : 800;
                    if (b == true && photo.PixelWidth < 800)
                    {
                        width = photo.PixelWidth;
                        height = photo.PixelHeight;
                    }
                    else if (b == false && photo.PixelHeight < 800)
                    {
                        width = photo.PixelWidth;
                        height = photo.PixelHeight;
                    }
                    quality = 90;
                    break;
                case 2:
                    width = b ? 1280 : (int)(photo.PixelWidth * 1280 / photo.PixelHeight);
                    height = b ? (int)(photo.PixelHeight * 1280 / photo.PixelWidth) : 1280;
                    if (b == true && photo.PixelWidth < 1280)
                    {
                        width = photo.PixelWidth;
                        height = photo.PixelHeight;
                    }
                    else if (b == false && photo.PixelHeight < 1280)
                    {
                        width = photo.PixelWidth;
                        height = photo.PixelHeight;
                    }
                    quality = 100;
                    break;
                case 3:
                    width = photo.PixelWidth;
                    height = photo.PixelHeight;
                    quality = 100;
                    break;
                default:
                    width = b ? 600 : (int)(photo.PixelWidth * 600 / photo.PixelHeight);
                    height = b ? (int)(photo.PixelHeight * 600 / photo.PixelWidth) : 600;
                    if (b == true && photo.PixelWidth < 600)
                    {
                        width = photo.PixelWidth;
                        height = photo.PixelHeight;
                    }
                    else if (b == false && photo.PixelHeight < 600)
                    {
                        width = photo.PixelWidth;
                        height = photo.PixelHeight;
                    }
                    quality = 80;
                    break;
            }
            photo.SaveJpeg(stream, width, height, 0, quality);

            var buff = stream.ToArray();
            Utils.PhotoUploader.PhotosUploadSuccess += PhotoUploader_PhotosUploadSuccess;
            Utils.PhotoUploader.PhotosUploadFailed += PhotoUploader_PhotosUploadFailed;
            Utils.PhotoUploader.uploadPhoto(buff, status, location);
        }

        void PhotoUploader_PhotosUploadFailed(object sender, FailedEventArgs e)
        {
            PhotosUploadFailed(this, new FailedEventArgs(new Error()));
        }

        void PhotoUploader_PhotosUploadSuccess(object sender, EventArgs e)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                     this.HomeTimeLineStatus.Insert(0, sender as Status);
                     HomeTimeLineStatusChanged();
                     PhotosUploadSuccess(this, e);
                 });
            }
            catch (Exception)
            {
                FailedEventArgs ex = new FailedEventArgs();
                PhotosUploadFailed(this, ex);
            }
        }
        #endregion
        #region direct
        public void DirectMessagesConversationList(int page = 1, int count = 20)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.DIRECT_MESSAGES_CONVERSATION_LIST,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("count", count.ToString());
            restRequest.AddParameter("page", page.ToString());

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ObservableCollection<Items.DirectMessageItem> list = new ObservableCollection<Items.DirectMessageItem>();
                        var ds = new DataContractJsonSerializer(list.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        list = ds.ReadObject(ms) as ObservableCollection<Items.DirectMessageItem>;
                        ms.Close();

                        UserTimelineEventArgs<Items.DirectMessageItem> e = new UserTimelineEventArgs<Items.DirectMessageItem>();
                        e.UserStatus = list;
                        DirectMessageConversationListSuccess(this, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        DirectMessageConversationListFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    DirectMessageConversationListFailed(this, e);
                }
            });
        }

        public void DirectMessagesConversation(string id, int count = 60, int page = 1)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.DIRECT_MESSAGES_CONVERSATION,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("id", id);
            restRequest.AddParameter("count", count.ToString());
            restRequest.AddParameter("page", page.ToString());

            GetClient().BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ObservableCollection<Items.DirectMessage> list = new ObservableCollection<Items.DirectMessage>();
                        var ds = new DataContractJsonSerializer(list.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        list = ds.ReadObject(ms) as ObservableCollection<Items.DirectMessage>;
                        ms.Close();

                        UserTimelineEventArgs<Items.DirectMessage> e = new UserTimelineEventArgs<Items.DirectMessage>();
                        e.UserStatus = list;
                        DirectMessageConversationSuccess(this, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);
                        DirectMessageConversationFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    DirectMessageConversationFailed(this, e);
                }
            });
        }

        public void DirectMessagesNew(string user, string text, string in_reply_to_id)
        {
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.DIRECT_MESSAGES_NEW,
                Method = Hammock.Web.WebMethod.Post
            };

            var client = GetClient();
            client.AddHeader("content-type", "application/x-www-form-urlencoded");
            restRequest.AddParameter("user", user);
            restRequest.AddParameter("text", text);
            if (in_reply_to_id != null)
                restRequest.AddParameter("in_reply_to_id", in_reply_to_id);

            client.BeginRequest(restRequest, (request, response, userstate) =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Items.DirectMessage d = new Items.DirectMessage();
                        var ds = new DataContractJsonSerializer(d.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        d = ds.ReadObject(ms) as Items.DirectMessage;
                        ms.Close();
                        EventArgs e = new EventArgs();
                        DirectMessageNewSuccess(d, e);
                    }
                    else
                    {
                        Items.Error er = new Items.Error();
                        var ds = new DataContractJsonSerializer(er.GetType());
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                        er = ds.ReadObject(ms) as Items.Error;
                        ms.Close();
                        FailedEventArgs e = new FailedEventArgs(er);

                        DirectMessageNewFailed(this, e);
                    }
                }
                catch (Exception)
                {
                    FailedEventArgs e = new FailedEventArgs();
                    DirectMessageNewFailed(this, e);
                }
            });
        }
        #endregion

    }
}
