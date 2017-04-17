using System;
using System.Collections.Generic;
using UnityEngine;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Required;
using XposeCraft.Game.Actors.Resources.Minerals;

namespace XposeCraft.Game.Actors.Resources
{
    public abstract class Resource : Actor, IResource
    {
        /// <summary>
        /// Internal method, do not use.
        /// </summary>
        public void GatherByWorker(List<UnitController> builderUnits)
        {
            UnitSelection.SetTarget(builderUnits, GameObject, GameObject.transform.position);
        }

        public static T CreateResourceActor<T>(GameObject gameObject) where T : Resource
        {
            Type type;
            switch (gameObject.name)
            {
                case "BlueCrystal":
                    type = typeof(Mineral);
                    break;
                default:
                    throw new InvalidOperationException("Invalid Resource type of GameObject based on its name");
            }
            return Create<T>(type, gameObject, null);
        }
    }
}
