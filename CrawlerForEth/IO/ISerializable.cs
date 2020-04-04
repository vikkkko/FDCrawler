using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CrawlerForEth.IO
{
    public interface ISerializable
    {
        void Srialize(BinaryWriter writer);
        byte[] Serialize();
        void Deserialize(BinaryReader reader);
    }
}
