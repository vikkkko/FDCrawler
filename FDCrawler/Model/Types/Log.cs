using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace FDCrawler.Model.Types
{
    [BsonIgnoreExtraElements]
    class Log
    {
        public List<string> argsTypes { get; set; }
        public List<string> args { get; set; }
        public string address { get; set; }
        public string blockHash { get; set; }
        public BigInteger blockNumber { get; set; }
        public string data { get; set; }
        public BigInteger logIndex { get; set; }
        public List<string> topics { get; set; }
        public string transactionHash { get; set; }
        public BigInteger transactionIndex { get; set; }
        public string eventName { get; set; }
        public string eventHash { get; set; }
        public long counter;

        public static Log FromJObject(JToken jo)
        {
            Log log = new Log();
            log.address =(string)jo["address"];
            log.blockHash = (string)jo["blockHash"];
            log.blockNumber = ((string)jo["blockNumber"]).ToBigInteger();
            log.data = (string)jo["data"] ?? "";
            log.logIndex = ((string)jo["logIndex"]).ToBigInteger();
            log.topics = jo["topics"].ToObject<List<string>>();
            log.eventHash = log.topics[0];
            log.transactionHash = (string)jo["transactionHash"];
            log.transactionIndex = ((string)jo["transactionIndex"]).ToBigInteger();
            return log;
        }

        public void ExtendFromEventInfo(EventInfo eventInfo,long counter)
        {
            this.argsTypes = eventInfo.argsTypes;
            this.eventHash = eventInfo.eventHash;
            this.eventName = eventInfo.eventName;
            this.counter = counter;

            if (data == null)
                throw new ArgumentNullException();

            List<string> datas = new List<string>();
            data = data.Replace("0x","");
            int count = data.Length / 64;
            for(var i = 0;i<count;i++)
            {
                datas.Add(string.Format("0x{0}",data.Substring(i*64,64)));
            }
            //topics中的第一个字符串代表的是eventhash,所以从第二个开始
            int tIndex = 1;
            int dIndex = 0;
            args = new List<string>();
            foreach (var index in eventInfo.indexeds)
            {
                if (index == true)
                {
                    args.Add(topics[tIndex]);
                    tIndex++;
                }
                else
                {
                    args.Add(datas[dIndex]);
                    dIndex++;
                }
            }
        }
    }
}
