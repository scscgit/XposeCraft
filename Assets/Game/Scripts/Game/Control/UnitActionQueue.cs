using System.Collections.Generic;
using XposeCraft.Game.Control.GameActions;

namespace XposeCraft.Game.Control
{
    public class UnitActionQueue
    {
        private Queue<IGameAction> _queue = new Queue<IGameAction>();

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
            return this;
        }
    }
}
