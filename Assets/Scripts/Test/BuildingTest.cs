using System;
using UnityEngine;

namespace XposeCraft.Test
{
    /// <summary>
    /// Druha faza hry.
    ///
    /// Cielom je pomocou pracovnikov vytvarat nove budovy,
    /// ktore budu produkovat vojenske jednotky, alebo inak ich vylepsovanim rozsirovat pravdepodobnost vyhry.
    /// </summary>
    public class BuildingTest : ScriptableObject
    {
        public MyBotData MyBotData;

        public void BuildingStage(Action startNextStage)
        {
            startNextStage();
        }
    }
}
