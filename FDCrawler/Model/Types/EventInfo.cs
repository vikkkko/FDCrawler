using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace FDCrawler.Model.Types
{
    [BsonIgnoreExtraElements]
    class EventInfo
    {
        public List<string> fields;
        public List<string> argsTypes;
        public List<bool> indexeds;
        public string eventHash;
        public string eventName;
    }
}
