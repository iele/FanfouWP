using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace FanfouWPTaskAgent.Agent
{
    public class AgentReader
    {
        public static string[] ReadAgentParameter()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {

                using (IsolatedStorageFileStream readStream = new IsolatedStorageFileStream("agent.txt", System.IO.FileMode.Open, FileAccess.Read, isf))
                {
                    {
                        using (StreamReader sr = new StreamReader(readStream, Encoding.Unicode))
                        {
                            string[] ss = new string[5];
                            ss[0] = sr.ReadLine();
                            ss[1] = sr.ReadLine();
                            ss[2] = sr.ReadLine();
                            ss[3] = sr.ReadLine();
                            ss[5] = sr.ReadLine();
                            return ss;
                        }
                    }
                }
            }
        }
    }
}
