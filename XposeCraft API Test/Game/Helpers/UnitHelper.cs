using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;
using XposeCraft_UI_API_Prototype_Test.GameInternal;

namespace XposeCraft_UI_API_Prototype_Test.Game.Helpers
{
	/// <summary>
	/// Provide easy access for operations, that may be complicated or impossible by using direct API of other classess.
	/// TODO: decide what hierarchy should they have.
	/// </summary>
	class UnitHelper : ActorHelper<IActor>
	{
		public static IList<UnitType> GetUnitsAsList<UnitType>() where UnitType : IUnit
		{
			var list = new List<UnitType>();
			ForEach(unit =>
			{
				if (unit is UnitType)
				{
					list.Add((UnitType)unit);
				}
			}, from: Model.Instance.Units);
			return list;
		}

		public static UnitType[] GetUnits<UnitType>() where UnitType : IUnit
		{
			return GetUnitsAsList<UnitType>().ToArray();
		}
	}
}
