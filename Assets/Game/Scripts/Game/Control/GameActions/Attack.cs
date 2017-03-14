using XposeCraft.Game.Actors;

namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Action of attack to a unit
    /// </summary>
    class Attack : GameAction
    {
        private IActor Target { get; set; }

        public Attack(IActor target)
        {
            Target = target;
        }
    }
}
