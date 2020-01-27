using System;

namespace Market.CustomersAndStaff.GodLikeTools
{
    public class BadCommandLineException : Exception
    {
        public BadCommandLineException(string format, params object[] parameters)
            : base(string.Format(format, parameters))
        {
        }
    }
}