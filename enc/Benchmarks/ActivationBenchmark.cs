using Encog.Engine.Network.Activation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace enc
{
    class ActivationBenchmark : IExperiment
    {
        public string Command => "a";

        public string Description => "Testuje czas obliczania funkcji aktywacji na podstawie losowo wygenerowanej tablicy.\n" +
            "-a int: rozmiar tablicy do obliczeń. Domyślnie 20000000.\n" +
            "-m double: maksymalna wartość bezwzględna elementów tablicy. Domyślnie 50.\n" +
            "-s string: jeżeli podano, to zapisuje wynik do pliku w formacie .csv.";
        
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
            int size = ExperimentOptions.getParameterInt(options, "a", 20000000);
            double maxabs = ExperimentOptions.getParameterDouble(options, "m", 50.0);
            
            var functions = GetAllActivationFuns();

            List<String> results = new List<string>
            {
                "Funkcja;Wartość;Pochodna"
            };

            // Test
            Console.WriteLine("Funkcja                            Wartość   Pochodna");
            foreach (var fun in functions)
            {
                // Na wszelki wypadek
                GC.Collect();
                GC.WaitForPendingFinalizers();

                double[] after = RandomArray(size, -maxabs, maxabs, 1);
                double[] before = RandomArray(size, -maxabs, maxabs, 1);
                var stopwatch = new Stopwatch();

                string functionName = fun.GetType().Name;
                double activationElapsed;
                double derivativeElapsed = 0;

                Console.Write(functionName.PadRight(35));

                // Aktywacja
                stopwatch.Start();
                fun.ActivationFunction(after, 0, after.Length);
                stopwatch.Stop();

                activationElapsed = stopwatch.Elapsed.TotalMilliseconds;
                Console.Write(activationElapsed.ToString("0.000").PadRight(10));

                // Pochodna
                if (fun.HasDerivative)
                {
                    stopwatch.Restart();
                    for (int j = 0; j < after.Length; j++)
                        fun.DerivativeFunction(before[j], after[j]);
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

        IEnumerable<IActivationFunction> GetAllActivationFuns()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(IActivationFunction));

            return from t in assembly.GetTypes()
                       where t.GetInterfaces().Contains(typeof(IActivationFunction))
                                && t.GetConstructor(Type.EmptyTypes) != null
                       select Activator.CreateInstance(t) as IActivationFunction;
        }
    }
}
