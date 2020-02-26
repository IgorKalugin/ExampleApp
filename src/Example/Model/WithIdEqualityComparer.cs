using System.Collections.Generic;
using System.Diagnostics;

namespace Example.Model
{
    public class WithIdEqualityComparer : EqualityComparer<IWithId>
    {
        private static WithIdEqualityComparer instance;
        public static WithIdEqualityComparer Instance => instance ?? (instance = new WithIdEqualityComparer());

        private WithIdEqualityComparer()
        {
        }
        
        public override bool Equals(IWithId x, IWithId y)
        {
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            return x.Id == y.Id;
        }

        public override int GetHashCode(IWithId obj)
        {
            Debug.Assert(obj != null, nameof(obj) + " != null");
            return obj.Id.GetHashCode();
        }
    }
}