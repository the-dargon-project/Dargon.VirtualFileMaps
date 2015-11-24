using System;
using System.Linq;
using Dargon.VirtualFileMaps;
using NMockito;
using Xunit;

namespace Dargon.VirtualFileMapping
{
   public class SectorRangeTests : NMockitoInstance
   {
      [Fact]
      public void SizeTest()
      {
         AssertEquals(0, new SectorRange(0, 0).Size);
         AssertEquals(1, new SectorRange(0, 1).Size);
         AssertEquals(1, new SectorRange(1, 2).Size);
         AssertEquals(10, new SectorRange(1, 11).Size);
         AssertEquals(108, new SectorRange(12, 120).Size);
      }

      [Fact]
      public void CompareToTest()
      {
         AssertEquals(-1, new SectorRange(0, 10).CompareTo(new SectorRange(11, 20)));
         AssertEquals(1, new SectorRange(11, 20).CompareTo(new SectorRange(0, 10))); 
      }

      [Fact]
      public void IntersectsTest()
      {
         var a = new SectorRange(0, 10);
         var b = new SectorRange(0, 11);
         var c = new SectorRange(10, 20);
         var d = new SectorRange(19, 30);
         var e = new SectorRange(20, 30);
         AssertFalse(c.Intersects(a));
         AssertTrue(c.Intersects(b));
         AssertTrue(c.Intersects(c));
         AssertTrue(c.Intersects(d));
         AssertFalse(c.Intersects(e));
      }

      [Fact]
      public void FullyContainsTest()
      {
         var a = new SectorRange(0, 10);
         var b = new SectorRange(8, 12);
         var c = new SectorRange(10, 12);

         var d = new SectorRange(10, 20);

         var e = new SectorRange(14, 16);
         var f = new SectorRange(18, 20);
         var g = new SectorRange(18, 22);
         var h = new SectorRange(20, 30);

         AssertFalse(d.FullyContains(a));
         AssertFalse(d.FullyContains(b));
         AssertTrue(d.FullyContains(c));
         AssertTrue(d.FullyContains(d));
         AssertTrue(d.FullyContains(e));
         AssertTrue(d.FullyContains(f));
         AssertFalse(d.FullyContains(g));
         AssertFalse(d.FullyContains(h));
      }

      [Fact]
      public void ContainsTest()
      {
         var a = new SectorRange(10, 20);
         AssertFalse(a.Contains(9));
         AssertTrue(a.Contains(10));
         AssertTrue(a.Contains(15));
         AssertTrue(a.Contains(19));
         AssertFalse(a.Contains(20));
         AssertFalse(a.Contains(22));
      }

      [Fact]
      public void ChopFrontTest()
      {
         var initial = new SectorRange(1000, 2000);
         var chopResults1 = initial.Chop(new SectorRange(1000, 1500)).ToArray();
         var chopResults2 = initial.Chop(new SectorRange(500, 1500)).ToArray();

         AssertEquals(1, chopResults1.Length);
         AssertEquals(1, chopResults2.Length);

         AssertEquals(chopResults1[0], chopResults2[0]);
         AssertEquals(new SectorRange(1500, 2000), chopResults1[0]);
      }

      [Fact]
      public void ChopMiddleTest()
      {
         var initial = new SectorRange(1000, 2000);
         var chopResults = initial.Chop(new SectorRange(1250, 1750)).ToArray();

         AssertEquals(2, chopResults.Length);
         AssertEquals(new SectorRange(1000, 1250), chopResults[0]);
         AssertEquals(new SectorRange(1750, 2000), chopResults[1]);
      }

      [Fact]
      public void ChopBackTest()
      {
         var initial = new SectorRange(1000, 2000);
         var chopResults1 = initial.Chop(new SectorRange(1500, 2000)).ToArray();
         var chopResults2 = initial.Chop(new SectorRange(1500, 2500)).ToArray();

         AssertEquals(1, chopResults1.Length);
         AssertEquals(1, chopResults2.Length);

         AssertEquals(chopResults1[0], chopResults2[0]);
         AssertEquals(new SectorRange(1000, 1500), chopResults1[0]);
      }
   }
}
