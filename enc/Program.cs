using enc.lander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace enc
{
    public static class Program
    {
        private static IEnumerable<IExperiment> examples;

        static public void WriteMenu()
        {
            foreach (IExperiment e in examples)
            {
                Console.WriteLine(e.Command.PadRight(5) + " | " + e.Name);
            }
        }

        static void Main(string[] args)
        {
            examples = from t in Assembly.GetExecutingAssembly().GetTypes()
                       where t.GetInterfaces().Contains(typeof(IExperiment))
                       select Activator.CreateInstance(t) as IExperiment;

            WriteMenu();

            string com;
            do
            {
                com = Console.ReadLine();
                var options = ExperimentOptions.ParseOptions(com);

                foreach(IExperiment e in examples)
                {
                        if (com.Split('-')[0].Trim() == e.Command)
                            e.Run(options);
                }
            } while (com != "e");
        }
    }
}
