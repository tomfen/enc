using Encog.ML.Data;
using System;
using Encog.ML;
using System.Linq;
using Encog.Util;
using System.Collections.Generic;

namespace enc
{
    public class Evaluation
    {
        public static Dictionary<string, double> F1(IMLRegression network, IMLDataSet testSet, String[] labels)
        {
            Dictionary<string, double> ret = new Dictionary<string, double>();

            var d = DivideByClass(network, testSet, 0.5);

            for (int i = 0; i < testSet.IdealSize; i++)
                ret.Add(labels[i], F1(d[i]));

            Tuple<double, bool>[] macro = new Tuple<double, bool>[0];

            for (int i = 0; i < testSet.IdealSize; i++)
                macro = macro.Concat(d[i]).ToArray();

            ret.Add("Macro Avg.", ret.Values.Average());
            ret.Add("Micro Avg.", F1(macro));

            return ret;
        }

        private static Tuple<double, bool>[][] DivideByClass(IMLRegression network, IMLDataSet testSet, double threshold)
        {
            // podziel na klasy
            Tuple<double, bool>[][] divided = new Tuple<double, bool>[testSet.IdealSize][];

            for (int i = 0; i < testSet.IdealSize; i++)
            {
                divided[i] = new Tuple<double, bool>[testSet.Count];
            }

            for (int i = 0; i < testSet.Count; i++)
            {
                var pred = network.Compute(testSet[i].Input);
                var ideal = testSet[i].Ideal;

                for (int j = 0; j < testSet.IdealSize; j++)
                {
                    divided[j][i] = new Tuple<double, bool>(pred[j], ideal[j] > threshold);
                }
            }

            return divided;
        }

        private static double F1(Tuple<double, bool>[] d)
        {
            double false_pos = 0;
            double false_neg = 0;
            double true_pos = 0;
            double true_neg = 0;

            for (int i = 0; i < d.Length; i++)
            {
                if (d[i].Item2)
                {
                    if (d[i].Item1 > 0.0)
                        true_pos++;
                    else
                        false_neg++;
                }
                else
                {
                    if (d[i].Item1 > 0.0)
                        false_pos++;
                    else
                        true_neg++;
                }
            }
            
            double recall = true_pos / (true_pos + false_pos);
            double precision = true_pos / (true_pos + false_neg);
            double f1 = 2.0 * (precision * recall) / (precision + recall);

            if (Double.IsNaN(f1))
                f1 = 0;

            return f1;
        }

        public static double Accuracy(IMLRegression network, IMLDataSet testSet)
        {
            double correct = 0;
            double wrong = 0;
            for (int i = 0; i < testSet.Count; i++)
            {
                if (EngineArray.MaxIndex(network.Compute(testSet[i].Input)) ==
                    EngineArray.MaxIndex(testSet[i].Ideal))
                    correct += 1;
                else
                    wrong += 1;
            }

            return correct / (correct + wrong);
        }

        public static double ErrorRate(IMLRegression network, IMLDataSet testSet)
        {
            return 1.0 - Accuracy(network, testSet);
        }

        public static Dictionary<string, double> BreakEven(IMLRegression network, IMLDataSet testSet, String[] labels)
        {
            
            Dictionary<string, double> ret = new Dictionary<string, double>();

            var d = DivideByClass(network, testSet, 0.5);
            
            for (int i = 0; i < testSet.IdealSize; i++)
                ret.Add(labels[i], BreakEven(d[i]));

            Tuple<double, bool>[] macro = new Tuple<double, bool>[0];

            for (int i = 0; i < testSet.IdealSize; i++)
                macro = macro.Concat(d[i]).ToArray();

            ret.Add("Macro Avg.", ret.Values.Average());
            ret.Add("Micro Avg.", BreakEven(macro));

            return ret;
        }

        private static double BreakEven(Tuple<double, bool>[] pairs)
        {
            pairs = pairs.OrderByDescending(x => x.Item1).ToArray();

            double fn = pairs.Count(x => x.Item2);
            double tn = pairs.Count(x => !x.Item2);
            double tp = 0;
            double fp = 0;

            double precisionPrev = 1;
            double recallPrev = 0;
            double precision;
            double recall;

            double best = 0;

            //Zmniejszamy próg
            for (int i = 0; i < pairs.Length; i++)
            {
                if (pairs[i].Item2)
                {
                    tp++;
                    fn--;
                }
                else
                {
                    fp++;
                    tn--;
                }

                precision = tp / (tp + fp);
                recall = tp / (tp + fn);

                if (recall == precision) {
                    best = Math.Max(best, recall);
                }
                else if (precisionPrev > recallPrev && precision < recall) //interpolacja
                {
                    double w1 = precisionPrev - recallPrev;
                    double w2 = recall - precision;
                    double weightedAvg = (precisionPrev * w2 + precision * w1) / (w1 + w2);
                    best = Math.Max(best, weightedAvg);
                }
                else if (precisionPrev < recallPrev && precision > recall)
                {
                    double w1 = recallPrev - precisionPrev; 
                    double w2 = precision - recall;
                    double weightedAvg = (precisionPrev * w2 + precision * w1) / (w1 + w2);
                    best = Math.Max(best, weightedAvg);
                }

                precisionPrev = recall;
                recallPrev = precision;
            }

            return best;
        }
    }

}
