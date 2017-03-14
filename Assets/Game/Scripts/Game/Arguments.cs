using System;
using System.Collections.Generic;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Game
{
    [Serializable]
    public class Arguments
    {
        /// <summary>
        /// Before running the function, the instance to this current even will be returned here and can be used,
        /// for example, to unregister it after the run.
        /// </summary>
        public Event ThisEvent { get; set; }

        public IDictionary<string, string> StringMap { get; private set; }

        // Game resources

        public int Minerals { get; set; }

        // Actors

        public IUnit MyUnit { get; set; }
        public IBuilding MyBuilding { get; set; }
        public IUnit[] EnemyUnits { get; set; }
        public IBuilding[] EnemyBuildings { get; set; }

        public Arguments()
        {
            StringMap = new Dictionary<string, string>();
        }
    }
}
