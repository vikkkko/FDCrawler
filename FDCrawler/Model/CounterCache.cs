using CrawlerForEth.IO.Mongodb;
using FDCrawler.Model.Types;
using MongoDB.Driver;

namespace FDCrawler.Model
{
    class CounterCache : Cache<Counter>
    {
        public override string collName => "counters";

        public CounterCache(IMongoDatabase database) : base(database)
        {
        }
    }
}
