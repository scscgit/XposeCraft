using System;
using UnityEngine;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.GameInternal;

namespace XposeCraft.Core.Resources
{
    public class ResourceDropOff : MonoBehaviour
    {
        public bool[] type = new bool[0];

        void Start()
        {
            var typeList = new ResourceManager.BoolList();
            typeList.AddRange(type);
            var buildingController = gameObject.GetComponent<BuildingController>();
            if (buildingController == null)
            {
                throw new Exception(typeof(ResourceDropOff) + " added to a non-building Game Object");
            }
            GameManager.Instance.ResourceManagerFaction[buildingController.FactionIndex]
                .AddDropOff(gameObject, typeList);
        }
    }
}
