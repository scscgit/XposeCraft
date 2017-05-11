using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Game.Control.GameActions
{
    public abstract class GameAction : IGameAction
    {
        public virtual bool Progress(IUnit unit, UnitController unitController)
        {
            if (unitController != null)
            {
                return true;
            }
            if (UnitActionQueue.ExceptionOnDeadUnitAction)
            {
                throw new UnitDeadException();
            }
            return false;
        }

        public virtual void OnFinish(IUnit unit, UnitController unitController)
        {
        }
    }
}
