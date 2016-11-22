using System;
using System.Collections.Generic;
using XposeCraft.Game.Actors.Units;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Actors.Buildings
{
	/// <summary>
	/// A base building that creates new workers and receives collected materials.
	/// </summary>
	class BaseCenter : Building
	{
		public BaseCenter(Position position) : base(position)
		{
		}

		public static readonly int WorkerCreateTime = 50;

		public bool CreateWorker()
		{
			if (Model.Instance.Minerals >= 50)
			{
				Model.Instance.Minerals -= 50;
				EventScheduler.CreateUnit
				(
					WorkerCreateTime,
					() => Model.Instance.Units.Add(new Worker(this.Position))
				);
				return true;
			}
			return false;
		}
	}
}
