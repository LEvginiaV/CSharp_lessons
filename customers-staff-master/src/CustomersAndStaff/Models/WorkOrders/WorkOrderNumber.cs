using System;
using System.Collections.Generic;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Market.CustomersAndStaff.Models.WorkOrders
{
    public struct WorkOrderNumber
    {
        public static readonly WorkOrderNumber Min = new WorkOrderNumber {Series = "АА", Number = 000001};
        public static readonly WorkOrderNumber Max = new WorkOrderNumber {Series = "ЯЯ", Number = 999999};

        public string Series { get; set; }
        public int Number { get; set; }

        public WorkOrderNumber(string series, int number)
        {
            Series = series;
            Number = number;
        }

        public bool Equals(WorkOrderNumber other)
        {
            return string.Equals(Series, other.Series, StringComparison.Ordinal) && Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is WorkOrderNumber other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Series?.GetHashCode() ?? 0) * 397) ^ Number;
            }
        }

        public static WorkOrderNumber operator +(WorkOrderNumber number, int value)
        {
            var strPart = number.Series;
            var numPart = number.Number;

            numPart += value;

            while(numPart > 999999)
            {
                strPart = IncrementStringPart(strPart);
                numPart -= 999999;
            }
            while(numPart < 1)
            {
                strPart = DecrementStringPart(strPart);
                numPart += 999999;
            }

            return new WorkOrderNumber
                {
                    Series = strPart,
                    Number = numPart,
                };
        }

        public static WorkOrderNumber operator -(WorkOrderNumber number, int value)
        {
            return number + -value;
        }

        public static bool operator ==(WorkOrderNumber number1, WorkOrderNumber number2)
        {
            return number1.Equals(number2);
        }

        public static bool operator !=(WorkOrderNumber number1, WorkOrderNumber number2)
        {
            return !(number1 == number2);
        }

        public static bool operator >(WorkOrderNumber number1, WorkOrderNumber number2)
        {
            return string.Compare(number1.Series, number2.Series, StringComparison.Ordinal) > 0 || 
                   number1.Series.Equals(number2.Series, StringComparison.Ordinal) && number1.Number > number2.Number;
        }

        public static bool operator <(WorkOrderNumber number1, WorkOrderNumber number2)
        {
            return string.Compare(number1.Series, number2.Series, StringComparison.Ordinal) < 0 ||
                   number1.Series.Equals(number2.Series, StringComparison.Ordinal) && number1.Number < number2.Number;
        }

        public override string ToString()
        {
            return $"{Series}-{Number:000000}";
        }

        public static WorkOrderNumber Parse(string str)
        {
            var parts = str.Split('-');
            return new WorkOrderNumber
                {
                    Series = parts[0],
                    Number = int.Parse(parts[1]),
                };
        }

        private static string IncrementStringPart(string str)
        {
            var chars = str.ToCharArray();
            chars[1] = IncrementChar(chars[1]);
            if(chars[1] > 'Я')
            {
                chars[0] = IncrementChar(chars[0]);
                chars[1] = 'А';

                if(chars[0] > 'Я')
                {
                    throw new ArgumentException("Result value is greater then max");
                }
            }

            return new string(chars);
        }

        private static string DecrementStringPart(string str)
        {
            var chars = str.ToCharArray();
            chars[1] = DecrementChar(chars[1]);
            if (chars[1] < 'А')
            {
                chars[0] = DecrementChar(chars[0]);
                chars[1] = 'Я';

                if (chars[0] < 'А')
                {
                    throw new ArgumentException("Result value is less then min");
                }
            }

            return new string(chars);
        }

        private static char IncrementChar(char ch)
        {
            do
            {
                ch++;
            } while(deniedChars.Contains(ch));

            return ch;
        }

        private static char DecrementChar(char ch)
        {
            do
            {
                ch--;
            } while (deniedChars.Contains(ch));

            return ch;
        }

        private static readonly HashSet<char> deniedChars = new HashSet<char>(new[] {'Ё', 'З', 'Й', 'О', 'Ч', 'Ь', 'Ы', 'Ъ'});
    }
}