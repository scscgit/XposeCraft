using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Buildings;
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
			// Game started, the first worker will get to work
			Worker firstWorker = UnitHelper.GetUnits<Worker>()[0];
			firstWorker.SendGather(MaterialHelper.GetNearestMineralsTo(firstWorker));

			// After he collected minerals, he will get to start the next stage
			Event.Register(Events.MineralsChanged, args =>
			{
				if (args.Minerals > 50)
				{
					var baseCenter = BuildingHelper.GetBuildings<BaseCenter>()[0];
					baseCenter.
					// After creating, he needs to go gather too
					Event.Register(Events.UnitCreated, args =>
					{
						if (args.Unit.GetType().Equals(typeof(Worker)))
						{
							TryNubianArmory();
							worker.SendGather(MaterialHelper.GetNearestMineralsTo(worker));

						}
					});

				}
			});
		}
	}
}
