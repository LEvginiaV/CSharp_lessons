using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataType
{
    class Program
    {
        static void Main(string[] args)
        {
            double pi = Math.PI;
            int tenThousand = 10000;
            float tenThousandPi = (float)pi * tenThousand;
            int roundedTenThousandPi = (int)Math.Round(tenThousandPi);
            int integerPartOfTenThousandPi = (int)tenThousandPi;
            Console.WriteLine(integerPartOfTenThousandPi);
            Console.WriteLine(roundedTenThousandPi);

        }
    }
}
