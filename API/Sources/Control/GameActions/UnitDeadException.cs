using System;

namespace XposeCraft.Game.Control.GameActions
{
    public class UnitDeadException : Exception
    {
        public UnitDeadException() : base("Unit cannot perform the Action, because sadly, it is already dead")
        {
        }
    }
}
