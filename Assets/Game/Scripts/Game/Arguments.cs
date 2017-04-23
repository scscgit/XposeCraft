using System;
using System.Collections.Generic;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Game
{
    /// <summary>
    /// Description of a Game Event occurrence.
    /// </summary>
    [Serializable]
    public class Arguments
    {
        /// <summary>
        /// Before running the function, the instance to this current even will be returned here and can be used,
        /// for example, to unregister it after the run.
        /// </summary>
        public Event ThisEvent { get; set; }

        // TODO: replace
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

        /// <summary>
        /// Clone constructor.
        /// TODO: do a deep clone.
        /// </summary>
        /// <param name="arguments">Arguments to have its parameters copied to the new instance.</param>
        /// <param name="thisEvent">Event represented by the Arguments instance.</param>
        public Arguments(Arguments arguments, Event thisEvent)
        {
            ThisEvent = thisEvent;
            // TODO: clone
            StringMap = arguments.StringMap;
            Minerals = arguments.Minerals;
            MyUnit = arguments.MyUnit;
            MyBuilding = arguments.MyBuilding;
            // TODO: clone
            EnemyUnits = arguments.EnemyUnits;
            // TODO: clone
            EnemyBuildings = arguments.EnemyBuildings;
        }
    }
}
