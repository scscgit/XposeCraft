using System;
using System.Collections.Generic;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Required;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Control;
using XposeCraft.Game.Control.GameActions;
using XposeCraft.Game.Enums;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Actors.Units
{
    /// <inheritdoc cref="IUnit"/>
    public abstract class Unit : Actor, IUnit
    {
        private UnitActionQueue _actionQueue;

        public UnitActionQueue ActionQueue
        {
            get
            {
                if (_actionQueue != null)
                {
                    return _actionQueue;
                }
                _actionQueue = new UnitActionQueue();
                UnitController._actionDequeue = new UnitActionQueue.ActionDequeue(this, UnitController, _actionQueue);
                _actionQueue.Dequeue = UnitController._actionDequeue;
                UnitController._actionDequeue.Dequeue();
                return _actionQueue;
            }
            set
            {
                if (Dead)
                {
                    throw new UnitDeadException();
                }
                // Cancels the previous pending action by finishing it
                if (UnitController._actionDequeue != null)
                {
                    UnitController._actionDequeue.Finish();
                }
                // Initializes the callback mechanism in the controller
                // TODO: clone before creating a representing dequeue
                _actionQueue = value;
                UnitController._actionDequeue = new UnitActionQueue.ActionDequeue(this, UnitController, _actionQueue);
                _actionQueue.Dequeue = UnitController._actionDequeue;
                UnitController._actionDequeue.Dequeue();
            }
        }

        /// <summary>
        /// Internal method, do not use.
        /// </summary>
        internal bool AttackedByUnit(UnitController attackerUnit)
        {
            if (Dead)
            {
                return true;
            }
            if (GameManager.Instance.Factions[UnitController.FactionIndex]
                    .Relations[attackerUnit.FactionIndex]
                    .state != 2)
            {
                throw new Exception("The target Unit is not enemy, so it cannot be attacked");
            }
            UnitSelection.SetTarget(new List<UnitController> {attackerUnit}, GameObject, GameObject.transform.position);
            return false;
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

        public bool Dead
        {
            get { return GameObject == null; }
        }

        public override OwnershipType Ownership
        {
            get { return Player.CurrentPlayer.Units.Contains(this) ? OwnershipType.Owned : OwnershipType.Enemy; }
        }

        public override bool Visible
        {
            get { return Ownership == OwnershipType.Owned || Player.CurrentPlayer.EnemyVisibleUnits.Contains(this); }
        }
    }
}
