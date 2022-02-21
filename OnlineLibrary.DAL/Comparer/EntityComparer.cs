using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OnlineLibrary.DAL.Comparer
{
    public class EntityComparer<T> : IEqualityComparer<T> where T : class
    {
        public bool Equals([AllowNull] T x, [AllowNull] T y)
        {
            return x.Equals(y);
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return base.GetHashCode();
        }
    }
}
