

namespace Slide01
{
    class Program
    {
        static void Main()
        {
            System.Console.WriteLine(Min(1, 2, 3));
        }

        static int Min(int a, int b, int c)
        {
            return System.Math.Min(a, System.Math.Min(b, c));
        }
    }
}
