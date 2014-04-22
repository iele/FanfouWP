using FanfouWP.API.Event;
using FanfouWP.API.Items;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FanfouWP.Storage
{
    public class AgentWriter
    {
        public static void WriteAgentParameter(string username, string password, string oauthToken, string oauthSecret, int freq)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists("agent.txt"))
                {
                    isf.DeleteFile("agent.txt");

                }
                using (IsolatedStorageFileStream writeStream = new IsolatedStorageFileStream("agent.txt", System.IO.FileMode.CreateNew, FileAccess.Write, isf))
                {
                    {
                        var s = username + "\n" + password + "\n" + oauthToken + "\n" + oauthSecret + "\n" + freq;
                        StreamWriter sw = new StreamWriter(writeStream, Encoding.Unicode);
                        sw.Write(s);
                        sw.Flush();
                    }
                }
            }
        }
    }
}
