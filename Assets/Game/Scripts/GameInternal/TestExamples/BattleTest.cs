using System;
using UnityEngine;
using XposeCraft.Game;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Control;
using XposeCraft.Game.Control.GameActions;
using XposeCraft.Game.Enums;
using XposeCraft.Game.Helpers;

namespace XposeCraft.GameInternal.TestExamples
{
    /// <summary>
    /// Tretia faza hry.
    ///
    /// Cielom je pouzitim postavenych jednotiek znicit nepriatela,
    /// pripadne pocas boja stavat dalsie jednotky a rozsirovat svoju zakladnu.
    /// </summary>
    internal class BattleTest : ScriptableObject
    {
        public MyBotData MyBotData;

        public void BattleStage(Action startNextStage)
        {
            // Plan individual unit attacks or a return when meeting too many units
            GameEvent.Register(GameEventType.EnemyUnitsOnSight, args =>
            {
                if (args.EnemyUnits.Length > UnitHelper.GetMyUnits<IUnit>().Length)
                {
                    // Too many enemies, return back
                    foreach (IUnit unit in UnitHelper.GetMyUnits<IUnit>())
                    {
                        unit.MoveTo(PlaceType.MyBase.Center);
                    }
                }
                else
                {
                    UnitActionQueue queue = new UnitActionQueue();
                    foreach (IUnit enemy in args.EnemyUnits)
                    {
                        queue.After(new Attack(enemy));
                    }
                    foreach (IUnit unit in UnitHelper.GetMyUnits<IUnit>())
                    {
                        unit.ActionQueue = queue;
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
            var myUnits = UnitHelper.GetMyUnits<IUnit>();
            // TODO: decide if units can do this or just after ForEach
            Array.ForEach(myUnits, unit =>
            {
                unit.AttackMoveTo(PlaceType.EnemyBase.Right)
                    .After(new WaitForActionsOf(myUnits))
                    .After(new CustomFunction(() => { WorkersCanExpand(); }))
                    .After(new AttackMove(PlaceType.EnemyBase.Center));
            });
        }

        void WorkersCanExpand()
        {
            // TODO: make workers expand to other minerals over the map
        }

        void ScheduleTacticsWhenLowHp()
        {
            GameEvent.Register(GameEventType.UnitReceivedFire, args =>
            {
                UnitActionQueue oldActions = args.MyUnit.ActionQueue;
                if (args.MyUnit.Health < args.MyUnit.MaxHealth / 2)
                {
                    // Any unit of course exposes its position in form of coordinates; MyBotData contains data
                    args.MyUnit.MoveTo(MyBotData.HealMeetPointUnit.Position);
                    MyBotData.MeetPointEvent = GameEvent.Register(GameEventType.UnitGainedHealth,
                        argsB => { argsB.MyUnit.ActionQueue = oldActions; });
                }
            });
        }

        void RememberEnemies()
        {
            GameEvent.Register(GameEventType.EnemyUnitsOnSight, args =>
            {
                var enemies = args.EnemyUnits;
                foreach (IUnit enemy in enemies)
                {
                    if (!MyBotData.CurrentEnemies.Contains(enemy))
                    {
                        MyBotData.CurrentEnemies.Add(enemy);
                    }
                }
                // TODO: use Set; TODO: remove on kill / losing sight
            });
        }

        // Preferably called only when destroying enemy base
        void DestroyBuildings()
        {
            GameEvent.Register(GameEventType.EnemyBuildingsOnSight, args =>
            {
                // TODO: my unit does not necessarily have to be a unit, even building can see a building
                var enemies = args.EnemyBuildings;
                foreach (IUnit unit in UnitHelper.GetMyUnits<IUnit>())
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
