using System;
using System.Collections.Generic;
using System.Linq;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Helpers
{
	class BuildingHelper : ActorHelper<IBuilding>
	{
		public static IList<BuildingType> GetBuildingsAsList<BuildingType>() where BuildingType : IBuilding
		{
			var list = new List<BuildingType>();
			ForEach<BuildingType>(building =>
			{
				list.Add(building);
			}, from: Model.Instance.Buildings);
			return list;
		}

		public static Position ClosestEmptySpaceTo(IBuilding building)
		{
			// TODO: implement, demo
			return new Position();
		}

		public static BuildingType[] GetBuildings<BuildingType>() where BuildingType : IBuilding
		{
			return GetBuildingsAsList<BuildingType>().ToArray();
		}
	}
}
