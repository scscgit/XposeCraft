using System;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Control;
using XposeCraft.Game.Control.GameActions;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Actors.Units
{
    public abstract class Unit : Actor, IUnit
    {
        public UnitActionQueue ActionQueue { get; set; }

        public int Health
        {
            get { return UnitController.GetHealth(); }
        }

        public int MaxHealth
        {
            get { return UnitController.GetMaxHealth(); }
        }

        protected UnitController UnitController { get; private set; }

        protected override void Initialize(Player playerOwner)
        {
            base.Initialize(playerOwner);
            UnitController = GameObject.GetComponent<UnitController>();
            UnitController.UnitActor = this;
            UnitController.PlayerOwner = playerOwner;
            if (!GameObject.CompareTag("Unit"))
            {
                throw new InvalidOperationException("Unit Actor has invalid state, GameObject is missing tag");
            }
        }

        public UnitActionQueue Attack(IUnit unit)
        {
            return ActionQueue = new UnitActionQueue(new Attack(unit));
        }

        public UnitActionQueue Attack(IBuilding building)
        {
            return ActionQueue = new UnitActionQueue(new Attack(building));
        }

        public UnitActionQueue MoveTo(Position position)
        {
            return ActionQueue = new UnitActionQueue(new Move(position));
        }

        public UnitActionQueue AttackMoveTo(Position position)
        {
            return ActionQueue = new UnitActionQueue(new AttackMove(position));
        }
    }
}
