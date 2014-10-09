using System.Collections.Generic;

namespace Dargon.VirtualFileMapping
{
   public interface ISector
   {
      long Size { get; }
      IEnumerable<KeyValuePair<SectorRange, ISector>> Segment(IEnumerable<SectorRange> newPieces);
      void Read(long readOffset, long readLength, byte[] buffer, long bufferOffset);
   }
}