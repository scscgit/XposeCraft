using UnityEngine;

namespace XposeCraft.GameInternal
{
    public class Log
    {
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }

        public static LogLevel Level = LogLevel.Debug;

        public static void d(object context, object message)
        {
            d(string.Format("{0}: {1}", context, message));
        }

        public static void d(object message)
        {
            if (Level > LogLevel.Debug)
            {
                return;
            }
            Debug.Log(message);
        }

        public static void i(object context, object message)
        {
            i(string.Format("{0}: {1}", context, message));
        }

        public static void i(object message)
        {
            if (Level > LogLevel.Info)
            {
                return;
            }
            Debug.Log(message);
        }

        public static void w(object context, object message)
        {
            if (Level > LogLevel.Warning)
            {
                return;
            }
            Debug.LogWarning(string.Format("{0}: WARNING! {1}", context, message));
        }

        public static void w(object message)
        {
            if (Level > LogLevel.Warning)
            {
                return;
            }
            Debug.LogWarning(string.Format("WARNING! {0}", message));
        }

        public static void e(object context, object message)
        {
            if (Level > LogLevel.Error)
            {
                return;
            }
            Debug.LogError(string.Format("{0}: ERROR!!! {1}", context, message));
        }

        public static void e(object message)
        {
            if (Level > LogLevel.Error)
            {
                return;
            }
            Debug.LogError(string.Format("ERROR!!! {0}", message));
        }
    }
}
