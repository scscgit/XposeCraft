using UnityEngine;

namespace XposeCraft.GameInternal
{
    public class Log
    {
        public static void i(object context, object message)
        {
            Debug.Log(context + ": " + message);
        }

        public static void i(object message)
        {
            Debug.Log(message);
        }

        public static void e(object context, object message)
        {
            Debug.LogError(context + ": ERROR!!! " + message);
        }

        public static void e(object message)
        {
            Debug.LogError("ERROR!!! " + message);
        }
    }
}
