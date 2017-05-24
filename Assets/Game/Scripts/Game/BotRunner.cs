using UnityEngine;
using XposeCraft.Core.Misc;
using XposeCraft.GameInternal;

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
            set
            {
                if (value)
                {
                    GameInternal.Tutorial.Instance.TutorialStart();
                }
                else
                {
                    GameInternal.Tutorial.Instance.TutorialStop();
                }
            }
        }

        /// <summary>
        /// Configures the ability run the all Player's code again, right after it gets recompiled by Unity.
        /// If the value is false, the code will only run once after the game is started.
        /// </summary>
        public static bool HotSwap
        {
            set
            {
                GameManager.Instance.HotSwap = value;
                GameInternal.Tutorial.Instance.OnHotSwap();
            }
        }

        /// <summary>
        /// If the <see cref="HotSwap"/> is turned on, this can be used to check if the current script execution
        /// happened other than the first time. False means this is the first execution during the single Game run.
        /// It is advised to cache the value. It must be also requested during the initialization, not in Events!
        /// </summary>
        public static bool CurrentlyAfterHotswap
        {
            get { return GameObject.Find(GameTestRunner.ScriptName).GetComponent<GameTestRunner>().WasStarted; }
        }

        /// <summary>
        /// Modifier of the game speed in the Unity editor, value between 0 and 100.
        /// </summary>
        public static int Speed
        {
            set { GameObject.Find("Player Manager").GetComponent<AlterTime>().NormalSpeed = value; }
        }

        /// <summary>
        /// Logs a string message into the Unity console.
        /// All C# objects provide method ToString() that can be used to get a string format.
        /// </summary>
        /// <param name="message">Mesage to be displayed.</param>
        public static void Log(string message)
        {
            GameInternal.Log.i(message);
        }
    }
}
