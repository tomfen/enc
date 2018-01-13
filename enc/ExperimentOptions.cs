using System.Collections.Generic;
using System.Linq;

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
        
        public static double getParameterDouble(Dictionary<string, string> options, string parameter, double def)
        {
            try
            {
                return double.Parse(options[parameter]);
            }
            catch
            {
                return def;
            }
        }

        public static int getParameterInt(Dictionary<string, string> options, string parameter, int def)
        {
            try
            {
                return int.Parse(options[parameter]);
            }
            catch
            {
                return def;
            }
        }
        
        public static int[] getParameterIntArray(Dictionary<string, string> options, string parameter, int[] def)
        {
            try
            {
                return options[parameter].Trim().Split().Select(x => int.Parse(x))
                    .ToArray();
            }
            catch
            {
                return def;
            }
        }

        public static double[] getParameterDoubleArray(Dictionary<string, string> options, string parameter, double[] def)
        {
            try
            {
                return options[parameter].Split().Select(x => double.Parse(x)).ToArray();
            }
            catch
            {
                return def;
            }
        }
    }
}
