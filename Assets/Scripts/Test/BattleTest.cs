using System;

namespace XposeCraft.Test
{
    /// <summary>
    /// Tretia faza hry.
    ///
    /// Cielom je pouzitim postavenych jednotiek znicit nepriatela,
    /// pripadne pocas boja stavat dalsie jednotky a rozsirovat svoju zakladnu.
    /// </summary>
    class BattleTest
    {
        public void BattleStage(Action startNextStage)
        {
            startNextStage();
        }
    }
}
