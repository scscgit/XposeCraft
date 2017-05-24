using System;
using XposeCraft.Game;

namespace XposeCraft.BotScripts
{
    /// <summary>
    /// Tretia faza hry.
    ///
    /// Cielom je pouzitim postavenych jednotiek znicit nepriatela,
    /// pripadne pocas boja stavat dalsie jednotky a rozsirovat svoju zakladnu.
    /// </summary>
    public class BattleTest : BotScript
    {
        public MyBotData MyBotData;

        public void BattleStage(Action startNextStage)
        {
            startNextStage();
        }
    }
}
