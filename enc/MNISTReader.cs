using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enc
{
    class MNISTReader
    {
        public static double[] readLabels(string path)
        {
            FileStream file = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(file);

            int magic = br.ReadInt32BE();
            int items = br.ReadInt32BE();

            double[] ret = new double[items];

            for (int i = 0; i < items; i++)
                ret[i] = br.ReadByte();

            file.Close();
            br.Close();

            return ret;
        }

        static public double[][] readIdx(String fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(file);


            int magic = br.ReadInt32BE();
            int dim = magic & 0xff;

            int items = br.ReadInt32BE();
            int itemSize = 1;
            for (int i = 0; i < dim - 1; i++)
            {
                itemSize *= br.ReadInt32BE();
            }

            double[][] ret = new double[items][];

            for (int i = 0; i < items; i++)
            {
                ret[i] = new double[itemSize];
                byte[] read = br.ReadBytes(itemSize);

                for (int j = 0; j < itemSize; j++)
                {
                    ret[i][j] = read[j];
                }
            }

            file.Close();
            br.Close();

            return ret;
        }
    }
}
