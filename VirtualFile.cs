using System;
using System.Text;

namespace Dargon.VirtualFileMapping
{
   public class VirtualFile
   {
      private readonly ISectorCollection sectors;

      public VirtualFile() : this(new SectorCollection()) { }
      public VirtualFile(ISectorCollection sectors) { this.sectors = sectors; }

      public byte[] Read(long offset, long length)
      {
         var buffer = new byte[length];
         Read(offset, length, buffer, 0);
         return buffer;
      }

      public void Read(long offset, long length, byte[] buffer, long bufferOffset = 0)
      {
         if (buffer.Length < length + bufferOffset) {
            throw new ArgumentException("The buffer size is not large enough.");
         }

         long bytesRemaining = length;
         var sectorsToRead = sectors.GetSectorsForRange(new SectorRange(offset, offset + length));
         for (var i = 0; i < sectorsToRead.Length; i++) {
            var sectorKvp = sectorsToRead[i];
            var sectorRange = sectorKvp.Key;
            var sector = sectorKvp.Value;

            long readOffset = i == 0 ? offset - sectorRange.startInclusive : 0;
            long readLength = i == sectorsToRead.Length - 1 ? bytesRemaining : sector.Size - readOffset;

            sector.Read(readOffset, readLength, buffer, bufferOffset);

            bufferOffset += readLength;
            bytesRemaining -= readLength;
         }
      }
   }
}