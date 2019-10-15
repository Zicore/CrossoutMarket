using System;
using System.Collections.Generic;

namespace Crossout.Web.Cache
{
    public class BasicCache<T, T2>
    {
        private readonly Dictionary<T, CacheEntry<T, T2>> cache = new Dictionary<T, CacheEntry<T, T2>>();
        //private static object lockObject = new object();

        public CacheEntry<T, T2> Get(T id, Func<T, T2> loadEntry, DateTime dateTime, TimeSpan expirationTimeSpan)
        {
            if (Contains(id))
            {
                if (IsExpired(id, dateTime))
                {
                    cache[id].Value = loadEntry(id);
                    cache[id].ExpirationDateTime = DateTime.Now + expirationTimeSpan;
                }
            }
            else
            {
                cache[id] = new CacheEntry<T, T2>
                {
                    Id = id,
                    Value = loadEntry(id),
                    ExpirationDateTime = DateTime.Now + expirationTimeSpan
                };
            }

            return cache[id];
        }
        
        public bool Contains(T id)
        {
            return cache.ContainsKey(id);
        }

        public bool IsExpired(T id, DateTime dateTime)
        {
            return cache[id].ExpirationDateTime < dateTime;
        }
    }
}
