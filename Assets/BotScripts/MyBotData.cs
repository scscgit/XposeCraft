using XposeCraft.Game;

namespace XposeCraft.Test
{
    /// <summary>
    /// Additional serialized place to store custom data and references.
    /// </summary>
    public class MyBotData : BotScript
    {
        public int SomeNumber;
        public GameEvent SomeEvent;
    }
}
