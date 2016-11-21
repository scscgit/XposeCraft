using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game.Control.GameActions
{
	/// <summary>
	/// Action of a movement to a new position
	/// </summary>
	class Move : GameAction
	{
		Position Where;

		public Move(Position where)
		{
			Where = where;
		}
	}
}
