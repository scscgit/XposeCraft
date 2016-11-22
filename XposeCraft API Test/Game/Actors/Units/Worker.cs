using System;
using System.Collections.Generic;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Materials;
using XposeCraft.Game.Enums;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Actors.Units
{
	/// <summary>
	/// Can gather various materials (mainly minerals) and build various buildings - based on a current API level.
	/// </summary>
	class Worker : Unit
	{
		static readonly int MAX_HP = 120;

		static readonly int BASE_CENTER_DELAY = 150;
		static readonly int NUBIAN_ARMORY_DELAY = 75;

		public Worker(Position position) : base(position, MAX_HP)
		{
		}

		public IMaterial Gathering
		{
			get; private set;
		} = null;

		public void SendGather(IMaterial material)
		{
			Gathering = material;
		}

		// TODO:
		// 1. send worker to the position near building, queued event when arrival
		// 2. start construction object, queued event when finished
		// 2.5. if interrupted, worker can repeat step 1 and continue on 2 without creating a new object
		// 3. finished event, return to gather
		public void CreateBuilding(BuildingType buildingType, Position position)
		{
			IBuilding building;
			switch (buildingType)
			{
				case BuildingType.BaseCenter:
					building = new BaseCenter(position);
					GameTimer.Schedule(BASE_CENTER_DELAY, () =>
					{
						building.IsFinished = true;
					});
					break;
				case BuildingType.NubianArmory:
					building = new NubianArmory(position);
					GameTimer.Schedule(NUBIAN_ARMORY_DELAY, () =>
					{
						building.IsFinished = true;
					});
					break;
				default:
					throw new Exception("Wrong building type parameter in CreateBuilding");
			}
			Model.Instance.Buildings.Add(building);
		}
	}
}
