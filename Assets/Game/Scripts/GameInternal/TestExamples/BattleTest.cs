using System;
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
    internal class BattleTest : BotScript
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

        public static void RegisterReceiveFire()
        {
            var receivedFire = new Action<Arguments>(args =>
            {
                foreach (var enemyUnit in args.EnemyUnits)
                {
                    BotRunner.Log(enemyUnit + " attacked me");
                    // The Unit defends itself
                    if (args.MyUnit != null)
                    {
                        EnqueueFirst(new Attack(enemyUnit), args.MyUnit);
                    }
                    // Another Unit helps
                    AttackUnit(enemyUnit);
                }
            });
            GameEvent.Register(GameEventType.UnitReceivedFire, args => receivedFire.Invoke(args));
            GameEvent.Register(GameEventType.BuildingReceivedFire, args => receivedFire.Invoke(args));
        }

        public static void AttackUnit(IUnit enemyUnit)
        {
            var donkeys = UnitHelper.GetMyUnits<DonkeyGun>();
            if (donkeys.Length < 2)
            {
                EnqueueFirst(new Attack(enemyUnit), BuildingTest.FindWorkerThatGathers());
                return;
            }
            foreach (var donkeyGun in UnitHelper.GetMyUnits<DonkeyGun>())
            {
                EnqueueFirst(new Attack(enemyUnit), donkeyGun);
            }
        }

        public static void EnqueueFirst(IGameAction gameAction, IUnit myUnit)
        {
            // Puts the new target above the current Queue
            var oldQueue = myUnit.ActionQueue;
            myUnit.ActionQueue = new UnitActionQueue(gameAction)
                .After(new CustomFunction(() => myUnit.ActionQueue = oldQueue));
        }

        void GoAttack()
        {
            var myUnits = UnitHelper.GetMyUnits<Unit>();
            // Default meet point is any other unit
            MyBotData.HealMeetPointUnit = myUnits[0];
            // TODO: decide if units can do this or just after ForEach
            Array.ForEach(myUnits, unit =>
            {
                // Busy builders won't go into the war
                if (unit is Worker && !(unit.ActionQueue.CurrentAction is GatherResource))
                {
                    return;
                }
                unit.AttackMoveTo(PlaceType.EnemyBase.Right)
                    .After(new WaitForActionsOf(myUnits))
                    .After(new CustomFunction(() =>
                    {
                        MyBotData.HealMeetPointUnit = unit;
                        WorkersCanExpand();
                    }))
                    .After(new AttackMove(PlaceType.EnemyBase.Center));
            });
        }

        void WorkersCanExpand()
        {
            var baseCenter = BuildingHelper.GetMyBuildings<BaseCenter>()[0];
            if (baseCenter != null && baseCenter.CanNowProduceUnit(UnitType.Worker))
            {
                baseCenter.ProduceUnit(UnitType.Worker);
            }
            // TODO: make workers expand to other minerals over the map
            BuildingTest.FindWorkerThatGathers().MoveTo(PlaceType.MyBase.UnderRampLeft);
        }

        void ScheduleTacticsWhenLowHp()
        {
            GameEvent.Register(GameEventType.UnitReceivedFire, args =>
            {
                // If attacking, run away
                if (args.MyUnit.ActionQueue.CurrentAction is Attack
                    || args.MyUnit.ActionQueue.CurrentAction is AttackMove)
                {
                    UnitActionQueue oldActions = args.MyUnit.ActionQueue;
                    if (args.MyUnit.Health < args.MyUnit.MaxHealth / 2)
                    {
                        // Any unit of course exposes its position in form of coordinates; MyBotData contains data
                        args.MyUnit.MoveTo(MyBotData.HealMeetPointUnit.Position);
                        BotRunner.Log("Unit " + args.MyUnit + " is waiting to be healed");
                        MyBotData.MeetPointEvent = GameEvent.Register(GameEventType.UnitGainedHealth,
                            argsB => { argsB.MyUnit.ActionQueue = oldActions; });
                    }
                }
                // If hiding, defend
                else if (args.EnemyUnits.Length > 0)
                {
                    EnqueueFirst(new Attack(args.EnemyUnits[0]), args.MyUnit);
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
