using System;

namespace Crossout.Web.Cache
{
    public class CacheEntry<T, T2>
    {
        public DateTime ExpirationDateTime { get; set; }
        public T Id { get; set; }

        public T2 Value { get; set; }
    }
}
