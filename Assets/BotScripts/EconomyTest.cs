using System;
using XposeCraft.Game;

namespace XposeCraft.BotScripts
{
    /// <summary>
    /// Prva faza hry.
    ///
    /// Cielom je zbierat suroviny pomocou jednotiek pracovnikov
    /// a pri dostatocnom pocte surovin vytvarat dalsich pracovnikov na zrychlenie ekonomie.
    /// </summary>
    public class EconomyTest : BotScript
    {
        public MyBotData MyBotData;

        public void EconomyStage(Action startNextStage)
        {
            startNextStage();
        }
    }
}
