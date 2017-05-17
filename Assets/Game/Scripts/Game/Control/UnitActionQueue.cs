using System;
using System.Collections.Generic;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Control.GameActions;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Control
{
    public class UnitActionQueue
    {
        /// <summary>
        /// Internal accessor for a dequeue operation over the Action Queue.
        /// </summary>
        internal class UnitActionDequeue
        {
            private IUnit _unit;
            private UnitController _unitController;
            private UnitActionQueue _unitActionQueue;
            private IGameAction _currentAction;

            internal UnitActionDequeue(IUnit unit, UnitController unitController, UnitActionQueue queue)
            {
                _unit = unit;
                _unitController = unitController;
                _unitActionQueue = queue;
            }

            public void Finished()
            {
                if (_currentAction == null)
                {
                    return;
                }
                var previousAction = _currentAction;
                _currentAction = null;
                // Player context is needed for all asynchronous actions
                Player.CurrentPlayer = _unitController.PlayerOwner;
                previousAction.OnFinish(_unit, _unitController);
            }

            public void Dequeue()
            {
                Finished();
                if (_unitActionQueue._queue.Count == 0)
                {
                    return;
                }
                _currentAction = _unitActionQueue._queue.Peek();
                // Player context is needed for all asynchronous actions
                Player.CurrentPlayer = _unitController.PlayerOwner;
                try
                {
                    if (!_currentAction.Progress(_unit, _unitController))
                    {
                        return;
                    }
                    Remove();
                }
                catch (Exception)
                {
                    // If the progress failed because of an exception, it mustn't block the queue (debugging purposes)
                    Remove();
                    throw;
                }
            }

            private void Remove()
            {
                Log.d(string.Format("Unit {0} dequeued {1} action", _unitController.name, _currentAction.GetType()));
                _unitActionQueue._queue.Dequeue();
            }
        }

        /// <summary>
        /// If true, attempt at executing an Action with a dead Unit will throw a <see cref="UnitDeadException"></see>.
        /// </summary>
        public static bool ExceptionOnDeadUnitAction
        {
            get { return Player.CurrentPlayer.ExceptionOnDeadUnitAction; }
            set { Player.CurrentPlayer.ExceptionOnDeadUnitAction = value; }
        }

        private Queue<IGameAction> _queue = new Queue<IGameAction>();

        public int QueueCount
        {
            get { return _queue.Count; }
        }

        public UnitActionQueue()
        {
        }

        public UnitActionQueue(IGameAction action)
        {
            After(action);
        }

        /// <summary>
        /// Enqueues a new Action at the end of the Queue.
        /// </summary>
        /// <param name="action">Action to be acted on by the Queue owner after finishing all previous Actions.</param>
        /// <returns>This queue to add other actions.</returns>
        public UnitActionQueue After(IGameAction action)
        {
            _queue.Enqueue(action);
            if (_queue.Count > 1)
            {
                Tutorial.Instance.ActionQueue();
            }
            return this;
        }
    }
}
