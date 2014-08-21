using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using FanfouWPTaskAgent.Agent;
using System.Net;
using Hammock.Authentication.OAuth;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;

namespace FanfouWPTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private dynamic s;

        private Item notification;

        private int count;
        static ScheduledAgent()
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private void getMetions(Item i)
        {
            if (i.mentions == 0)
            {
                ShellToast st = new ShellToast();
                st.Title = "饭窗";
                st.Content = "你有" + i.direct_messages + "条新私信 " + i.friend_requests + "条好友请求";
                st.Show();

                foreach (var item in ShellTile.ActiveTiles)
                {
                    var data = new IconicTileData();
                    data.Title = "饭窗";
                    data.WideContent1 = "新消息提醒";
                    data.WideContent2 = "您有" + i.mentions + "条提及";
                    data.WideContent3 = i.direct_messages + "条新私信 " + i.friend_requests + "条好友请求";
                    data.Count = i.mentions + i.direct_messages > 99 ? 99 : i.mentions;
                    item.Update(data);
                }
                NotifyComplete();
                return;
            }

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
                    Token = s[2],
                    TokenSecret = s[3],
                    ClientUsername = s[0],
                    ClientPassword = s[1]
                }

            };
            client.AddHeader("Accept-Encoding", "GZip");
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.STATUS_MENTION_TIMELINE,
                Method = Hammock.Web.WebMethod.Get
            };
            restRequest.AddParameter("count", i.mentions.ToString());
            ObservableCollection<Status> status = null;
            try
            {
                client.BeginRequest(restRequest, (request, response, userstate) =>
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        status = new ObservableCollection<Status>();
                        var ds = new DataContractJsonSerializer(status.GetType());
                        var ms = new MemoryStream(Agent.GzipDecompress.Decompress(response.ContentStream));
                        status = ds.ReadObject(ms) as ObservableCollection<Status>;
                        ms.Close();

                        if (status != null)
                        {
                            foreach (var item in status)
                            {
                                ShellToast st = new ShellToast();
                                st.Title = "饭窗";
                                var msg = item.user.screen_name + ": " + item.text;
                                if (msg.Length > 40)
                                    st.Content = msg.Substring(0, 40);
                                else
                                    st.Content = msg;
                                st.Show();
                            }

                            foreach (var item in ShellTile.ActiveTiles)
                            {
                                var data = new IconicTileData();
                                data.Title = "饭窗";
                                data.WideContent1 = "新消息提醒";
                                data.WideContent2 = "您有" + i.mentions + "条提及";
                                data.WideContent3 = i.direct_messages + "条新私信 " + i.friend_requests + "条好友请求";
                                data.Count = i.mentions + i.direct_messages + i.friend_requests > 99 ? 99 : i.mentions + i.direct_messages + i.friend_requests;
                                item.Update(data);
                            }
                        }
                        NotifyComplete();
                    }
                    else
                    {
                        ShellToast st = new ShellToast();
                        st.Title = "饭窗通知";
                        st.Content = "你有" + i.mentions.ToString() + "新提及";
                        st.Show();

                        foreach (var item in ShellTile.ActiveTiles)
                        {
                            var data = new IconicTileData();
                            data.Title = "饭窗";
                            data.WideContent1 = "新消息提醒";
                            data.WideContent2 = "您有" + i.mentions + "条提及";
                            data.WideContent3 = i.direct_messages + "条新私信 " + i.friend_requests + "条好友请求";
                            data.Count = i.mentions + i.direct_messages + i.friend_requests > 99 ? 99 : i.mentions + i.direct_messages + i.friend_requests;
                            item.Update(data);
                        }

                        NotifyComplete();
                    }
                });
            }
            catch (Exception)
            {

                NotifyComplete();
            }
        }


        protected override void OnInvoke(ScheduledTask task)
        {
            //ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));

            s = AgentReader.ReadAgentParameter();
            count = int.Parse(s[4]);

            if (s.Length != 5)
            {
                NotifyComplete();
                return;
            }

            if (count == 1)
            {
                NotifyComplete();
                return;
            }

            try
            {
                var ss = TaskStorage.ReadAgentParameter();
                notification = new Item();
                notification.mentions = int.Parse(ss[1]);
                notification.direct_messages = int.Parse(ss[2]);
                notification.friend_requests = int.Parse(ss[3]);
            }
            catch (Exception)
            {
                notification = new Item();
                notification.mentions = 0;
                notification.friend_requests = 0;
                notification.direct_messages = 0;
            }

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
                    Token = s[2],
                    TokenSecret = s[3],
                    ClientUsername = s[0],
                    ClientPassword = s[1]
                }
            };
            client.AddHeader("Accept-Encoding", "GZip");

            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.ACCOUNT_NOTIFICATION,
                Method = Hammock.Web.WebMethod.Get
            };
            try
            {
                client.BeginRequest(restRequest, (request, response, userstate) =>
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Item i = new Item();
                        var ds = new DataContractJsonSerializer(i.GetType());
                        var ms = new MemoryStream(Agent.GzipDecompress.Decompress(response.ContentStream));
                        i = ds.ReadObject(ms) as Item;
                        ms.Close();

                        TaskStorage.WriteAgentParameter(0, i.mentions, i.direct_messages, i.friend_requests);

                        if (i.mentions == 0 && i.friend_requests == 0 && i.direct_messages == 0)
                        {
                            NotifyComplete();
                            return;
                        }

                        if (notification != null)
                        {
                            if (this.notification.direct_messages == i.direct_messages && this.notification.friend_requests == i.friend_requests && this.notification.mentions == i.mentions)
                            {
                                NotifyComplete();
                                return;
                            }
                        }

                        this.notification = i;
                        getMetions(i);
                    }
                });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
            }
        }
    }
}