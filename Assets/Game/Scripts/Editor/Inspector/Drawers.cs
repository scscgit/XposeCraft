using UnityEditor;
using XposeCraft.Game.Enums;
using XposeCraft.GameInternal;

namespace XposeCraft.Inspector
{
    [CustomPropertyDrawer(typeof(Player.RegisteredEventsDictionary))]
    public class RegisteredEventsDictionaryDrawer : DictionaryDrawer<EventType, Player.EventList>
    {
    }
}
