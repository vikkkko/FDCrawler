using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrawlerForEth.IO
{
    interface ICache<T> : ICollection<T>
    {
        void Add(IList<T> values);
    }
}
