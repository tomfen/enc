using Encog.Neural.Networks;
using Encog.ML.Data;
using System;
using Encog.ML.Data.Specific;
using Encog.ML.Data.Basic;
using Encog.ML;
using System.Linq;
using Encog.Util;

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

        static public double Accuracy(IMLRegression network, BasicMLDataSet validationSet)
        {
            double correct = 0;
            double wrong = 0;
            for (int i = 0; i < validationSet.Count; i++)
            {
                if (EngineArray.MaxIndex(network.Compute(validationSet[i].Input)) ==
                    EngineArray.MaxIndex(validationSet[i].Ideal))
                    correct += 1;
                else
                    wrong += 1;
            }

            return correct / (correct + wrong);
        }
    }

}
