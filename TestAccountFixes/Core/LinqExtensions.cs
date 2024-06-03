using System.Collections.Generic;
using System.Linq;

namespace TestAccountFixes.Core;

public static class LinqExtensions {
    public static bool None<TSource>(this IEnumerable<TSource> source) => !source.Any();
}