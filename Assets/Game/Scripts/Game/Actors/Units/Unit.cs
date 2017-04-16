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

        protected override void Initialize()
        {
            base.Initialize();
            UnitController = GameObject.GetComponent<UnitController>();
            UnitController.UnitActor = this;
            // TODO: make sure the specification will never require asynchronous or other random Actor initialization
            UnitController.PlayerOwner = Player.CurrentPlayer;
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
