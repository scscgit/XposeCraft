using System.Collections.Generic;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Materials;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.GameInternal
{
    /// <summary>
    /// Data structures used within a game: collections etc.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Instance of the game Model
        /// </summary>
        private static Model _instance;

        public static Model Instance
        {
            get { return _instance ?? (_instance = new Model()); }
        }

        public Model()
        {
            Units = new List<IUnit>();
            Buildings = new List<IBuilding>();
            Materials = new List<IMaterial>();
            Minerals = 80;
        }

        /// <summary>
        /// In-game Actors
        /// </summary>

        public IList<IUnit> Units { get; set; }

        public IList<IBuilding> Buildings { get; set; }

        public IList<IMaterial> Materials { get; set; }

        /// <summary>
        /// Currencies of the player
        /// </summary>

        public int Minerals { get; set; }
    }
}
