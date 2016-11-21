using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors.Units
{
	class Unit: Actor, IUnit
	{
		protected Unit(Position position):base(position)
		{
		}
	}
}
