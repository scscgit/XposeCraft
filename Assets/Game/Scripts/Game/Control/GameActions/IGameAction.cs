namespace XposeCraft.Game.Control.GameActions
{
    public interface IGameAction
    {
        /// <summary>
        /// Action invocation that attempts to finish the required operation.
        /// </summary>
        /// <returns>true if the operation is finished and the Action should never be invoked again.</returns>
        bool Progress();
    }
}
