using Encog.Engine.Network.Activation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace enc
{
    class ActivationBenchmark : IExperiment
    {
        public string Command => "a";

        public string Description => "Testuje czas obliczania funckji aktywacji";

        public string Options => "";

        public string Name => "Test funkcji aktywacji";

        static private double[] RandomArray(int size, double min, double max)
        {
            var ret = new double[size];
            var random = new Random();

            for (int i = 0; i < size; i++)
            {
                ret[i] = random.NextDouble() * (max - min) + min;
            }

            return ret;
        }

        public void Run(Dictionary<string, string> options)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(IActivationFunction));

            var funs = from t in assembly.GetTypes()
                            where t.GetInterfaces().Contains(typeof(IActivationFunction))
                                     && t.GetConstructor(Type.EmptyTypes) != null
                            select Activator.CreateInstance(t) as IActivationFunction;

            double[] arr = RandomArray(20000000, -10.0, 10.0);

            Console.WriteLine("Pochodna:");
            foreach (var fun in funs)
            {
                if (!fun.HasDerivative)
                    continue;

                double[] res = (double[])arr.Clone();

                var stopwatch = new Stopwatch();

                stopwatch.Start();
                
                for (int j = 0; j < res.Length; j++)
                    fun.DerivativeFunction(res[j], 0);

                stopwatch.Stop();
                
                Console.WriteLine(fun.ToString().PadRight(60) + "time: " + stopwatch.Elapsed.ToString(@"ss\.ffff"));
            }


            Console.WriteLine("\nWartość funkcji:");
            foreach (var fun in funs)
            {
                double[] res = (double[])arr.Clone();

                var stopwatch = new Stopwatch();

                stopwatch.Start();
                
                fun.ActivationFunction(res, 0, res.Length);

                stopwatch.Stop();

                Console.WriteLine(fun.ToString().PadRight(60) + "time: " + stopwatch.Elapsed.ToString(@"ss\.ffff"));
            }
        }
    }
}
