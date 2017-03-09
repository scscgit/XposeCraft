using System.Collections.Generic;
using System.Linq;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Helpers
{
    class BuildingHelper : ActorHelper<IBuilding>
    {
        public static IList<TBuilding> GetBuildingsAsList<TBuilding>() where TBuilding : IBuilding
        {
            var list = new List<TBuilding>();
            ForEach<TBuilding>(building => { list.Add(building); }, Model.Instance.Buildings);
            return list;
        }

        public static Position ClosestEmptySpaceTo(IBuilding building)
        {
            // TODO: implement, demo
            return new Position();
        }

        public static TBuilding[] GetBuildings<TBuilding>() where TBuilding : IBuilding
        {
            return GetBuildingsAsList<TBuilding>().ToArray();
        }
    }
}
