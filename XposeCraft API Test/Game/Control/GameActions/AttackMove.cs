using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game.Control.GameActions
{
	/// <summary>
	/// Action of a movement to a new position,
	/// during which the unit attacks any enemies before it continues
	/// </summary>
	class AttackMove : GameAction
	{
		Position Where;

		public AttackMove(Position where)
		{
			Where = where;
		}
	}
}
