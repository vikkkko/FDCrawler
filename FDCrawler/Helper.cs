using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using FDCrawler.Model.Types;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Numerics;
using Nethereum.Hex.HexTypes;

namespace FDCrawler
{
    static class Helper
    {
        public static async Task<JArray> GetLogsFromAmber(string contractAddress)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-api-key", "UAKc2836bc84f9326ce4737b7dab5be974d");
            client.DefaultRequestHeaders.Add("x-amberdata-blockchain-id", "1c9c969065fcd1cf");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Postman-Token", "116261b1-6542-48a4-8131-f39c5d009ee6");
            JArray ja = new JArray();
            int i = 0;
            while (true)
            {
                Uri uri = new Uri(string.Format("https://web3api.io/api/v2/addresses/{0}/logs?page={1}&size=100", contractAddress, i));
                var json = JObject.Parse(await (await client.GetAsync(uri)).Content.ReadAsStringAsync());
                JArray _ja = (JArray)json["payload"]["records"];
                if (_ja.Count == 0)
                    break;
                ja.Merge(_ja);
                i++;
            }
            JArray newJa = new JArray();
            //因为amber的数据不大一样，所以需要额外处理一下
            for (var index = ja.Count - 1; index >= 0; index--)
            {
                var j = ja[index];
                var data = "";
                foreach (var d in j["data"])
                    data += d;
                j["data"] = data;
                newJa.Add(j);
            }

            return newJa;
        }

        public static BigInteger ToBigInteger(this string str)
        {
            try
            {
                if (str.IndexOf("0x") == 0)
                {
                    return (new HexBigInteger(str)).Value;
                }
                else
                {
                    return BigInteger.Parse(str);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
