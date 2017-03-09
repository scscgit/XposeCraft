using System;
using System.Collections.Generic;

namespace XposeCraft.Game.Actors.Buildings
{
	abstract class Building : Actor, IBuilding
	{
		public bool IsFinished
		{
			get; set;
		} = false;

		protected Building(Position position) : base(position)
		{
		}
	}
}
