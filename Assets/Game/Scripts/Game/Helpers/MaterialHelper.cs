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
        public static IList<TMaterial> GetMaterialsAsList<TMaterial>() where TMaterial : IMaterial
        {
            var list = new List<TMaterial>();
            ForEach<TMaterial>(material => { list.Add(material); }, Model.Instance.Materials);
            return list;
        }

        public static TMaterial[] GetMaterials<TMaterial>() where TMaterial : IMaterial
        {
            return GetMaterialsAsList<TMaterial>().ToArray();
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
