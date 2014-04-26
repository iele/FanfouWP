using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP.Utils
{
   public  class UnicodeToUtf8
    {
       public static string converter(string unicodeString)
       {
           Encoding utf8 = Encoding.UTF8;
           Encoding unicode = Encoding.Unicode;
           byte[] unicodeBytes = unicode.GetBytes(unicodeString);
           byte[] utf8Bytes = Encoding.Convert(unicode, utf8, unicodeBytes);
           char[] utf8Chars = new char[utf8.GetCharCount(utf8Bytes, 0, utf8Bytes.Length)];
           utf8.GetChars(utf8Bytes, 0, utf8Bytes.Length, utf8Chars, 0);
           return new string(utf8Chars);
       }
    }
}
