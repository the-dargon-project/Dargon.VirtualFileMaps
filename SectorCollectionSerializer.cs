using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ItzWarty;

namespace Dargon.VirtualFileMaps
{
   public class SectorCollectionSerializer
   {
      private readonly static Dictionary<Guid, Type> sectorTypesByGuid = new Dictionary<Guid, Type>();
      private const uint SECTOR_COLLECTION_MAGIC = 0x534D4656U; // VFMS

      static SectorCollectionSerializer()
      {
         var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ISector).IsAssignableFrom(p) && !p.IsInterface);
         foreach (var type in types) {
            sectorTypesByGuid.Add(Guid.Parse(type.GetCustomAttribute<GuidAttribute>().Value), type);
         }
      }

      public void Serialize(SectorCollection sectorCollection, BinaryWriter writer)
      {
         writer.Write((uint)SECTOR_COLLECTION_MAGIC);
         
         var sectors = sectorCollection.EnumerateSectorPairs().ToList();
         writer.Write((uint)sectors.Count);
         
         foreach (var sectorPair in sectors) {
            writer.Write(Guid.Parse(sectorPair.Value.GetType().GetCustomAttribute<GuidAttribute>().Value));
            writer.Write((long)sectorPair.Key.startInclusive);
            writer.Write((long)sectorPair.Key.endExclusive);
            sectorPair.Value.Serialize(writer);
         }
      }

      public SectorCollection Deserialize(BinaryReader reader)
      {
         var sectorCollectionMagic = reader.ReadUInt32();

         if (sectorCollectionMagic != SECTOR_COLLECTION_MAGIC) {
            throw new FormatException("Expected Sector Collection Magic " + SECTOR_COLLECTION_MAGIC + " but read " + sectorCollectionMagic);
         }

         var sectorCount = reader.ReadUInt32();
         var collection = new SectorCollection();
         for (var i = 0; i < sectorCount; i++) {
            var guid = reader.ReadGuid();
            var startInclusive = reader.ReadInt64();
            var endExclusive = reader.ReadInt64();
            var sector = (ISector)Activator.CreateInstance(sectorTypesByGuid[guid]);
            sector.Deserialize(reader);
            collection.AssignSector(new SectorRange(startInclusive, endExclusive), sector);
         }
         return collection;
      }
   }
}
