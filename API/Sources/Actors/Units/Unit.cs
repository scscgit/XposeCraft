using System;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Control;

namespace XposeCraft.Game.Actors.Units
{
    public abstract class Unit : Actor, IUnit
    {
        public UnitActionQueue ActionQueue
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int Health
        {
            get { throw new NotImplementedException(); }
        }

        public int MaxHealth
        {
            get { throw new NotImplementedException(); }
        }

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
    }
}
