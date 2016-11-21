using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game.Enum
{
	public class PlaceType
	{
		public static Position NearBase { get; } = new Position();
		public static Position UnderBaseRamp { get; } = new Position();
		public static Position EnemyBaseCenter { get; } = new Position();
		public static Position EnemyBasePositionLeft { get; } = new Position();
		public static Position EnemyBasePositionFront { get; } = new Position();
		public static Position EnemyBasePositionRight { get; } = new Position();
		public static Position EnemyBasePositionBack { get; } = new Position();
	}
}
