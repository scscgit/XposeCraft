using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Materials;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Materials.Minerals;
using XposeCraft_UI_API_Prototype_Test.GameInternal;

namespace XposeCraft_UI_API_Prototype_Test.Game.Helpers
{
	class MaterialHelper : ActorHelper<IMaterial>
	{
		public static IList<MaterialType> GetMaterialsAsList<MaterialType>() where MaterialType : IMaterial
		{
			var list = new List<MaterialType>();
			ForEach(unit =>
			{
				if (unit is MaterialType)
				{
					list.Add((MaterialType)unit);
				}
			}, from: Model.Instance.Materials);
			return list;
		}

		public static MaterialType[] GetMaterials<MaterialType>() where MaterialType : IMaterial
		{
			return GetMaterialsAsList<MaterialType>().ToArray();
		}

		public Mineral GetNearestMineralsTo(IActor actor)
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
