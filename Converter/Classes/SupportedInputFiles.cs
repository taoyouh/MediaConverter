using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Classes
{
    static class SupportedInputFiles
    {
        public static IEnumerable<string> GetList()
        {
            return new string[]
            {
                ".m4a",
                ".m4v",
                ".mp4",
                ".mp3",
                ".mpg",
                ".mpeg",
                ".flac"
            };
        }
    }
}
