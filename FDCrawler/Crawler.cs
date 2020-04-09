using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Linq;
using FDCrawler.Model;
using FDCrawler.Model.Types;
using MongoDB.Driver;
using MongoDB.Bson;
using CrawlerForEth.IO.Mongodb;
using System;
using CrawlerForEth;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace FDCrawler
{
    public class Crawler
    {
        private readonly Web3Manager web3Manager;
        private readonly MongoClient mongoClient;

        private ContractNeedCache contractNeedModel;
        private LogCache logModel;
        private EventInfoCache eventInfoModel;
        private CounterCache counterCache;

        public Crawler(Web3Manager web3Manager, MongoClient mongoClient)
        {
            this.web3Manager = web3Manager;
            this.mongoClient = mongoClient;
            contractNeedModel = new ContractNeedCache(mongoClient.GetDatabase(Settings.Ins.mongodbDatabase));
            logModel = new LogCache(mongoClient.GetDatabase(Settings.Ins.mongodbDatabase));
            eventInfoModel = new EventInfoCache(mongoClient.GetDatabase(Settings.Ins.mongodbDatabase));
            counterCache = new CounterCache(mongoClient.GetDatabase(Settings.Ins.mongodbDatabase));
        }

        public async Task Start()
        {
            //先获取要处理的高度
            var counters = counterCache.Find(new BsonDocument("CollName", "blockHeight"));
            long execBlockNumber = counters.Count == 0 ? -1 : counters[0].counter;
            while (true)
            {
                try
                {
                    //获取当前区块的高度
                    BigInteger currentBlockNumber = (await web3Manager.Current.Eth.Blocks.GetBlockNumber.SendRequestAsync()).Value;
                    if (execBlockNumber < currentBlockNumber)
                    {
                        using (var session = mongoClient.StartSession())
                        {
                            session.StartTransaction(new TransactionOptions(readConcern: ReadConcern.Snapshot, writeConcern: WriteConcern.WMajority));
                            try
                            {
                                Stopwatch sw = new Stopwatch();
                                sw.Start();
                                execBlockNumber++;
                                Console.WriteLine(string.Format("开始处理高度：{0}", execBlockNumber));
                                await CrawlAndExec(session, execBlockNumber);
                                ///更新高度
                                await counterCache.Update(session, new BsonDocument("CollName", "blockHeight"), new Counter() { CollName = "blockHeight", counter = execBlockNumber });
                                session.CommitTransaction();
                                sw.Stop();
                                Console.WriteLine(string.Format("结束处理高度：{0},处理耗时：{1}", execBlockNumber,sw.ElapsedMilliseconds));
                            }
                            catch (Exception e)
                            {
                                session.AbortTransaction();
                                throw e;
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    execBlockNumber--;
                    Console.WriteLine(e);
                    web3Manager.ChangeWeb3();
                }

                await Task.Delay(Settings.Ins.waitingTime);
            }
        }

        public async Task CrawlAndExec(IClientSessionHandle session,BigInteger execBlockNumber)
        {
            JArray logs = new JArray();
            //先判断有没有要预先入库的
            var contractNeedPending = contractNeedModel.Find(new BsonDocument("type", EnumExecuteType.PENDING), new BsonDocument(), 0, 1);
            if (contractNeedPending.Count !=0 && contractNeedPending[0].type == EnumExecuteType.PENDING)
            {
                var _logs =await Helper.GetLogsFromAmber(contractNeedPending[0].contractHash);
                logs.Merge(_logs);
                BsonDocument filter = new BsonDocument("type", EnumExecuteType.PENDING);
                filter["contractHash"] = contractNeedPending[0].contractHash;
                await contractNeedModel.Update(session, filter, new ContractNeed() { type = EnumExecuteType.EXECUTE, contractHash = contractNeedPending[0].contractHash });
            }
            //爬取链上数据
            BlockWithTransactions blockWithTransactions =await web3Manager.Current.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(execBlockNumber));
            Transaction[] transactions = blockWithTransactions.Transactions;
            contractNeedModel.FindAllByContractHash();
            List<Task> actions = transactions.Select<Transaction,Task>(async (tran) =>
            {
                try
                {
                    TransactionReceipt transactionR = await web3Manager.Current.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(tran.TransactionHash);
                    if (transactionR.Logs.Count != 0)
                    {
                        var _logs = transactionR.Logs.Where(l =>
                        {
                            string contractHash = (string)l["address"];
                            if (contractNeedModel.ContractHash_ALL.ContainsKey(contractHash))
                            {
                                return true;
                            }
                            return false;
                        }).ToList();
                        if (_logs.Count != 0)
                            logs.Merge(_logs);
                    }
                    //Console.WriteLine(string.Format("foreach 高度{0}中", execBlockNumber));  
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("高度{0}获取交易receipt报错了", execBlockNumber));  
                    throw e;    
                }
            }).ToList();
            await Task.WhenAll(actions);
            Console.WriteLine("处理到了这里");
            ExecLogs(logs);
            await Commit(session);
        }

        public void ExecLogs(JArray ja)
        {
            if (ja.Count != 0)
            {
                var counter = logModel.counter;
                eventInfoModel.FindAllByEventHash();
                //筛选出需要入库的log
                var logs = ja.Select(l =>
                {
                    var log = Log.FromJObject(l);
                    if (eventInfoModel.EventHash_All.ContainsKey(log.eventHash))
                        log.ExtendFromEventInfo(eventInfoModel.EventHash_All[log.eventHash], counter++);
                    return log;
                }).ToList();
                logModel.Add(logs);
            }
        }


        private async Task Commit(IClientSessionHandle session)
        {
            await logModel.Commit(session);
            await contractNeedModel.Commit(session);
            await eventInfoModel.Commit(session);
        }

    }
}
