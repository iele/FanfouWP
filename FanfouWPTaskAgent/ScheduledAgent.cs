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

namespace FanfouWPTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
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

        protected override void OnInvoke(ScheduledTask task)
        {
            var s = AgentReader.ReadAgentParameter();

            if (s.Length != 4)
            {
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


            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = FanfouConsts.ACCOUNT_NOTIFICATION,
                Method = Hammock.Web.WebMethod.Get
            };

            client.BeginRequest(restRequest, (request, response, userstate) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Item i = new Item();
                    var ds = new DataContractJsonSerializer(i.GetType());
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    i = ds.ReadObject(ms) as Item;
                    ms.Close();

                    string notifocation = "";
                    if (i.mentions != 0)
                    {
                        notifocation += i.mentions.ToString() + "个提及 ";
                    }
                    if (i.direct_messages != 0)
                    {
                        notifocation += i.direct_messages.ToString() + "个未读私信 ";
                    }
                    if (notifocation == "")
                    {
                        NotifyComplete();
                        return;
                    }

                    ShellToast st = new ShellToast();
                    st.Title = "饭窗";
                    st.Content = "您有" + notifocation;
                    st.Show();

                    foreach (var item in ShellTile.ActiveTiles)
                    {
                        var data = new IconicTileData();
                        data.Title = "饭窗";
                        data.WideContent1 = "饭窗通知";
                        data.WideContent2 = "您有" + notifocation;
                        data.Count = i.mentions + i.direct_messages > 99 ? 99 : i.mentions + i.direct_messages;
                        item.Update(data);
                    }
                }
                else
                {
                    ShellToast st = new ShellToast();
                    st.Title = "饭窗";
                    st.Content = "获取后台通知失败";
                    st.Show();
                }
                NotifyComplete();
            });

        }
    }
}