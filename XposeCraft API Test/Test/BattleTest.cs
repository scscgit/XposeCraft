using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.App.TestRunner;
using XposeCraft_UI_API_Prototype_Test.Game;
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
		}

		void AttackPhase()
		{
			var myUnits = UnitHelper.GetUnits<IUnit>();
			// TODO: decide if units can do this or just after ForEach
			Array.ForEach(myUnits, unit =>
			{
				unit.AttackMoveTo(PlaceType.EnemyBasePositionRight)
					.After(new WaitForActionsOf(myUnits))
					.After(new CustomFunction(() =>
					{
						WorkersCanExpand();
					}))
					.After(new AttackMove(PlaceType.EnemyBaseCenter));
			});
		}

		void WorkersCanExpand()
		{

		}

		void ScheduleTacticsWhenLowHp(Unit[] units)
		{
			Event.Register(EventType.UnitReceivedFire, args =>
			{
				UnitQueue actions = args<Unit>().GetActions();
				if (args<Unit>().Health < args<Unit>().MaxHealth / 2)
				{
					// Any unit of course exposes its position in form of coordinates; MyBot is custom player’s class
					args<Unit>().MoveTo(MyBot.HealMeetPointUnit.Position);
					MyBot.MeetPointEvent = RegisterEvent(Events.UnitGainedHealth, args =>
					{
						args<Unit>().ReplaceActions(actions);
					});
				}
			});
		}

		void x()
		{
			RegisterEvent(Events.EnemyUnitFound, args =>
			{
				// Decision pending: how to discriminate between two semantics of Unit at EnemyFound event? Will be documented
				var my = args<Unit>(Events.EnemyFound.MyUnit);
				var enemy = args<Unit>(Events.EnemyFound.EnemyUnit);
				if (!MyBot.CurrentEnemies.Contain(enemy)) { MyBot.CurrentEnemies.Put(enemy) }
				// TODO: use Set; TODO: remove on kill / losing sight
			});

			// When destroying enemy base
			RegisterEvent(Events.EnemyBuildingFound, args =>
			{
				// TODO: my unit does not necessarily have to be a unit, even building can see a building
				var my = args<Unit>(Events.EnemyFound.MyUnit); var enemy = args<BuildingTest>();
				GetUnits().ForEach(unit->unit.AddAction(unit->unit.Attack(enemy)));
			});
		}
	}
}
