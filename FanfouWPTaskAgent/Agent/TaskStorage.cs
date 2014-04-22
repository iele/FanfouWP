using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWPTaskAgent.Agent
{
    public class TaskStorage
    {
        public static void WriteAgentParameter(int freq, int mention, int direct, int request)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists("agent-storage.txt"))
                {
                    isf.DeleteFile("agent-storage.txt");

                }
                using (IsolatedStorageFileStream writeStream = new IsolatedStorageFileStream("agent-storage.txt", System.IO.FileMode.CreateNew, FileAccess.Write, isf))
                {
                    {
                        var s = freq + "\n" + mention + "\n" + direct + "\n" + request;
                        StreamWriter sw = new StreamWriter(writeStream, Encoding.Unicode);
                        sw.Write(s);
                        sw.Flush();
                    }
                }
            }
        }
        public static string[] ReadAgentParameter()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {

                using (IsolatedStorageFileStream readStream = new IsolatedStorageFileStream("agent-storage.txt", System.IO.FileMode.Open, FileAccess.Read, isf))
                {
                    {
                        using (StreamReader sr = new StreamReader(readStream, Encoding.Unicode))
                        {
                            string[] ss = new string[4];
                            ss[0] = sr.ReadLine() ;
                            ss[1] = sr.ReadLine();
                            ss[2] = sr.ReadLine();
                            ss[3] = sr.ReadLine();
                            return ss;
                        }
                    }
                }
            }
        }
    }
}
