using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Buildings;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Materials;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;

namespace XposeCraft_UI_API_Prototype_Test.GameInternal
{
	/// <summary>
	/// Data structures used within a game: collections etc.
	/// </summary>
	public class Model
	{
		public static Model Instance
		{
			get;
		} = new Model();

		public IList<IUnit> Units
		{
			get; set;
		}

		public IList<IBuilding> Buildings
		{
			get; set;
		}

		public IList<IMaterial> Materials
		{
			get; set;
		}
	}
}
