using System;

namespace Market.CustomersAndStaff.Models.Customers
{
    public class Birthday
    {
        public Birthday()
        {
            
        }

        public Birthday(int day, int month, int? year = null)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public int? Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public static Birthday Parse(string str)
        {
            var parts = str.Split('.');
            if(parts.Length < 2 || parts.Length > 3 || parts[0].Length != 2 || parts[1].Length != 2 || parts.Length == 3 && parts[2].Length != 4)
            {
                throw new FormatException("Expected dd.mm.yyyy or dd.mm");
            }

            var day = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var year = parts.Length == 3 ? int.Parse(parts[2]) : (int?)null;

            return new Birthday(day, month, year);
        }

        public override string ToString()
        {
            if(Year != null)
            {
                return $"{Day:00}.{Month:00}.{Year}";
            }

            return $"{Day:00}.{Month:00}";
        }

        private bool Equals(Birthday other)
        {
            return Year == other.Year && Month == other.Month && Day == other.Day;
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((Birthday)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Year.GetHashCode();
                hashCode = (hashCode * 397) ^ Month;
                hashCode = (hashCode * 397) ^ Day;
                return hashCode;
            }
        }
    }
}