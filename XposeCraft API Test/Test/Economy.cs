using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Buildings;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;
using XposeCraft_UI_API_Prototype_Test.Game.Enums;
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

			EventForCreatingAnother();
		}

		void EventForCreatingAnother()
		{
			Event.Register(EventType.MineralsChanged, argsA =>
			{
				if (argsA.Minerals > 50)
				{
					// After he collected minerals, another worker will be built
					var baseCenter = BuildingHelper.GetBuildings<BaseCenter>()[0];
					baseCenter.CreateWorker();

					// After creating (it means after few seconds), he will need to go gather too
					Event.Register(EventType.UnitCreated, argsB =>
					{
						if (argsB.Unit.GetType().Equals(typeof(Worker)))
						{
							Worker worker = (Worker)argsB.Unit;
							worker.SendGather(MaterialHelper.GetNearestMineralsTo(worker));
							argsB.ThisEvent.UnregisterEvent();
						}
					});

				}
				argsA.ThisEvent.UnregisterEvent();

				// This event will work only while there are not enough workers.
				// After that, minerals will be left to go over 150.
				if (UnitHelper.GetUnits<Worker>().Length >= 5)
				{
					argsA.ThisEvent.UnregisterEvent();
				}
			});
		}
	}
}