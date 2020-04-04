using CrawlerForEth.IO.Mongodb;
using FDCrawler.Model.Types;
using MongoDB.Driver;

namespace FDCrawler.Model
{
    class EventInfoCache : Cache<EventInfo>
    {
        public override string collName => "eventInfos";

        public EventInfoCache(IMongoDatabase database) : base(database)
        {
        }
    }
}
