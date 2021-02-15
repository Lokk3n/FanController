using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanController.ServiceContainers
{
    public static class ExtensionClass
    {
        public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> enumerable)
        {
            var previous = default(T);

            using (var e = enumerable.GetEnumerator())
            {
                if (e.MoveNext())
                    previous = e.Current;

                while (e.MoveNext())
                    yield return Tuple.Create(previous, previous = e.Current);
            }
        }
    }
}
