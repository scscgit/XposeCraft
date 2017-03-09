using System;
using System.Collections.Generic;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors.Buildings
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
