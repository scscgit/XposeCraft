using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors
{
	abstract class Actor : IActor
	{
		protected Actor(Position position)
		{
			this.Position = position;
		}

		public Position Position
		{
			get; protected set;
		}
	}
}
