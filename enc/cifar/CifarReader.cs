using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace enc.cifar
{
    class CifarIterator : IEnumerator<Tuple<Mat, int>>, IEnumerable
    {
        private const int samplesize = 1 + 32 * 32 * 3;
        private const int samplesPerFile = 10000;

        private Tuple<Mat, int> _current;
        private FileStream file;
        private BinaryReader br;

        public CifarIterator(string filename)
        {
            file = new FileStream(filename, FileMode.Open);
            br = new BinaryReader(file);
        }

        public Tuple<Mat, int> Current => _current;

        object IEnumerator.Current => _current;

        static public void ReadImages(String fileName, out double[] labels, out Mat[] images)
        {
            images = new Mat[samplesPerFile];
            labels = new double[samplesPerFile];
            
            FileStream file = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(file);

            for (int sample = 0; sample < samplesPerFile; sample++)
            {
                labels[sample] = br.ReadByte();

                Mat R = new Mat(32, 32, MatType.CV_8UC1, br.ReadBytes(32 * 32 * 1));
                Mat G = new Mat(32, 32, MatType.CV_8UC1, br.ReadBytes(32 * 32 * 1));
                Mat B = new Mat(32, 32, MatType.CV_8UC1, br.ReadBytes(32 * 32 * 1));

                Mat img = new Mat();
                Cv2.Merge(new Mat[3] { B, G, R }, img);
                images[sample] = img;
            }

            file.Close();
            br.Close();
        }

        public void Dispose()
        {
            file.Close();
            br.Close();
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            if (br.BaseStream.Position != br.BaseStream.Length)
            {
                byte label = br.ReadByte();

                Mat R = new Mat(32, 32, MatType.CV_8UC1, br.ReadBytes(32 * 32 * 1));
                Mat G = new Mat(32, 32, MatType.CV_8UC1, br.ReadBytes(32 * 32 * 1));
                Mat B = new Mat(32, 32, MatType.CV_8UC1, br.ReadBytes(32 * 32 * 1));

                Mat img = new Mat();
                Cv2.Merge(new Mat[3] { B, G, R }, img);

                _current = new Tuple<Mat, int>(img, label);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
