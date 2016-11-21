using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors.Units
{
	class DonkeyGun : Unit
	{
		public static readonly int MAX_HP = 120;

		public DonkeyGun(Position position) : base(position, MAX_HP)
		{
		}
	}
}
