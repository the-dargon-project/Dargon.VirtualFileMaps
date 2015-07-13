using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Dargon.VirtualFileMaps;
using ItzWarty;
using NMockito;
using Xunit;

namespace Dargon.VirtualFileMapping
{
   public class NullSectorTest : NMockitoInstance
   {
      private readonly NullSector testObj;

      public NullSectorTest() {
         testObj = new NullSector(1000);
      }

      [Fact]
      public void SizeReflectsConstructorParameterTest()
      {
         AssertEquals(1000, new NullSector(1000).Size);
         AssertEquals(2000, new NullSector(2000).Size);
      }

      [Fact]
      public void SegmentTest()
      {
         var ranges = new[]{ new SectorRange(0, 250), new SectorRange(400, 450), new SectorRange(700, 1000) };
         var rangeSizes = ranges.Select(range => range.Size);
         var results = testObj.Segment(new SectorRange(0, 1000), ranges);
         AssertTrue(results.All(result => result.Value is NullSector));
         AssertTrue(results.Select(result => result.Key).SequenceEqual(ranges));
         AssertTrue(results.Select(result => result.Value.Size).SequenceEqual(rangeSizes));
      }

      [Fact]
      public void ReadZeroesBufferBytes()
      {
         var random = new Random(0);
         var size = 5000;
         var initial = Util.Generate(size, (i) => (byte)((unchecked((uint)random.Next()) % 0xFF) + 1));
         var clone = Util.Generate(size, i => initial[i]);

         var readOffset = 0;
         var readLength = 1000;
         var bufferOffset = 2000;

         new NullSector(readLength).Read(readOffset, readLength, clone, bufferOffset);

         AssertTrue(Util.ByteArraysEqual(initial.SubArray(0, bufferOffset), clone.SubArray(0, bufferOffset)));
         AssertTrue(clone.SubArray(bufferOffset, readLength).All((value) => value == 0));
         AssertTrue(Util.ByteArraysEqual(initial.SubArray(bufferOffset + readLength), clone.SubArray(bufferOffset + readLength)));
      }
   }
}
