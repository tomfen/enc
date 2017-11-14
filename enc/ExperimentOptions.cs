using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enc
{
    class ExperimentOptions
    {
        public static Dictionary<string, string> ParseOptions(string com)
        {
            var parts = com.Split('-');

            var dict = new Dictionary<string, string>();

            for(int i=1; i<parts.Length; i++)
            {
                var p1 = parts[i].Split(new char[] { ' ' }, 2);
                dict.Add(p1[0], p1.Length > 1? p1[1] : null);
            }

            return dict;
        }
    }
}
