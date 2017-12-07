using System.Collections.Generic;
using System.Linq;

namespace MyApp.Types.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool OrderlessSequenceEquals<T>(this IEnumerable<T> src, IEnumerable<T> other) =>
            src?.OrderBy(e => e)?.SequenceEqual(other?.OrderBy(e => e) ?? Enumerable.Empty<T>()) ?? false;
    }
}