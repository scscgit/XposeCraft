using System;
using System.Collections.Generic;
using XposeCraft.App.TestRunner;

namespace XposeCraft.Test
{
	/// <summary>
	/// Druha faza hry.
	///
	/// Cielom je pomocou pracovnikov vytvarat nove budovy,
	/// ktore budu produkovat vojenske jednotky, alebo inak ich vylepsovanim rozsirovat pravdepodobnost vyhry.
	/// </summary>
	class BuildingTest
	{
		public BuildingTest()
		{
		}

		public void BuildingStage(Runner.NextStageStarter startNextStage)
		{
			startNextStage();
		}
	}
}
