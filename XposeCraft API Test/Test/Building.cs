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
/// Druha faza hry.
/// 
/// Cielom je pomocou pracovnikov vytvarat nove budovy,
/// ktore budu produkovat vojenske jednotky, alebo inak ich vylepsovanim rozsirovat pravdepodobnost vyhry.
/// </summary>
namespace XposeCraft_UI_API_Prototype_Test.Test
{
	public class Building
	{
		public Building()
		{
		}

		/// <summary>
		/// Finds a worker that is just gathering any materials, without any other task
		/// </summary>
		/// <returns>A bored worker</returns>
		private Worker FindWorkerThatGathers()
		{
			foreach (Worker worker in UnitHelper.GetUnits<Worker>())
			{
				if (worker.Gathering != null)
				{
					return worker;
				}
			}
			// First one by default if all have work, a bad workaround
			return UnitHelper.GetUnits<Worker>()[0];
		}

		public void BuildingStage()
		{
			// A first building
			Event.Register(EventType.MineralsChanged, args =>
			{
				if (args.Minerals > 150)
				{
					var baseCenter = BuildingHelper.GetBuildings<BaseCenter>()[0];
					var position = BuildingHelper.ClosestEmptySpaceTo(baseCenter);
					FindWorkerThatGathers().CreateBuilding(BuildingType.NubianArmory, position);

					// We only need one army production building for now
					args.ThisEvent.UnregisterEvent();
				}
			});

			// Worker will return to work afterwards
			Event.Register(EventType.BuildingCreated, args =>
			{
				if (
				args.Building.GetType().Equals(typeof(NubianArmory))
				&&
				args.Unit.GetType().Equals(typeof(Worker))
				)
				{
					var worker = (Worker)args.Unit;
					worker.SendGather(MaterialHelper.GetNearestMineralsTo(worker));
					BuildArmy();
				}
				args.ThisEvent.UnregisterEvent();
			});
		}

		void BuildArmy()
		{
			var buildingArmy = Event.Register(EventType.MineralsChanged, args =>
			{
				if (args.Minerals > 100)
				{
					foreach (NubianArmory armory in BuildingHelper.GetBuildings<NubianArmory>())
					{
						if (armory.CreateUnit(UnitType.DonkeyGun))
						{
							break;
						}
					}
					if (MyBot.Army++ >= 5)
					{
						args.ThisEvent.UnregisterEvent();
						MyBot.AttackPhase = true;
					}
				}
			});
		}
	}
}
