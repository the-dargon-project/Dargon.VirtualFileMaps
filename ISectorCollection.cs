using System.Collections.Generic;

namespace Dargon.VirtualFileMapping
{
   public interface ISectorCollection
   {
      void AssignSector(SectorRange range, ISector sector);
      void DeleteRange(long startInclusive, long endExclusive);
      void DeleteRange(SectorRange range);
      IReadOnlyList<ISector> EnumerateSectors();
      KeyValuePair<SectorRange, ISector>[] GetSectorsForRange(SectorRange range);
   }
}