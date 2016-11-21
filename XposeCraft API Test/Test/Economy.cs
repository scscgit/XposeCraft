using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;
using XposeCraft_UI_API_Prototype_Test.Game.Helpers;

/// <summary>
/// Prva faza hry.
/// 
/// Cielom je zbierat suroviny pomocou jednotiek pracovnikov
/// a pri dostatocnom pocte surovin vytvarat dalsich pracovnikov na zrychlenie ekonomie.
/// </summary>
namespace XposeCraft_UI_API_Prototype_Test.Test
{
	public class Economy
	{
		public Economy()
		{
		}

		public void EconomyStage()
		{
			Worker firstWorker = UnitHelper.GetUnits<Worker>()[0];
			firstWorker.SendGather(MaterialHelper.GetNearestMineralsTo(firstWorker));

			Event.Register(Events.MineralsChanged, args =>
			{
				if (args.Minerals > 50)
				{
					Worker worker = BuildingHelper.CreateUnit<Worker>();
					worker.SendGather();
					TryNubianArmory();
				}
			});
		}
	}
}
