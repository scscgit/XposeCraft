using UnityEngine;
using XposeCraft.Game;

namespace XposeCraft.Test
{
    /// <summary>
    /// Additional serialized place to store custom data and references.
    /// </summary>
    public class MyBotData : ScriptableObject
    {
        public int SomeNumber;
        public GameEvent SomeEvent;
    }
}
