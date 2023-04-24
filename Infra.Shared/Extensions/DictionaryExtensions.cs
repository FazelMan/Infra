using System.Collections.Generic;

namespace Infra.Shared.Extensions
{
	public static class DictionaryExtensions
	{
        public static void AddRange<T, TU>(this Dictionary<T, TU> dicOne, Dictionary<T, TU> dicTwo)
        {
            foreach (var element in dicTwo)
            {
	            dicOne[element.Key] = element.Value;
            }
        }
    }
}
