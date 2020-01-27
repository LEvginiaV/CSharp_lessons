using System.Linq;
using System.Text;

using Kontur.Selone.Properties;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations
{
    public class PhonePropTransformation : IPropTransformation<string>
    {
        public PhonePropTransformation(bool startsWithCode, string defaultValue)
        {
            this.startsWithCode = startsWithCode;
            this.defaultValue = defaultValue;
        }

        public string Deserialize(string value)
        {
            if(value == defaultValue)
            {
                return null;
            }

            if(startsWithCode)
            {
                value = value.Substring(3);
            }

            return new string(new []{'7'}.Concat(value.Where(char.IsDigit)).ToArray());
        }

        public string Serialize(string value)
        {
            if(value == null)
                return defaultValue;

            var builder = new StringBuilder(value);
            
            builder.Remove(0, 1);
            if(builder.Length >= 3)
            {
                builder.Insert(3, ' ');
            }
            if(builder.Length >= 7)
            {
                builder.Insert(7, '-');
            }
            if (builder.Length >= 10)
            {
                builder.Insert(10, '-');
            }

            return (startsWithCode ? "+7 " : "") + builder.ToString();
        }

        private readonly bool startsWithCode;
        private readonly string defaultValue;
    }
}