using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStudio.Extensions
{
	public static class EnumerableExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            foreach (var item in enumerable)
				action(item);
		}

		public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T,Task> action)
		{
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            foreach (var item in enumerable)
				await action(item);
		}

		public static bool None<T>(this IEnumerable<T> enumerable)
		{
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            return !enumerable.Any();
		}

		public static bool None<T>(this IEnumerable<T> enumerable, Func<T,bool> predicate)
		{
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            return !enumerable.Any(predicate);
		}

        static readonly Random Rand = new Random((int) ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeSeconds());

        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default(T) : list[Rand.Next(0, list.Count)];
        }

        public static async Task<Dictionary<TKey, TValue>> ToDictionaryAsync<TInput, TKey, TValue>(
			this IEnumerable<TInput> enumerable,
			Func<TInput, TKey> keySelector,
			Func<TInput, Task<TValue>> valueSelector)
		{
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            var dictionary = new Dictionary<TKey, TValue>();

			foreach (var item in enumerable)
			{
				var key = keySelector(item);

				var value = await valueSelector(item);

				dictionary.Add(key, value);
			}

			return dictionary;
		}
	}
}
