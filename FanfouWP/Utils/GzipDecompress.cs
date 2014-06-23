using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP.Utils
{
    public class GzipDecompress
    {
        public static byte[] Decompress(Stream stream)
        {
            using (MemoryStream tempMs = new MemoryStream())
            {
                GZipStream Decompress = new GZipStream(stream, CompressionMode.Decompress);
                Decompress.CopyTo(tempMs);
                Decompress.Close();
                return tempMs.ToArray();               
            }
        }

    }
}
