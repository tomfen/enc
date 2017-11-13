using enc.lunar;
using System;
using System.Collections.Generic;

namespace enc
{
    public static class Program
    {
        private static List<Type> examples = new List<Type>();

        static void Main(string[] args)
        {
            /*examples = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where((t) => { return t.GetInterfaces().Contains(typeof(IExperiment)); })
                    .ToList();

            foreach (Type t in examples)
                Console.WriteLine(t.);
            Console.ReadKey();*/



            string com;
            do
            {
                com = Console.ReadLine();

                switch (com)
                {
                    case "a":
                        ActivationBenchmark.run();
                        break;

                    case "t":
                        MNISTDemo.run();
                        break;

                    case "r":
                        ReutersDemo.run();
                        break;

                    case "l":
                        LunarLander.run();
                        break;
                }
            } while (com != "e");
        }
    }
}
