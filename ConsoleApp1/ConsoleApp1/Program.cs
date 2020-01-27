using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Program
    {
        static void Update(int[] b)
        {
            b[0]++;
            // ?
        }

        static void Main()
        {
            var a = new int[3][];
            a[0] = new int[0];
            a[2] = new int[2];
            Console.WriteLine(a[2]+"/");
            Update(a[2]);
            Console.WriteLine(a[2]);
        }
    }
}
