using System;
using System.Diagnostics;
using System.Linq;
using Dargon.VirtualFileMaps;
using NMockito;
using Xunit;

namespace Dargon.VirtualFileMapping {
   public class FileSectorTests : NMockitoInstance {
      [Fact]
      public void SegmentTest() {
         const string kFilePath = "FILE_PATH";
         var sector = new FileSector(kFilePath, 100, 900);
         AssertEquals(kFilePath, sector.Path);

         var newSectorKvps = sector.Segment(new SectorRange(300, 1200), new[] {
            new SectorRange(300, 500),
            new SectorRange(800, 1200),
         }).ToArray();

         AssertEquals(2, newSectorKvps.Length);

         var firstSector = (FileSector)newSectorKvps[0].Value;
         var secondSector = (FileSector)newSectorKvps[1].Value;

         AssertEquals(new SectorRange(300, 500), newSectorKvps[0].Key);
         AssertEquals(kFilePath, firstSector.Path);
         AssertEquals(200, firstSector.Size);
         AssertEquals(100, firstSector.Offset);

         AssertEquals(new SectorRange(800, 1200), newSectorKvps[1].Key);
         AssertEquals(kFilePath, secondSector.Path);
         AssertEquals(400, secondSector.Size);
         AssertEquals(600, secondSector.Offset);
      }
   }
}
