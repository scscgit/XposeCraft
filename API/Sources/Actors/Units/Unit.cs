using System;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Control;

namespace XposeCraft.Game.Actors.Units
{
    /// <inheritdoc cref="IUnit"/>
    public abstract class Unit : Actor, IUnit
    {
        public UnitActionQueue ActionQueue { get; set; }

        public int Health { get; }

        public int MaxHealth { get; }

        public UnitActionQueue Attack(IUnit unit)
        {
            throw new NotImplementedException();
        }

        public UnitActionQueue Attack(IBuilding building)
        {
            throw new NotImplementedException();
        }

        public UnitActionQueue MoveTo(Position position)
        {
            throw new NotImplementedException();
        }

        public UnitActionQueue AttackMoveTo(Position position)
        {
            throw new NotImplementedException();
        }

        public bool Dead { get; }
    }
}
