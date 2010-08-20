using System;
using System.Collections.Generic;

namespace SimpleCqrs.Core
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach(var item in source)
                action(item);

            return source;
        }
    }
}