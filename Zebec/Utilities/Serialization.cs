using Solnet.Programs.Utilities;
using Zebec.Models;

namespace Zebec.Utilities
{
    public static class Serialization
    {
        public static void WriteWhiteLists(this byte[] data, IList<WhiteList> list, int startOffset, out int endOffset)
        {
            data.WriteU32((uint)list.Count, startOffset);
            startOffset += 32;
            for (int i = 0; i < list.Count; i++)
            {
                var whiteList = list.ElementAt(i);
                var temp = new byte[33];
                temp.WritePubKey(whiteList.Address, 0);
                temp.WriteU8(whiteList.Counter, 32);
                data.WriteSpan(temp, startOffset);
                startOffset += 33;
            }
            endOffset = startOffset;
        }
    }

}