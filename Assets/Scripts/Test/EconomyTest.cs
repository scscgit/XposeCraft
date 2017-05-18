using System;
using UnityEngine;

namespace XposeCraft.Test
{
    /// <summary>
    /// Prva faza hry.
    ///
    /// Cielom je zbierat suroviny pomocou jednotiek pracovnikov
    /// a pri dostatocnom pocte surovin vytvarat dalsich pracovnikov na zrychlenie ekonomie.
    /// </summary>
    public class EconomyTest : ScriptableObject
    {
        public MyBotData MyBotData;

        public void EconomyStage(Action startNextStage)
        {
            startNextStage();
        }
    }
}
