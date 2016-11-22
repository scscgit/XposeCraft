using System;
using System.Collections.Generic;

namespace XposeCraft.Game
{
	public class Position
	{
		public static bool operator <(Position left, Position right)
		{
			return true;
		}

		public static bool operator >(Position left, Position right)
		{
			return false;
		}
	}
}
