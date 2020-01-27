using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    class Program
    {
        /// <summary>
        /// Expr1. Как поменять местами значения двух переменных? Можно ли это сделать без ещё одной временной переменной. Стоит ли так делать?
        /// </summary>
        public static void toChange2Variable ()
        {
            Console.Write("Введите a = ");
            double a = double.Parse(Console.ReadLine());
            Console.Write("Введите b = ");
            double b = double.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.WriteLine("Абра-кадабра - нажмите Enter для смены значений местами");
            Console.Read();
            a -= b;
            b += a;
            a = b - a;
            Console.WriteLine("a = " + a);
            Console.WriteLine("b = " + b);
        }

        /// <summary>
        /// Expr2. Задается натуральное трехзначное число (гарантируется, что трехзначное). Развернуть его, т.е. получить трехзначное число, записанное теми же цифрами в обратном порядке.
        /// </summary>
        public static void fromEndToStart ()
        {
            Console.Write("Введите трехзначное число a = ");
            int a = int.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.WriteLine("Абра-кадабра - нажмите Enter для записи числа наоборот");
            Console.Read();
            int b = a/100;
            int c = a%100/10;
            int d = a%10;
            int result = d*100+c*10+b;
            Console.WriteLine("Результат: " + result);
        }

        /// <summary>
        /// Expr3. Задано время Н часов (ровно). Вычислить угол в градусах между часовой и минутной стрелками. Например, 5 часов -> 150 градусов, 20 часов -> 120 градусов. Не использовать циклы.
        /// </summary>
        public static void findCorner ()
        {
            Console.WriteLine("Введите положение часовой стрелки от 1 до 12");
            int a = int.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.WriteLine("На часах " + a + " часов ровно, угол между часовой и минутной стрелками равен " + (6 - Math.Abs(6 - a)) * 30 + " градусам");
        }

        /// <summary>
        /// Expr4. Найти количество чисел меньших N, которые имеют простые делители X или Y.
        /// </summary>
        public static void findCountOfDividend ()
        {
            Console.WriteLine("Введите верхнее граничное значение N = ");
            int N = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите простой делитель X = ");
            int X = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите простой делитель Y = ");
            int Y = int.Parse(Console.ReadLine());
            Console.WriteLine();
            int result = (N - 1) / X + (N - 1) / Y - (N - 1) / (X * Y);
            Console.WriteLine("Количество чисел меньших " + N + " и кратных " + X + " или " + Y + " равно " + result);
        }

        /// <summary>
        /// Expr5. Найти количество високосных лет на отрезке [a, b] не используя циклы.
        /// </summary>
        public static void CountLeapYears ()
        {
            Console.WriteLine("Введите нижнюю границу года a = ");
            int a = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите нижнюю границу года b = ");
            int b = int.Parse(Console.ReadLine());
            Console.WriteLine();
            int result = b / 4 - (a - 1) / 4;
            Console.WriteLine("Количество високосных лет в диапазоне от " + a + " до " + b + " равно " + result);
        }

        /// <summary>
        /// Expr6. Посчитать расстояние от точки до прямой, заданной двумя разными точками.
        /// </summary>
        public static void findDistanceBetweemLineAndPoint()
        {
            Console.WriteLine("Введите  координаты точки A, принадлежащей линии (x1;y1) ");
            string A = Console.ReadLine();
            //Console.WriteLine(A.IndexOf(';', 0));
            double x1 = double.Parse(A.Substring(0, A.IndexOf(';', 0)));
            double y1 = double.Parse(A.Substring(A.IndexOf(';', 0)+1, A.Length- A.IndexOf(';', 0)-1));
            Console.WriteLine(x1);
            Console.WriteLine(y1);
            Console.WriteLine("Введите  координаты точки B, принадлежащей линии (x2;y2) ");
            string B = Console.ReadLine();
            double x2 = double.Parse(B.Substring(0, B.IndexOf(';', 0)));
            double y2 = double.Parse(B.Substring(B.IndexOf(';', 0) + 1, B.Length - B.IndexOf(';', 0) - 1));
            Console.WriteLine(x2);
            Console.WriteLine(y2);
            Console.WriteLine("Введите  координаты точки C, не лежащей на линии (x3;y3) ");
            string C = Console.ReadLine();
            double x3 = double.Parse(C.Substring(0, C.IndexOf(';', 0)));
            double y3 = double.Parse(C.Substring(C.IndexOf(';', 0) + 1, C.Length - C.IndexOf(';', 0) - 1));
            Console.WriteLine(x3);
            Console.WriteLine(y3);
            Console.WriteLine(Math.Abs(x3 * (y2 - y1) + y3 * (x1 - x2) + y1 * (x2 - x1) + x1 * (y1 - y2)));
            Console.WriteLine(Math.Sqrt(Math.Pow(y2 - y1, 2) + Math.Pow(x1 - x2, 2)));
            double result = Math.Abs(x3 * (y2 - y1) + y3 * (x1 - x2) + y1 * (x2 - x1) + x1 * (y1 - y2)) / Math.Sqrt(Math.Pow(y2 - y1, 2) + Math.Pow(x1 - x2, 2));
            Console.WriteLine("Расстояние от точки С(" + x3 + ";" + y3 + ") до прямой, заданной точками A(" + x1 + ";" + y1 + ") и B(" + x2 + ";" + y2 + ") равно " + result);
        }

        static void Main(string[] args)
        {
            findDistanceBetweemLineAndPoint();
        }
    }
}

