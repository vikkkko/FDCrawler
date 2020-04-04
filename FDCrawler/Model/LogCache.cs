using System.Threading.Tasks;
using CrawlerForEth.IO.Mongodb;
using FDCrawler.Model.Types;
using MongoDB.Driver;

namespace FDCrawler.Model
{
    class LogCache : Cache<Log>
    {
        public override string collName => "logs";

        public long counter { get; private set; }

        public LogCache(IMongoDatabase database) : base(database)
        {
            counter = base.coll.CountDocuments("{}");
        }

        public override Task Commit(IClientSessionHandle session)
        {
            counter += sets.Count;
            return base.Commit(session);
        }
    }
}
