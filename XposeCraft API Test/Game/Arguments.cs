using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Buildings;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;

namespace XposeCraft_UI_API_Prototype_Test.Game
{
	class Arguments
	{
		/// <summary>
		/// Before running the function, the instance to this current even will be returned here and can be used,
		/// for example, to unregister it after the run.
		/// </summary>
		public Event ThisEvent { get; set; }

		public IDictionary<string, string> StringMap { get; private set; } = new Dictionary<string, string>();

		/// <summary>
		/// Game resources
		/// </summary>

		public int Minerals { get; set; }

		/// <summary>
		/// Actors
		/// </summary>

		public IUnit MyUnit { get; set; }
		public IBuilding MyBuilding { get; set; }
		public IUnit EnemyUnit { get; set; }
		public IBuilding EnemyBuilding { get; set; }
	}
}
