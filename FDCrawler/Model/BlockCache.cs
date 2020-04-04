using CrawlerForEth.IO.Mongodb;
using MongoDB.Driver;
using Nethereum.RPC.Eth.DTOs;

namespace FDCrawler.Model
{
    public class BlockCache : Cache<Block>
    {
        public override string collName => "blocks";

        public BlockCache(IMongoDatabase database):base(database)
        {
        }
    }
}
