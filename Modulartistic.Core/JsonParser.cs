using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    internal static class JsonParser
    {
        public static void ParseGenerationData(ref List<object> data, string filename)
        {
            // read file content
            string jsontext = File.ReadAllText(filename);

            int line = 0;
            int pos = 0;

            int c_bracket_cnt = 0;
            int s_bracket_cnt = 0;
            bool in_string = false;

            for (int i = 0; i < jsontext.Length; i++)
            {
                char c = jsontext[i];
                line++;
                pos++;
            }
        }
    }
}
