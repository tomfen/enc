using Encog.Engine.Network.Activation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        static private double[] RandomArray(int size, double min, double max, int seed)
        {
            var ret = new double[size];
            var random = new Random(seed);

            for (int i = 0; i < size; i++)
            {
                ret[i] = random.NextDouble() * (max - min) + min;
            }

            return ret;
        }

        public void Run(Dictionary<string, string> options)
        {
            int size = 20000000;
            double maxabs = -10.0;

            if (options.ContainsKey("a"))
                size = int.Parse(options["a"]);

            if (options.ContainsKey("v"))
                maxabs = double.Parse(options["v"]);

            var functions = getAllActivationFuns();

            List<String> results = new List<string>();
            results.Add("Funkcja;Wartość;Pochodna");

            // Test
            Console.WriteLine("Funkcja                            Wartość   Pochodna");
            foreach (var fun in functions)
            {
                double[] arr = RandomArray(size, -maxabs, maxabs, 1);
                var stopwatch = new Stopwatch();

                string functionName = fun.GetType().Name;
                double activationElapsed;
                double derivativeElapsed = 0;

                Console.Write(functionName.PadRight(35));

                // Aktywacja
                stopwatch.Start();
                fun.ActivationFunction(arr, 0, arr.Length);
                stopwatch.Stop();

                activationElapsed = stopwatch.Elapsed.TotalMilliseconds;
                Console.Write(activationElapsed.ToString("0.000").PadRight(10));

                // Pochodna
                if (fun.HasDerivative)
                {
                    stopwatch.Restart();
                    for (int j = 0; j < arr.Length; j++)
                        fun.DerivativeFunction(arr[j], 0);
                    stopwatch.Stop();

                    derivativeElapsed = stopwatch.Elapsed.TotalMilliseconds;
                    Console.WriteLine(derivativeElapsed.ToString("0.000"));
                }
                else
                {
                    Console.WriteLine("-");
                }


                results.Add(functionName + ";" + activationElapsed.ToString() + ";" + derivativeElapsed.ToString());
            }

            if(options.ContainsKey("s"))
            {
                File.WriteAllLines(options["s"], results);
            }
        }

        IEnumerable<IActivationFunction> getAllActivationFuns()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(IActivationFunction));

            return from t in assembly.GetTypes()
                       where t.GetInterfaces().Contains(typeof(IActivationFunction))
                                && t.GetConstructor(Type.EmptyTypes) != null
                       select Activator.CreateInstance(t) as IActivationFunction;
        }
    }
}
