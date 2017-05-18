using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Control;

namespace XposeCraft.Game.Actors.Units
{
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
    }
}
