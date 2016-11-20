using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game;

/// <summary>
/// Tretia faza hry.
/// 
/// Cielom je pouzitim postavenych jednotiek znicit nepriatela,
/// pripadne pocas boja stavat dalsie jednotky a rozsirovat svoju zakladnu.
/// </summary>
namespace XposeCraft_UI_API_Prototype_Test.Test
{
	public class Battle
	{
		public Battle()
		{
		}

		/*
		public void BattleStage()
		{
			var buildingArmy = Event.Register(Events.EnemyOnSight, args =>
			{
				if (args<Unit[]>().Length > GetUnits().Length)
				{
					GetUnits().MoveTo(Places.Base);
				}
				else {
					UnitQueue queue = new UnitQueue();
					foreach (Unit enemy in args<Unit[]>())
					{
						queue.AddAttack(enemy);
					}
					GetUnits().ReplaceActions(queue);
				}
			});
		}

		private void AttackPhase()
		{
			GetUnits().AttackMoveTo(Places.EnemyBasePositionF) // TODO: decide if units can do this or just after ForEach
				.After(units->units.ForEach(unit->unit.WaitForActionsOf(units)))
				.After(units->WorkersCanExpand())
				.After(units-> {
				units.ForEach(unit->unit.AttackMoveTo(Places.EnemyBaseCenter); )
					ScheduleTacticsWhenLowHp(units);
			});
		}

		private void ScheduleTacticsWhenLowHp(Unit[] units)
		{
			RegisterEvent(Events.UnitReceivedFire, args =>
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
			RegisterEvent(Events.EnemyUnitFound, args => {
				// Decision pending: how to discriminate between two semantics of Unit at EnemyFound event? Will be documented
				var my = args<Unit>(Events.EnemyFound.MyUnit);
				var enemy = args<Unit>(Events.EnemyFound.EnemyUnit);
				if (!MyBot.CurrentEnemies.Contain(enemy)) { MyBot.CurrentEnemies.Put(enemy) }
				// TODO: use Set; TODO: remove on kill / losing sight
			});

			// When destroying enemy base
			RegisterEvent(Events.EnemyBuildingFound, args => {
				// TODO: my unit does not necessarily have to be a unit, even building can see a building
				var my = args<Unit>(Events.EnemyFound.MyUnit); var enemy = args<Building>();
				GetUnits().ForEach(unit->unit.AddAction(unit->unit.Attack(enemy)));
			});
		}
	*/
	}
}
