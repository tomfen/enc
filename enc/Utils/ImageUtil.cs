using OpenCvSharp;
using System;
using System.Runtime.InteropServices;

namespace enc
{
    class ImageUtil
    {
        static public Mat Deskew(Mat img)
        {
            int width = img.Width;
            int height = img.Height;

            var m = img.Moments();

            if (Math.Abs(m.Mu02) < 1e-2)
                return img;

            float skew = (float)(m.Mu11 / m.Mu02);
            var M = new MatOfFloat(2, 3, new float[,] { { 1, -skew, 0.5f * width * skew }, { 0, 1, 0 } });
            return img.WarpAffine(M, new Size(width, height));
        }

        static public double[] HOGVector(Mat img, HOGDescriptor hog)
        {
            float[] vectorF = hog.Compute(img);

            double[] ret = new double[vectorF.Length];
            Array.Copy(vectorF, ret, vectorF.Length);

            return ret;
        }

        static public double[] ImgVector(Mat img)
        {
            var floatMat = new Mat();
            img.ConvertTo(floatMat, MatType.CV_64F);
            floatMat /= 255;

            int length = floatMat.Cols * floatMat.Rows * floatMat.Channels();

            if (!floatMat.IsContinuous())
                throw new Exception("Mat must be continuous");

            var ret = new double[length];
            Marshal.Copy(floatMat.Data, ret, 0, length);

            return ret;
        }
    }
}
