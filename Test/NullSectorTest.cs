using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ItzWarty;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMockito;

namespace Dargon.VirtualFileMapping
{
   [TestClass]
   public class NullSectorTest : NMockitoInstance
   {
      private NullSector testObj;

      [TestInitialize]
      public void Setup()
      {
         InitializeMocks();

         testObj = new NullSector(1000);
      }

      [TestMethod]
      public void SizeReflectsConstructorParameterTest()
      {
         AssertEquals(1000, new NullSector(1000).Size);
         AssertEquals(2000, new NullSector(2000).Size);
      }

      [TestMethod]
      public void SegmentTest()
      {
         var ranges = new[]{ new SectorRange(0, 250), new SectorRange(600, 1000) };
         var rangeSizes = ranges.Select(range => range.Size);
         var results = testObj.Segment(ranges);
         AssertTrue(results.All(result => result.Value is NullSector));
         AssertTrue(results.Select(result => result.Key).SequenceEqual(ranges));
         AssertTrue(results.Select(result => result.Value.Size).SequenceEqual(rangeSizes));
      }

      [TestMethod]
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
