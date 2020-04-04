using Nethereum.Web3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CrawlerForEth
{
    public class Web3Manager
    {
        private int position = 0;

        private List<Web3> webs;

        public Web3Manager(IEnumerable<string> urls)
        {
            if (urls is IList<string>)
                webs = new List<Web3>((urls as IList<string>).Count);
            else if (urls is Array)
                webs = new List<Web3>((urls as Array).Length);
            else
                throw new ArgumentException();
            foreach (var url in urls)
            {
                webs.Add(new Web3(url));
            }
        }

        public Web3 Current => webs[position];

        public void ChangeWeb3()
        {
            position = position++ < webs.Count ? position++ : 0;
        }
    }
}
