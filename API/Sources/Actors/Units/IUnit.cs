using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Control;
using XposeCraft.Game.Control.GameActions;

namespace XposeCraft.Game.Actors.Units
{
    /// <summary>
    /// Unit able to freely move in the game and enqueue multiple <see cref="IGameAction"/> commands.
    /// </summary>
    public interface IUnit : IActor
    {
        /// <summary>
        /// Current health of the Unit, 0 means dead. Maximum value is represented by <see cref="MaxHealth"/> field.
        /// </summary>
        int Health { get; }

        /// <summary>
        /// Maximum health of the Unit. Current health is represented by <see cref="Health"/> field.
        /// </summary>
        int MaxHealth { get; }

        /// <summary>
        /// Queued actions of the Unit. After their completion the Unit will no longer do any action.
        /// </summary>
        UnitActionQueue ActionQueue { get; set; }

        /// <summary>
        /// Unit will attack another Unit.
        /// </summary>
        /// <param name="unit">Target of attack.</param>
        /// <returns>Chained queue to add other actions.</returns>
        UnitActionQueue Attack(IUnit unit);

        /// <summary>
        /// Unit will attack another Building.
        /// </summary>
        /// <param name="building">Target of attack.</param>
        /// <returns>Chained queue to add other actions.</returns>
        UnitActionQueue Attack(IBuilding building);

        /// <summary>
        /// Unit will move to a new Position.
        /// </summary>
        /// <param name="position">Target position.</param>
        /// <returns>Chained queue to add other actions.</returns>
        UnitActionQueue MoveTo(Position position);

        /// <summary>
        /// Unit will move to a new position, but will attack any enemies on sight before it continues to move.
        /// </summary>
        /// <param name="position">Target position.</param>
        /// <returns>Chained queue to add other actions.</returns>
        UnitActionQueue AttackMoveTo(Position position);

        /// <summary>
        /// True if the Unit already died an can never be used again.
        /// </summary>
        bool Dead { get; }
    }
}
