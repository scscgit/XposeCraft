using UnityEngine;
using XposeCraft.Game.Control;
using XposeCraft.Game.Control.GameActions;

namespace XposeCraft.Game.Actors.Units
{
    public abstract class Unit : Actor, IUnit
    {
        public UnitActionQueue ActionQueue { get; private set; }

        public int Health
        {
            get { return -1; }
        }

        public int MaxHealth
        {
            get { return -1; }
        }

        protected Unit(GameObject gameObject) : base(gameObject)
        {
        }

        // TODO: deprecate in favor of equals operator?
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
