using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Control;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors.Units
{
	public interface IUnit : IActor
	{
		int Health { get; }
		int MaxHealth { get; }

		UnitActionQueue ActionQueue { get; }
		UnitActionQueue ReplaceActionQueue(UnitActionQueue queue);

		UnitActionQueue Attack(IUnit unit);
		UnitActionQueue MoveTo(Position position);
		UnitActionQueue AttackMoveTo(Position position);
	}
}
