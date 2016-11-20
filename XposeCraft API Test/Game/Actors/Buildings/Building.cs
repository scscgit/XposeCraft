using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors.Buildings
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
