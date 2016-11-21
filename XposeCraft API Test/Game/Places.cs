using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game
{
	class Places
	{
		public Position NearBase { get; } = new Position();
		public Position UnderBaseRamp { get; } = new Position();
		public Position EnemyBaseCenter { get; } = new Position();
		public Position EnemyBasePositionLeft { get; } = new Position();
		public Position EnemyBasePositionFront { get; } = new Position();
		public Position EnemyBasePositionRight { get; } = new Position();
		public Position EnemyBasePositionBack { get; } = new Position();
	}
}
