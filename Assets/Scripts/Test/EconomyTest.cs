using System;

namespace XposeCraft.Test
{
    /// <summary>
    /// Prva faza hry.
    ///
    /// Cielom je zbierat suroviny pomocou jednotiek pracovnikov
    /// a pri dostatocnom pocte surovin vytvarat dalsich pracovnikov na zrychlenie ekonomie.
    /// </summary>
    class EconomyTest
    {
        public EconomyTest()
        {
        }

        public void EconomyStage(Action startNextStage)
        {
            startNextStage();
        }
    }
}
