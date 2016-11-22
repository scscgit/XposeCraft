using System;
using System.Collections.Generic;
using System.Linq;
using XposeCraft.Game.Actors;
using XposeCraft.Game.Actors.Materials;
using XposeCraft.Game.Actors.Materials.Minerals;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Helpers
{
	class MaterialHelper : ActorHelper<IMaterial>
	{
		public static IList<MaterialType> GetMaterialsAsList<MaterialType>() where MaterialType : IMaterial
		{
			var list = new List<MaterialType>();
			ForEach<MaterialType>(material =>
			{
				list.Add(material);
			}, from: Model.Instance.Materials);
			return list;
		}

		public static MaterialType[] GetMaterials<MaterialType>() where MaterialType : IMaterial
		{
			return GetMaterialsAsList<MaterialType>().ToArray();
		}

		public static Mineral GetNearestMineralsTo(IActor actor)
		{
			Mineral closestMineral = null;
			ForEach<Mineral>(mineral =>
			{
				if (closestMineral == null)
				{
					closestMineral = mineral;
				}
				else if (mineral.Position < closestMineral.Position)
				{
					closestMineral = mineral;
				}
			}, Model.Instance.Materials);
			return closestMineral;
		}
	}
}
