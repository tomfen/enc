using Encog.Neural.Networks;
using Encog.ML.Data;
using System;
using Encog.ML.Data.Specific;
using Encog.ML.Data.Basic;

namespace enc{
public class Evaluation
{

    

    public static string F1(BasicNetwork network, IMLDataSet testSet)
    {
        double[] false_pos = new double[testSet.IdealSize];
        double[] false_neg = new double[testSet.IdealSize];
        double[] true_pos = new double[testSet.IdealSize];
        double[] true_neg = new double[testSet.IdealSize];

        for (int i = 0; i < testSet.Count; i++)
        {
            var comp = network.Compute(testSet[i].Input);

            for (int j = 0; j < comp.Count; j++)
            {
                if(testSet[i].Ideal[j] > 0.0)
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
        
        double[] recall = new double[testSet.IdealSize];
        double[] precision = new double[testSet.IdealSize];
        double[] f1 = new double[testSet.IdealSize];

        for (int j = 0; j < testSet.IdealSize; j++)
        {
            recall[j] = (double)true_pos[j] / (true_pos[j] + false_pos[j]);
            precision[j] = (double)true_pos[j] / (true_pos[j] + false_neg[j]);
            f1[j] = 2.0 * (precision[j] * recall[j]) / +(precision[j] + recall[j]);
            if (Double.IsNaN(f1[j])) f1[j] = 0;
        }
        
        return string.Join("\n", f1);
        
    }

    static public double accuracy(BasicNetwork network, BasicMLDataSet validationSet)
    {
        double correct = 0;
        double wrong = 0;
        for (int i = 0; i < validationSet.Count; i++)
        {
            int win = network.Winner(validationSet[i].Input);
            if (1.0 == validationSet[i].Ideal[win])
                correct += 1;
            else
                wrong += 1;
        }

        return correct / (correct + wrong);
    }
}

}
