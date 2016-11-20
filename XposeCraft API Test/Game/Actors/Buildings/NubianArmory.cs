using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Enums;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors.Buildings
{
	class NubianArmory : Building
	{
		public NubianArmory(Position position) : base(position)
		{
		}

		public bool CreateUnit(UnitType type)
		{

			// TODO: add to the queue, event when created, public accessors to the current state
			return true;
		}
	}
}
