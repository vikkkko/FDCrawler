using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

namespace CrawlerForEth.IO.Mongodb
{
    public abstract class Cache<T> : ICache<T>
    {
        public Cache(IMongoDatabase database)
        {
            coll = database.GetCollection<T>(collName);
        }

        public IMongoCollection<T> coll { get; }

        public abstract string collName { get; }

        public int Count => sets.Count;

        public bool IsReadOnly => false;

        public HashSet<T> sets = new HashSet<T>();

        private SemaphoreSlim slim = new SemaphoreSlim(1);

        public virtual void Clear()
        {
            sets.Clear();
        }

        public virtual void Add(T item)
        {
            slim.Wait();
            sets.Add(item);
            slim.Release();
        }

        public virtual void Add(IList<T> items)
        {
            slim.Wait();
            sets.UnionWith(items);
            slim.Release();
        }

        public bool Contains(T item)
        {
            return sets.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(T item)
        {
            if (Contains(item))
            {
                sets.Remove(item);
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return sets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public virtual async Task Commit(IClientSessionHandle session)
        {
            slim.Wait();
            if (sets.Count != 0)
            {
                await this.InsertMany(session, sets.ToList());
                Clear();
            }
            slim.Release();
        }
    }
}
