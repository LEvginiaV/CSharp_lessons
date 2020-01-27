using System.Text.RegularExpressions;

namespace Market.CustomersAndStaff.ModelValidators.Commons
{
    public class StringValidators
    {
        public static bool ValidateLettersAndNumbers(string str)
        {
            return Regex.IsMatch(str, @"^[а-яА-Яa-zA-Z0-9\s-\.ёЁ]+$");
        }

        public static bool ValidatePhone(string str)
        {
            return Regex.IsMatch(str, @"^[\d]+$");
        }
    }
}