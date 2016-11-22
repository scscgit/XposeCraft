using System;
using System.Collections.Generic;

namespace XposeCraft.Game.Actors.Materials
{
	abstract class Material : Actor, IMaterial
	{
		protected Material(Position position) : base(position)
		{
		}
	}
}
