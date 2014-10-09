using System;
using System.Collections.Generic;
using System.Linq;
using ItzWarty;

namespace Dargon.VirtualFileMapping
{
   public class NullSector : ISector
   {
      private readonly long size;
      public NullSector(long size) { this.size = size; }
      public long Size { get { return size; } }
      public IEnumerable<KeyValuePair<SectorRange, ISector>> Segment(IEnumerable<SectorRange> newPieces) { return newPieces.Select(range => range.PairValue((ISector)new NullSector(range.Size))); }
      public void Read(long readOffset, long readLength, byte[] buffer, long bufferOffset) { Array.Clear(buffer, (int)bufferOffset, (int)readLength); }
   }
}
