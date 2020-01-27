using System.Linq;

using Market.CustomersAndStaff.ModelValidators.Commons;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests.ValidatorsTests
{
    public class StringValidatorsTests
    {
        [TestCase("абвгдеёжзийклмнопрстуфхцчшщъыьэюя АБВГЕЁЖЗИЙКЛМОНПРСТУФХЦЧШЩЪЫЬЭЮЯ", ExpectedResult = true)]
        [TestCase("Петухов-Рыбкин", ExpectedResult = true)]
        [TestCase("Петухов.Рыбкин", ExpectedResult = true)]
        [TestCase("-", ExpectedResult = true)]
        [TestCase(".", ExpectedResult = true)]

        [TestCase("Василiй", ExpectedResult = true)]
        [TestCase("Васил1й", ExpectedResult = true)]
        [TestCase("`~!@#$%^&*()_+={}[:\"|;'\\,/<>?", ExpectedResult = false)]
        [TestCase("abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ", ExpectedResult = true)]
        [TestCase("1234567890", ExpectedResult = true)]
        public bool ValidateLettersAndNumbers(string name)
        {
            return StringValidators.ValidateLettersAndNumbers(name);
        }

        [TestCase("7899", ExpectedResult = true)]
        [TestCase("789d9", ExpectedResult = false)]
        [TestCase("+7899", ExpectedResult = false)]
        [TestCase("8(999)5889", ExpectedResult = false)]
        public bool PhoneValidatorTest(string phone)
        {
            return StringValidators.ValidatePhone(phone);
        }
    }
}