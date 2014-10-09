using System.Collections.Generic;
using System.Linq;
using ItzWarty;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMockito;

namespace Dargon.VirtualFileMapping
{
   [TestClass]
   public class VirtualFileTest : NMockitoInstance
   {
      private VirtualFile testObj;

      [Mock] private readonly ISectorCollection sectors = null;

      [TestInitialize]
      public void Setup()
      {
         InitializeMocks();

         testObj = new VirtualFile(sectors);
      }

      [TestMethod]
      public void ReadNothingFromEmptyFileTest()
      {
         var readRange = new SectorRange(0, 0);
         When(sectors.GetSectorsForRange(readRange)).ThenReturn(new KeyValuePair<SectorRange, ISector>[0]);
         var output = testObj.Read(0, 0);
         Verify(sectors).GetSectorsForRange(readRange);
         VerifyNoMoreInteractions();
         AssertEquals(0, output.Length);
      }

      [TestMethod]
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

      [TestMethod]
      public void ReadAcrossSectorsTest()
      {
         var readRange = new SectorRange(500, 1500);
         var firstRange = new SectorRange(0, 1000);
         var secondRange = new SectorRange(1000, 2000);

         var firstSector = CreateMock<ISector>();
         var secondSector = CreateMock<ISector>();

         When(firstSector.Size).ThenReturn(1000);
         When(secondSector.Size).ThenReturn(1000);
         When(sectors.GetSectorsForRange(readRange)).ThenReturn(new[] { firstRange.PairValue(firstSector), secondRange.PairValue(secondSector) });

         var buffer = testObj.Read(readRange.startInclusive, readRange.Size);

         Verify(sectors).GetSectorsForRange(readRange);
         Verify(firstSector).Read(500, 500, buffer, 0);
         Verify(secondSector).Read(0, 500, buffer, 500);
         Verify(firstSector).Size.Wrap();
         Verify(secondSector, AnyOrNoneTimes()).Size.Wrap();
         VerifyNoMoreInteractions();
      }
   }
}
