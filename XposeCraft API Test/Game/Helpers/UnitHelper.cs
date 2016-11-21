using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;
using XposeCraft_UI_API_Prototype_Test.Game.Enums;
using XposeCraft_UI_API_Prototype_Test.GameInternal;

namespace XposeCraft_UI_API_Prototype_Test.Game.Helpers
{
	/// <summary>
	/// Provide easy access for operations, that may be complicated or impossible by using direct API of other classess.
	/// TODO: decide what hierarchy should they have.
	/// </summary>
	class UnitHelper : ActorHelper<IUnit>
	{
		public static IList<UnitType> GetUnitsAsList<UnitType>() where UnitType : IUnit
		{
			var list = new List<UnitType>();
			ForEach<UnitType>(unit =>
			{
				list.Add(unit);
			}, from: Model.Instance.Units);
			return list;
		}

		// TODO: GetUnits() overload that implies UnitType = IUnit, but it causes error when I simply overload it
		public static UnitType[] GetUnits<UnitType>() where UnitType : IUnit
		{
			return GetUnitsAsList<UnitType>().ToArray();
		}
	}
}
