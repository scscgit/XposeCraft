using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Buildings;
using XposeCraft_UI_API_Prototype_Test.GameInternal;

namespace XposeCraft_UI_API_Prototype_Test.Game.Helpers
{
	class BuildingHelper : ActorHelper<IBuilding>
	{
		public static IList<BuildingType> GetBuildingsAsList<BuildingType>() where BuildingType : IBuilding
		{
			var list = new List<BuildingType>();
			ForEach(unit =>
			{
				if (unit is BuildingType)
				{
					list.Add((BuildingType)unit);
				}
			}, from: Model.Instance.Buildings);
			return list;
		}

		public static BuildingType[] GetBuildings<BuildingType>() where BuildingType : IBuilding
		{
			return GetBuildingsAsList<BuildingType>().ToArray();
		}
	}
}
