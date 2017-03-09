using XposeCraft.Game.Control;

namespace XposeCraft.Game.Actors.Units
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
