using System;
using UnityEngine;

namespace XposeCraft.Test
{
    /// <summary>
    /// Tretia faza hry.
    ///
    /// Cielom je pouzitim postavenych jednotiek znicit nepriatela,
    /// pripadne pocas boja stavat dalsie jednotky a rozsirovat svoju zakladnu.
    /// </summary>
    public class BattleTest : ScriptableObject
    {
        public MyBotData MyBotData;

        public void BattleStage(Action startNextStage)
        {
            startNextStage();
        }
    }
}
