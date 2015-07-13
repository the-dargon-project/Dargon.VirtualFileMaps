using System.Collections.Generic;
using System.Linq;
using Dargon.VirtualFileMaps;
using ItzWarty;
using NMockito;
using Xunit;

namespace Dargon.VirtualFileMapping
{
   public class SectorCollectionTests : NMockitoInstance
   {
      [Fact]
      public void AssignSectorTest() 
      { 
         var range = new SectorRange(1000, 2000);
         var sector = CreateMock<ISector>();
         var collection = new SectorCollection();
         collection.AssignSector(range, sector);

         var sectors = collection.EnumerateSectors().ToList();

         AssertEquals(1, sectors.Count);
         AssertEquals(sector, sectors[0]);
         VerifyNoMoreInteractions();
      }

      [Fact]
      public void DeleteRangeTest()
      {
         var initialRange = new SectorRange(0, 1000);
         var initialSector = CreateMock<ISector>();
         var chopRange = new SectorRange(250, 750);
         var leftSector = CreateMock<ISector>();
         var leftRange = new SectorRange(0, 250);
         var rightSector = CreateMock<ISector>();
         var rightRange = new SectorRange(750, 1000);
         var leftAndRightRange = new[] { leftRange, rightRange };
         var leftAndRightSectors = new[] { leftSector, rightSector };
         var leftAndRightRangeAndSectors = new[] { leftRange.PairValue(leftSector), rightRange.PairValue(rightSector) };
         
         When(initialSector.Segment(Eq(initialRange), EqSequence(leftAndRightRange))).ThenReturn(leftAndRightRangeAndSectors);

         var collection = new SectorCollection(new KeyValuePair<SectorRange, ISector>(initialRange, initialSector).Wrap());
         collection.DeleteRange(chopRange);

         Verify(initialSector).Segment(Eq(initialRange), Any<IEnumerable<SectorRange>>(x => x.SequenceEqual(leftAndRightRange)));
         VerifyNoMoreInteractions();

         AssertTrue(leftAndRightSectors.SequenceEqual(collection.EnumerateSectors()));
      }
   }
}
