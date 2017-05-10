using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Game.Control.GameActions
{
    public interface IGameAction
    {
        /// <summary>
        /// Internal, do not use directly.
        /// Action invocation that attempts to finish the required operation.
        /// </summary>
        /// <param name="unit">Unit actor that invoked the Action.</param>
        /// <param name="unitController">Controller script of the Unit intended to have the Action ran.</param>
        /// <returns>true if the operation is finished and the Action should never be invoked again.</returns>
        bool Progress(IUnit unit, UnitController unitController);

        /// <summary>
        /// Internal, do not use directly.
        /// Finishing operation guaranteed to be executed after the Action ends, before any other Action is invoked.
        /// </summary>
        /// <param name="unit">Unit actor that invoked the Action.</param>
        /// <param name="unitController">Controller script of the Unit intended to have the Action ran.</param>
        void OnFinish(IUnit unit, UnitController unitController);
    }
}
