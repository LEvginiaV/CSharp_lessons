using System;

namespace Pluralize
{
	public static class PluralizeTask
	{
		public static string PluralizeRubles(int count)
		{
			// Напишите функцию склонения слова "рублей" в зависимости от предшествующего числительного count.
            int lastDigit = count % 10;
            int beforeLastDigit = count % 100 / 10;
            //Console.WriteLine (lastDigit + "/" + beforeLastDigit);
            if (beforeLastDigit != 1)
                {
                    if (lastDigit == 1)
                        return "рубль";
                    else if (lastDigit == 2 || lastDigit == 3 || lastDigit == 4)
                        return "рубля";
                    else return "рублей";
                }
            else return "рублей";
		}
	}
}