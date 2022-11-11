using System.Collections.Generic;

namespace Lan.Shapes
{
    public static class CollectionExtension
    {
        public static void AddRange<T>(this ICollection<T> sourceCollection, IEnumerable<T> targetCollection)
        {
            foreach (var targetItem in targetCollection)
            {
                sourceCollection.Add(targetItem);
            }
        }
    }
}
