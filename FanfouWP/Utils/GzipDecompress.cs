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
                GZipStream decompress = new GZipStream(stream, CompressionMode.Decompress);
                decompress.CopyTo(tempMs);
                decompress.Close();
                return tempMs.ToArray();               
            }
        }

    }
}
