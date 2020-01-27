using System.Collections.Generic;

namespace Market.CustomersAndStaff.Utils.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            if(dict.TryGetValue(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey? key, TValue defaultValue = default(TValue)) where TKey: struct
        {
            if (key != null && dict.TryGetValue(key.Value, out var value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}