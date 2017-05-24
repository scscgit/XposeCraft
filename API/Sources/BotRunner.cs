using System;

namespace XposeCraft.Game
{
    /// <summary>
    /// Configuration for running your Bot.
    /// </summary>
    public class BotRunner
    {
        /// <summary>
        /// Starts or disables the Tutorial.
        /// </summary>
        public static bool Tutorial
        {
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Configures the ability run the all Player's code again, right after it gets recompiled by Unity.
        /// If the value is false, the code will only run once after the game is started.
        /// </summary>
        public static bool HotSwap
        {
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// If the <see cref="HotSwap"/> is turned on, this can be used to check if the current script execution
        /// happened other than the first time. False means this is the first execution during the single Game run.
        /// It is advised to cache the value. It must be also requested during the initialization, not in Events!
        /// </summary>
        public static bool CurrentlyAfterHotswap
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Modifier of the game speed in the Unity editor, value between 0 and 100.
        /// </summary>
        public static int Speed
        {
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Logs a string message into the Unity console.
        /// All C# objects provide method ToString() that can be used to get a string format.
        /// </summary>
        /// <param name="message">Mesage to be displayed.</param>
        public static void Log(string message)
        {
            throw new NotImplementedException();
        }
    }
}
