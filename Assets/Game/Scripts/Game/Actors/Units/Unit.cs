using System;
using System.Collections.Generic;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Required;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Control;
using XposeCraft.Game.Control.GameActions;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Actors.Units
{
    public abstract class Unit : Actor, IUnit
    {
        private UnitActionQueue _actionQueue;

        public UnitActionQueue ActionQueue
        {
            get { return _actionQueue; }
            set
            {
                // Cancels the previous pending action by finishing it
                if (UnitController._actionDequeue != null)
                {
                    UnitController._actionDequeue.Finished();
                }
                // Initializes the callback mechanism in the controller
                // TODO: clone before creating a representing dequeue
                _actionQueue = value;
                UnitController._actionDequeue =
                    new UnitActionQueue.ActionDequeue(this, UnitController, _actionQueue);
                UnitController._actionDequeue.Dequeue();
            }
        }

        /// <summary>
        /// Internal method, do not use.
        /// </summary>
        internal void AttackedByUnit(UnitController attackerUnit)
        {
            if (GameManager.Instance.Factions[UnitController.FactionIndex]
                    .Relations[attackerUnit.FactionIndex]
                    .state != 2)
            {
                throw new Exception("The target Unit is not enemy, so it cannot be attacked");
            }
            UnitSelection.SetTarget(new List<UnitController> {attackerUnit}, GameObject, GameObject.transform.position);
        }

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
