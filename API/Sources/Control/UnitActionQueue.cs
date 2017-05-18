using System;
using XposeCraft.Game.Control.GameActions;

namespace XposeCraft.Game.Control
{
    public class UnitActionQueue
    {
        /// <summary>
        /// If true, attempt at executing an Action with a dead Unit will throw a <see cref="UnitDeadException"></see>.
        /// </summary>
        public static bool ExceptionOnDeadUnitAction
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int QueueCount
        {
            get { throw new NotImplementedException(); }
        }

        public UnitActionQueue()
        {
            throw new NotImplementedException();
        }

        public UnitActionQueue(IGameAction action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enqueues a new Action at the end of the Queue.
        /// </summary>
        /// <param name="action">Action to be acted on by the Queue owner after finishing all previous Actions.</param>
        /// <returns>This queue to add other actions.</returns>
        public UnitActionQueue After(IGameAction action)
        {
            throw new NotImplementedException();
        }
    }
}
