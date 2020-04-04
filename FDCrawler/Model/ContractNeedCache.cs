using CrawlerForEth.IO.Mongodb;
using FDCrawler.Model.Types;
using MongoDB.Driver;

namespace FDCrawler.Model
{
    class ContractNeedCache : Cache<ContractNeed>
    {
        public override string collName => "contractNeeds";
        public ContractNeedCache(IMongoDatabase database) : base(database)
        {
        }
    }
}
