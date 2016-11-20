using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;
using XposeCraft_UI_API_Prototype_Test.GameInternal;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors.Buildings
{
	/// <summary>
	/// A base building that creates new workers and receives collected materials.
	/// </summary>
	class BaseCenter : Building
	{
		BaseCenter(Position position) : base(position)
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
					() => new Worker(this.Position)
				);
			}
			return false;
		}
	}
}
