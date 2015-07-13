using System.Collections.Generic;
using System.IO;

namespace Dargon.VirtualFileMaps
{
   public interface ISector
   {
      long Size { get; }
      IEnumerable<KeyValuePair<SectorRange, ISector>> Segment(SectorRange currentRange, IEnumerable<SectorRange> newRanges);
      void Read(long readOffset, long readLength, byte[] buffer, long bufferOffset);
      void Serialize(BinaryWriter writer);
      void Deserialize(BinaryReader reader);
   }
}