using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ItzWarty;

namespace Dargon.VirtualFileMaps
{
   [Guid("35544913-46AE-4118-930D-6A4A89FD087C")]
   public class NullSector : ISector
   {
      private long size;
      public NullSector() { }
      public NullSector(long size) { this.size = size; }
      public long Size { get { return size; } }
      public IEnumerable<KeyValuePair<SectorRange, ISector>> Segment(IEnumerable<SectorRange> newPieces) { return newPieces.Select(range => range.PairValue((ISector)new NullSector(range.Size))); }
      public void Read(long readOffset, long readLength, byte[] buffer, long bufferOffset) { Array.Clear(buffer, (int)bufferOffset, (int)readLength); }
      public void Serialize(BinaryWriter writer) { writer.Write((long)size);}
      public void Deserialize(BinaryReader reader) { size = reader.ReadInt64(); }
   }
}
