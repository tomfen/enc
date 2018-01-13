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
        public static string F1(IMLRegression network, IMLDataSet testSet)
        {
            double[] false_pos = new double[testSet.IdealSize + 1];
            double[] false_neg = new double[testSet.IdealSize + 1];
            double[] true_pos = new double[testSet.IdealSize + 1];
            double[] true_neg = new double[testSet.IdealSize + 1];

            for (int i = 0; i < testSet.Count; i++)
            {
                var comp = network.Compute(testSet[i].Input);

                for (int j = 0; j < comp.Count; j++)
                {
                    if (testSet[i].Ideal[j] > 0.0)
                    {
                        if (comp[j] > 0.0)
                            true_pos[j]++;
                        else
                            false_neg[j]++;
                    }
                    else
                    {
                        if (comp[j] > 0.0)
                            false_pos[j]++;
                        else
                            true_neg[j]++;
                    }
                }
            }

            double[] recall = new double[testSet.IdealSize+1];
            double[] precision = new double[testSet.IdealSize+1];
            double[] f1 = new double[testSet.IdealSize+1];

            false_pos[testSet.IdealSize] = false_pos.Sum();
            false_neg[testSet.IdealSize] = false_neg.Sum();
            true_pos[testSet.IdealSize] = true_pos.Sum();
            true_neg[testSet.IdealSize] = true_neg.Sum();

            for (int j = 0; j < testSet.IdealSize + 1; j++)
            {
                recall[j] = (double)true_pos[j] / (true_pos[j] + false_pos[j]);
                precision[j] = (double)true_pos[j] / (true_pos[j] + false_neg[j]);
                f1[j] = 2.0 * (precision[j] * recall[j]) / +(precision[j] + recall[j]);
                if (Double.IsNaN(f1[j])) f1[j] = 0;
            }

            return string.Join("\n", f1);

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
            const double threshold = 0.5;
            Dictionary<string, double> ret = new Dictionary<string, double>();

            // podziel na klasy
            Tuple<double, bool>[][] d = new Tuple<double, bool>[testSet.IdealSize][];

            for (int i=0; i < testSet.IdealSize; i++)
            {
                d[i] = new Tuple<double, bool>[testSet.Count];
            }

            for (int i=0; i < testSet.Count; i++)
            {
                var pred = network.Compute(testSet[i].Input);
                var ideal = testSet[i].Ideal;

                for (int j = 0; j < testSet.IdealSize; j++)
                {
                    d[j][i] = new Tuple<double, bool>(pred[j], ideal[j] > threshold);
                }
            }
            

            for (int i = 0; i < testSet.IdealSize; i++)
                ret.Add(labels[i], BreakEven(d[i]));

            Tuple<double, bool>[] macro = new Tuple<double, bool>[0];

            for (int i = 0; i < testSet.IdealSize; i++)
                macro = macro.Concat(d[i]).ToArray();

            ret.Add("Macro Avg.", ret.Values.Average());
            ret.Add("Micro Avg.", BreakEven(macro));

            return ret;
        }

        private static double BreakEven(Tuple<double, bool>[] d)
        {
            d = d.OrderByDescending(x => x.Item1).ToArray();
            
            int fn = d.Count(x => x.Item2);
            int tn = d.Count(x => !x.Item2);
            int tp = 0;
            int fp = 0;

            double precisionPrev = 1;
            double recallPrev = 0;
            double precision = 1;
            double recall = 0;

            for (int i = 0; i < d.Length; i++)
            {
                if (d[i].Item2)
                {
                    tp++;
                    fn--;
                }
                else
                {
                    fp++;
                    tn--;
                }

                precision = (double)tp / (tp + fp);
                recall = (double)tp / (tp + fn);

                if (recall >= precision)
                    break;

                precisionPrev = recall;
                recallPrev = precision;
            }

            return (recall + precision) / 2;
        }
    }

}
