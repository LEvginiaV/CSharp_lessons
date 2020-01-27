using System;
using System.Linq;

using Kontur.Selone.Properties;

using Market.CustomersAndStaff.Models.Customers;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations
{
    public class BirthdayPropTransformation : IPropTransformation<Birthday>
    {
        public Birthday Deserialize(string value)
        {
            return Parse(value);
        }

        public string Serialize(Birthday value)
        {
            return ToString(value);
        }

        private static Birthday Parse(string value)
        {
            var tokens = value.Split(' ');
            if(tokens.Length != 2 && (tokens.Length != 4 || tokens[3] != "г."))
            {
                throw new FormatException($"Wrong date string {value}");
            }

            if(!int.TryParse(tokens[0], out var day))
            {
                throw new FormatException($"Wrong date string {value}");
            }

            var month = months.Select((x, i) => (Key: i, Value: x)).FirstOrDefault(x => x.Value == tokens[1]).Key;
            if(month == 0)
            {
                throw new FormatException($"Wrong date string {value}");
            }

            var birthday = new Birthday(day, month);

            if(tokens.Length == 4)
            {
                if(!int.TryParse(tokens[2], out var year))
                {
                    throw new FormatException($"Wrong date string {value}");
                }

                birthday.Year = year;
            }

            return birthday;
        }

        private static string ToString(Birthday birthday)
        {
            return $"{birthday.Day} {months[birthday.Month]}" + (birthday.Year != null ? $" {birthday.Year} г." : "");
        }

        private static readonly string[] months = new[]
            {
              "",
              "января",
              "февраля",
              "марта",
              "апреля",
              "мая",
              "июня",
              "июля",
              "августа",
              "сентября",
              "октября",
              "ноября",
              "декабря",
            };
    }
}