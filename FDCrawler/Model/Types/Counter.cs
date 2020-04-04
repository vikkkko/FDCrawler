using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace FDCrawler.Model.Types
{
    [BsonIgnoreExtraElements]
    class Counter
    {
        public string CollName;
        public long counter;
    }
}
