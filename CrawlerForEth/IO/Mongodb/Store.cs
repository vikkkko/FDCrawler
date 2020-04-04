using CrawlerForEth.IO.Mongodb;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerForEth.IO.Mongodb
{
    public static class Store
    {
        public static List<T> Find<T>(this Cache<T> cache, BsonDocument filter, BsonDocument sort, int skip = 0, int limit = 0)
        {
            return cache.coll.Find(filter).Sort(sort).Skip(skip).Limit(limit).ToList();
        }

        public static List<T> Find<T>(this Cache<T> cache, BsonDocument filter, int skip = 0, int limit = 0)
        {
            return cache.coll.Find<T>(filter).Skip(skip).Limit(limit).ToList();
        }

        public static List<T> Find<T>(this Cache<T> cache, BsonDocument filter)
        {
            return cache.coll.Find(filter.ToBsonDocument()).ToList();
        }

        public static async Task InsertMany<T>(this Cache<T> cache, IClientSessionHandle session, IList<T> list)
        {
            await cache.coll.InsertManyAsync(session,list);
        }

        public static async Task Update<T>(this Cache<T> cache,IClientSessionHandle session, BsonDocument filter ,T t)
        {
            await cache.coll.ReplaceOneAsync(session,filter, t);
        }
    }
}
