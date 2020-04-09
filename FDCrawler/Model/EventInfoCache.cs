using CrawlerForEth.IO.Mongodb;
using FDCrawler.Model.Types;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace FDCrawler.Model
{
    class EventInfoCache : Cache<EventInfo>
    {
        public override string collName => "eventInfos";

        public Dictionary<string, EventInfo> EventHash_All = new Dictionary<string, EventInfo>();

        public EventInfoCache(IMongoDatabase database) : base(database)
        {
        }

        public void FindAllByEventHash()
        {
            EventHash_All.Clear();
            var datas = this.Find(new MongoDB.Bson.BsonDocument());
            foreach (var d in datas)
            {
                EventHash_All.Add(d.eventHash,d);
            }
        }
    }
}
