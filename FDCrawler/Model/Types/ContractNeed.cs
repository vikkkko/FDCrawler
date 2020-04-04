using MongoDB.Bson.Serialization.Attributes;

namespace FDCrawler.Model.Types
{
    [BsonIgnoreExtraElements]
    class ContractNeed
    {
        public string contractHash;
        public EnumExecuteType type;
    }
    enum EnumExecuteType
    {
        UNKONW,
        EXECUTE,
        PENDING
    }
}
