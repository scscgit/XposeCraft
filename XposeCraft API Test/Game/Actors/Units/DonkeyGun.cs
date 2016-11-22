using System;
using System.Collections.Generic;

namespace XposeCraft.Game.Actors.Units
{
	class DonkeyGun : Unit
	{
		static readonly int MAX_HP = 120;

		public DonkeyGun(Position position) : base(position, MAX_HP)
		{
		}
	}
}
