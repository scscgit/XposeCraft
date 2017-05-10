using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Game.Control.GameActions
{
    public abstract class GameAction : IGameAction
    {
        public virtual bool Progress(IUnit unit, UnitController unitController)
        {
            return true;
        }

        public virtual void OnFinish(IUnit unit, UnitController unitController)
        {
        }
    }
}
