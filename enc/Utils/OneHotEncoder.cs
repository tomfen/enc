namespace enc
{
    class OneHotEncoder
    {
        private int classes;
        private double negative;
        private double positive;

        static public double[][] Transform(double[] Y, double negative = -1, double positive = 1)
        {
            int classes = 0;
            for (int i = 0; i < Y.Length; i++)
                if (classes < Y[i]+1)
                    classes = (int)Y[i]+1;

            double[][] ret = new double[Y.Length][];

            for (int i = 0; i < Y.Length; i++)
            {
                ret[i] = new double[classes];
                for (int j = 0; j < classes; j++)
                    ret[i][j] = j == Y[i] ? positive : negative;
            }

            return ret;
        }

        public OneHotEncoder(int classes, double negative = -1, double positive = 1)
        {
            this.classes = classes;
            this.negative = negative;
            this.positive = positive;
        }

        public double[] Transform(double Y)
        {
            double[] ret = new double[classes];
            
            for (int j = 0; j < classes; j++)
                ret[j] = j == Y ? positive : negative;

            return ret;
        }
    }
}
