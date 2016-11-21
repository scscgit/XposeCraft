using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game
{
	class Position
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
