using Encog.ML;
using Encog.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enc
{
    class NetworkUtil
    {
        public static int[] compute(IMLClassification classifier, IMLDataSet ds)
        {
            int[] ret = new int[ds.Count];
            
            for (int i = 0; i < ds.Count; i++)
                ret[i] = classifier.Classify(ds[i].Input);

            return ret;
        }
    }
}
