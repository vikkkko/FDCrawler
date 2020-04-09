using CrawlerForEth.IO.Mongodb;
using FDCrawler.Model.Types;
using MongoDB.Driver;
using System.Collections.Generic;

namespace FDCrawler.Model
{
    class ContractNeedCache : Cache<ContractNeed>
    {
        public override string collName => "contractNeeds";

        public Dictionary<string, ContractNeed> ContractHash_ALL = new Dictionary<string, ContractNeed>();
        public ContractNeedCache(IMongoDatabase database) : base(database)
        {
        }

        public void FindAllByContractHash()
        {
            ContractHash_ALL.Clear();
            var datas = this.Find(new MongoDB.Bson.BsonDocument());
            foreach (var data in datas)
            {
                ContractHash_ALL.Add(data.contractHash,data);
            }
        }
    }
}
