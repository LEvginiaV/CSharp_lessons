using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutWhiteSpace
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine(RemoveStartSpaces("a"));
            Console.WriteLine(RemoveStartSpaces(" b"));
            Console.WriteLine(RemoveStartSpaces(" cd"));
            Console.WriteLine(RemoveStartSpaces(" efg"));
            Console.WriteLine(RemoveStartSpaces(" text"));
            Console.WriteLine(RemoveStartSpaces(" two words"));
            Console.WriteLine(RemoveStartSpaces("  two spaces"));
            Console.WriteLine(RemoveStartSpaces("	tabs"));
            Console.WriteLine(RemoveStartSpaces("		two	tabs"));
            Console.WriteLine(RemoveStartSpaces("                             many spaces"));
            Console.WriteLine(RemoveStartSpaces("               "));
            Console.WriteLine(RemoveStartSpaces("\n\r line breaks are spaces too"));
        }

        public static string RemoveStartSpaces(string text)
        {
            int i = 0;
            while (i < text.Length && char.IsWhiteSpace(text[i]))
                i += 1;
            return text.Substring(i);
        }
    }
}
