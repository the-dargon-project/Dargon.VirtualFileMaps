using System;
using System.Collections.Generic;
using System.Linq;
using ItzWarty;

namespace Dargon.VirtualFileMapping
{
   public class SectorCollection : ISectorCollection
   {
      private readonly SortedList<SectorRange, ISector> sectors;

      public SectorCollection() : this(Enumerable.Empty<KeyValuePair<SectorRange, ISector>>()) { }

      public SectorCollection(IEnumerable<KeyValuePair<SectorRange, ISector>> rangeAndSectors)
      {
         this.sectors = rangeAndSectors.Aggregate(new SortedList<SectorRange, ISector>(), (list, kvp) => list.Add(kvp.Key, kvp.Value));
      }

      public void AssignSector(SectorRange range, ISector sector) {
         DeleteRange(range);
         sectors.Add(range, sector);
      }

      public void DeleteRange(long startInclusive, long endExclusive) { DeleteRange(new SectorRange(startInclusive, endExclusive)); }

      public void DeleteRange(SectorRange range)
      {
         var kvpsToTouch = GetSectorsForRange(range);
         foreach (var kvp in kvpsToTouch) {
            sectors.Remove(kvp.Key);
            if (!range.FullyContains(kvp.Key)) {
               var newPieces = kvp.Key.Chop(range);
               var newRangeAndSectors = kvp.Value.Segment(newPieces);
               foreach (var rangeAndSector in newRangeAndSectors) {
                  sectors.Add(rangeAndSector.Key, rangeAndSector.Value);
               }
            }
         }
      }

      public IReadOnlyList<ISector> EnumerateSectors() { return new List<ISector>(sectors.Values); }

      public KeyValuePair<SectorRange, ISector>[] GetSectorsForRange(SectorRange range)
      {
         return sectors.Where(kvp => range.Intersects(kvp.Key)).ToArray();
      }
   }
}