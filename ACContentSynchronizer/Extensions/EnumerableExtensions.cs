using System;
using System.Collections.Generic;
using System.Linq;

namespace ACContentSynchronizer.Extensions {
  public static class EnumerableExtensions {
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> enumerable,
                                                                 Func<TSource, TKey> keySelector) {
      return enumerable.GroupBy(keySelector)
        .Select(x => x.First());
    }
  }
}
