using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors;
using XposeCraft_UI_API_Prototype_Test.GameInternal;

namespace XposeCraft_UI_API_Prototype_Test.Game.Helpers
{
	/// <summary>
	/// Provide easy access for operations, that may be complicated or impossible by using direct API of other classess.
	/// TODO: decide what hierarchy should they have.
	/// </summary>
	public class UnitHelper
	{
		public delegate void ForEachAction(Unit unit);

		private static readonly object ForEachLock = new object();
		protected static void ForEach(ForEachAction action)
		{
			lock (ForEachLock)
			{
				foreach (Unit unit in Model.Instance.Units)
				{
					action(unit);
				}
			}
		}

		public static IList<UnitType> GetUnitsList<UnitType>() where UnitType : Unit
		{
			var list = new List<UnitType>();
			ForEach(unit =>
			{
				var unitOfType = unit as UnitType;
				if (unitOfType != null)
				{
					list.Add(unitOfType);
				}
			});
			return list;
		}

		public static UnitType[] GetUnits<UnitType>() where UnitType : Unit
		{
			return GetUnitsList<UnitType>().ToArray();
		}
	}
}
