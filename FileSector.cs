using ItzWarty;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Dargon.VirtualFileMapping
{
   [Guid("5DB2B4C2-39AE-4629-988A-CFFFCE89F230")]
   public class FileSector : ISector
   {
      private string path;
      private long offset;
      private long length;

      public FileSector(string path, long offset, long length)
      {
         this.path = path;
         this.offset = offset;
         this.length = length;
      }

      public long Size { get { return length; } }

      public IEnumerable<KeyValuePair<SectorRange, ISector>> Segment(IEnumerable<SectorRange> newPieces) {
         foreach (var piece in newPieces) {
            yield return piece.PairValue((ISector)new FileSector(path, offset + piece.startInclusive, piece.endExclusive - piece.startInclusive));
         }
      }

      public void Read(long readOffset, long readLength, byte[] buffer, long bufferOffset)
      {
         using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
            fs.Seek(offset + readOffset, SeekOrigin.Begin);
            var bytesRemaining = readLength;
            while (bytesRemaining > 0) {
               bytesRemaining -= fs.Read(buffer, (int)bufferOffset, (int)bytesRemaining);
            }
         }
      }

      public void Serialize(BinaryWriter writer)
      {
         writer.WriteNullTerminatedString(path);
         writer.Write((long)offset);
         writer.Write((long)length);
      }

      public void Deserialize(BinaryReader reader)
      {
         path = reader.ReadNullTerminatedString();
         offset = reader.ReadInt64();
         length = reader.ReadInt64();
      }
   }
}
