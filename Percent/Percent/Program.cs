using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percent
{
    class Program
    {
        public static double Calculate(string userInput)
        {
            var first = userInput.IndexOf(" ");
            double sum = Convert.ToDouble(userInput.Substring(0, first));
            var second = userInput.LastIndexOf(" ");
            var percentMonth = Convert.ToDouble(userInput.Substring(first+1, second-first)) / 12;
            int time = Convert.ToInt16(userInput.Substring(second, userInput.Length-second));
            double result = sum * Math.Pow(1 + percentMonth / 100, time);
            /*Console.WriteLine(sum);
            Console.WriteLine(percent);
            Console.WriteLine(time);*/
            return result;
        }


        static void Main(string[] args)
        {
            string inputData = Console.ReadLine();
            Console.WriteLine(Calculate(inputData));
        }
    }
}
