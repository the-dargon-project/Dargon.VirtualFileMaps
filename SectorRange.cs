using System;
using System.Collections.Generic;

namespace Dargon.VirtualFileMaps
{
   public struct SectorRange : IComparable, IComparable<SectorRange>, IEquatable<SectorRange>
   {
      public long startInclusive;
      public long endExclusive;

      public SectorRange(long startInclusive, long endExclusive)
      {
         this.startInclusive = startInclusive;
         this.endExclusive = endExclusive;
      }

      public long Size { get { return endExclusive - startInclusive; } }

      public int CompareTo(SectorRange other) { return startInclusive.CompareTo(other.startInclusive); }
      public int CompareTo(object obj) { return this.CompareTo((SectorRange)obj); }

      public bool Intersects(SectorRange range) { return !((startInclusive >= range.endExclusive) || (range.startInclusive >= endExclusive)); }
      public bool FullyContains(SectorRange range) { return startInclusive <= range.startInclusive && range.endExclusive <= this.endExclusive; }
      public bool Contains(long x) { return startInclusive <= x && x < endExclusive; }

      public IEnumerable<SectorRange> Chop(SectorRange cutter)
      {
         if (this.FullyContains(cutter) && this.startInclusive < cutter.startInclusive && cutter.endExclusive < this.endExclusive) {
            yield return new SectorRange(this.startInclusive, cutter.startInclusive - 1);
            yield return new SectorRange(cutter.endExclusive, this.endExclusive);
         } else if (cutter.Contains(this.startInclusive)) {
            yield return new SectorRange(cutter.endExclusive, this.endExclusive);
         } else {
            yield return new SectorRange(this.startInclusive, cutter.startInclusive - 1);
         }
      }

      public bool Equals(SectorRange other) { return this.startInclusive == other.startInclusive && this.endExclusive == other.endExclusive; }
      public override bool Equals(object obj) { return obj is SectorRange && this.Equals((SectorRange)obj); }

      public override string ToString() { return "[SectorRange [" + startInclusive + ", " + endExclusive + ")]"; }
   }
}