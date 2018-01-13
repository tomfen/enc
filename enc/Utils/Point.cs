namespace enc.Utils
{
    public struct Point<T> where T: struct
    {
        Point(T x, T y)
        {
            X = x;
            Y = y;
        }

        public T X;
        public T Y;
    }
}
