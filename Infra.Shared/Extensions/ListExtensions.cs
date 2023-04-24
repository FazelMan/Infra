using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Infra.Shared.Extensions
{
    public static class ListExtensions
    {
        public static bool IsNullOrEmpty(this IEnumerable @this)
        {
            if (@this != null)
                return !@this.GetEnumerator().MoveNext();
            return true;
        }

        //public static void Remove<TSource>(
        //    this IList<TSource> source,
        //    Func<TSource, bool> predicate)
        //{
        //    var items = source.Where(predicate).ToList();

        //    foreach (var item in items)
        //        source.Remove(item);
        //}

        public static List<TSource> AddWithNullCheck<TSource>(
            this List<TSource> source,
            TSource item)
        {
            if (source == null)
                source = new List<TSource>();

            source.Add(item);

            return source;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> biggerSet, IEnumerable<T> smallerSet)
        {
            return !smallerSet.Except(biggerSet).Any();
        }

        public static TSource PopAt<TSource>(this List<TSource> list, int index)
        {
            TSource r = list[index];
            list.RemoveAt(index);
            return r;
        }


        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            T[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                {
                    bucket = new T[size];
                }

                bucket[count++] = item;

                if (count != size)
                {
                    continue;
                }

                yield return bucket.Select(x => x);

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
            {
                yield return bucket.Take(count);

            }
        }


        public static IEnumerable<List<T>> Split<T>(this List<T> source, int nSize = 30)
        {
            for (int i = 0; i < source.Count; i += nSize)
            {
                yield return source.GetRange(i, Math.Min(nSize, source.Count - i));

            }
        }

        public static List<TSource> ConvertToList<TSource>(this TSource source)
        {
            if (source == null)
                return new List<TSource>();

            return new List<TSource>() { source };
        }
    }
}

