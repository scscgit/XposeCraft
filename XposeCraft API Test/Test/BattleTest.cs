using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.App.TestRunner;
using XposeCraft_UI_API_Prototype_Test.Game;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Buildings;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;
using XposeCraft_UI_API_Prototype_Test.Game.Control;
using XposeCraft_UI_API_Prototype_Test.Game.Control.GameActions;
using XposeCraft_UI_API_Prototype_Test.Game.Enum;
using XposeCraft_UI_API_Prototype_Test.Game.Enums;
using XposeCraft_UI_API_Prototype_Test.Game.Helpers;

/// <summary>
/// Tretia faza hry.
/// 
/// Cielom je pouzitim postavenych jednotiek znicit nepriatela,
/// pripadne pocas boja stavat dalsie jednotky a rozsirovat svoju zakladnu.
/// </summary>

namespace XposeCraft_UI_API_Prototype_Test.Test
{
	class BattleTest
	{
		public BattleTest()
		{
		}

		Event BuildingUpArmy;

		public void BattleStage(TestRunner.NextStageStarter startNextStage)
		{
			// Plan individual unit attacks or a return when meeting too many units
			BuildingUpArmy = Event.Register(EventType.EnemyUnitsOnSight, args =>
			{
				if (args.EnemyUnits.Length > UnitHelper.GetUnits<IUnit>().Length)
				{
					// Too many enemies, return back
					foreach (IUnit unit in UnitHelper.GetUnits<IUnit>())
					{
						unit.MoveTo(PlaceType.NearBase);
					}
				}
				else
				{
					UnitActionQueue queue = new UnitActionQueue();
					foreach (Unit enemy in args.EnemyUnits)
					{
						queue.After(new Attack(enemy));
					}
					foreach (IUnit unit in UnitHelper.GetUnits<IUnit>())
					{
						unit.ReplaceActionQueue(queue);
					}
				}
			});

			// Initiate the attack
			GoAttack();
			// When the units get to low health, they will return back to heal
			ScheduleTacticsWhenLowHp();
			// Stores enemies for later possible calculations
			RememberEnemies();
			// Destroys buildings when possible, should be preferable contitioned
			DestroyBuildings();
		}

		void GoAttack()
		{
			var myUnits = UnitHelper.GetUnits<IUnit>();
			// TODO: decide if units can do this or just after ForEach
			Array.ForEach(myUnits, unit =>
			{
				unit.AttackMoveTo(PlaceType.EnemyBasePositionRight)
					.After(new WaitForActionsOf(myUnits))
					.After(new CustomFunction(() => { WorkersCanExpand(); }))
					.After(new AttackMove(PlaceType.EnemyBaseCenter));
			});
		}

		void WorkersCanExpand()
		{
			// TODO: make workers expand to other minerals over the map
		}

		void ScheduleTacticsWhenLowHp()
		{
			Event.Register(EventType.UnitReceivedFire, args =>
			{
				UnitActionQueue oldActions = args.MyUnit.ActionQueue;
				if (args.MyUnit.Health < args.MyUnit.MaxHealth / 2)
				{
					// Any unit of course exposes its position in form of coordinates; MyBot is custom player’s class
					args.MyUnit.MoveTo(MyBot.HealMeetPointUnit.Position);
					MyBot.MeetPointEvent = Event.Register(EventType.UnitGainedHealth,
						argsB => { argsB.MyUnit.ReplaceActionQueue(oldActions); });
				}
			});
		}

		void RememberEnemies()
		{
			Event.Register(EventType.EnemyUnitsOnSight, args =>
			{
				var enemies = args.EnemyUnits;
				foreach (IUnit enemy in enemies)
				{
					if (!MyBot.CurrentEnemies.Contains(enemy))
					{
						MyBot.CurrentEnemies.Add(enemy);
					}
				}
				// TODO: use Set; TODO: remove on kill / losing sight
			});
		}

		// Preferably called only when destroying enemy base
		void DestroyBuildings()
		{
			Event.Register(EventType.EnemyBuildingsOnSight, args =>
			{
				// TODO: my unit does not necessarily have to be a unit, even building can see a building
				var enemies = args.EnemyBuildings;
				foreach (IUnit unit in UnitHelper.GetUnits<IUnit>())
				{
					foreach (IBuilding enemy in enemies)
					{
						unit.ActionQueue.After(new Attack(enemy));
					}
				}
			});
		}
	}
}