using enc.Utils;
using System.Collections.Generic;
using System.IO;

namespace enc
{
    public class CSVWriter
    {
        public static void Save<T,Y>(string fileName, T[,] data, IEnumerable<Y> header = null, string delimiter=";")
        {
            using (var file = new StreamWriter(fileName))
            {
                if (header != null)
                    file.WriteLine(string.Join(delimiter, header));

                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        file.Write(data[i, j]);

                        if (j != data.GetLength(1))
                            file.Write(delimiter);
                    }
                    file.Write('\n');
                }
            }
        }

        public static void Save<T, Y>(string fileName, Point<T>[,] data, IEnumerable<Y> header = null, string delimiter = ";") where T:struct
        {
            //todo
            using (var file = new StreamWriter(fileName))
            {
                if (header != null)
                    file.WriteLine(string.Join(delimiter, header));

                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        file.Write(data[i, j]);

                        if (j != data.GetLength(1))
                            file.Write(delimiter);
                    }
                    file.Write('\n');
                }
            }
        }
    }
}
