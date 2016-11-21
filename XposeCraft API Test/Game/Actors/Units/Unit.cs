using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Control;
using XposeCraft_UI_API_Prototype_Test.Game.Control.GameActions;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors.Units
{
	public abstract class Unit : Actor, IUnit
	{
		UnitActionQueue ActionQueue;

		protected Unit(Position position) : base(position)
		{
		}

		public UnitActionQueue ReplaceActionQueue(UnitActionQueue queue)
		{
			return ActionQueue = queue;
		}

		public UnitActionQueue Attack(IUnit unit)
		{
			return ReplaceActionQueue(new UnitActionQueue(new Attack(unit)));
		}

		public UnitActionQueue MoveTo(Position position)
		{
			return ReplaceActionQueue(new UnitActionQueue(new Move(position)));
		}

		public UnitActionQueue AttackMoveTo(Position position)
		{
			return ReplaceActionQueue(new UnitActionQueue(new AttackMove(position)));
		}
	}
}
