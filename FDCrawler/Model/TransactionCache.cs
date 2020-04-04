using CrawlerForEth.IO.Mongodb;
using MongoDB.Driver;
using Nethereum.RPC.Eth.DTOs;

namespace FDCrawler.Model
{
    public class TransactionCache : Cache<Transaction>
    {
        public override string collName => "transactions";
        public TransactionCache(IMongoDatabase database) : base(database)
        {
        }
    }
}
