using System;
using System.Collections.Generic;
using System.Linq;
using Dargon.VirtualFileMaps;
using ItzWarty;
using NMockito;
using Xunit;

namespace Dargon.VirtualFileMapping
{
   public class VirtualFileTest : NMockitoInstance
   {
      private VirtualFile testObj;

      [Mock] private readonly ISectorCollection sectors = null;

      public VirtualFileTest() {
         testObj = new VirtualFile(sectors);
      }

      [Fact]
      public void ReadNothingFromEmptyFileTest()
      {
         var readRange = new SectorRange(0, 0);
         When(sectors.GetSectorsForRange(readRange)).ThenReturn(new KeyValuePair<SectorRange, ISector>[0]);
         var output = testObj.Read(0, 0);
         Verify(sectors).GetSectorsForRange(readRange);
         VerifyNoMoreInteractions();
         AssertEquals(0, output.Length);
      }

      [Fact]
      public void ReadWithinSectorTest()
      {
         var readRange = new SectorRange(250, 750);
         var sectorRange = new SectorRange(0, 1000);

         var sector = CreateMock<ISector>();

         When(sector.Size).ThenReturn(1000);
         When(sectors.GetSectorsForRange(readRange)).ThenReturn(new[] { sectorRange.PairValue(sector) });

         var buffer = testObj.Read(readRange.startInclusive, readRange.Size);

         Verify(sectors).GetSectorsForRange(readRange);
         Verify(sector).Read(250, 500, buffer, 0);
         Verify(sector, AnyOrNoneTimes()).Size.Wrap();
         VerifyNoMoreInteractions();
      }

      [Fact]
      public void ReadAcrossSectorsTest()
      {
         var readRange = new SectorRange(500, 1500);
         var firstRange = new SectorRange(100, 800);
         var secondRange = new SectorRange(900, 1100);
         var thirdRange = new SectorRange(1200, 2000);

         var firstSector = CreateMock<ISector>();
         var secondSector = CreateMock<ISector>();
         var thirdSector = CreateMock<ISector>();

         When(firstSector.Size).ThenReturn(700);
         When(secondSector.Size).ThenReturn(200);
         When(thirdSector.Size).ThenReturn(800);
         When(sectors.GetSectorsForRange(readRange)).ThenReturn(new[] { firstRange.PairValue(firstSector), secondRange.PairValue(secondSector), thirdRange.PairValue(thirdSector) });

         var buffer = testObj.Read(readRange.startInclusive, readRange.Size);

         Verify(sectors).GetSectorsForRange(readRange);
         Verify(firstSector).Read(400, 300, buffer, 0);
         Verify(secondSector).Read(0, 200, buffer, 400);
         Verify(thirdSector).Read(0, 300, buffer, 700);
         Verify(firstSector).Size.Wrap();
         Verify(secondSector).Size.Wrap();
         Verify(thirdSector, AnyOrNoneTimes()).Size.Wrap();
         VerifyNoMoreInteractions();
      }

      [Fact]
      public void ReadThrowsOnInadequateBuffer() 
      { 
         long offset = 10;
         long length = 100;
         long bufferOffset = 50;
         var buffer = new byte[bufferOffset + length - 1];

         AssertThrows<ArgumentException>(() => testObj.Read(offset, length, buffer, bufferOffset));
      }
   }
}
