using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors.Materials
{
	abstract class Material : Actor, IMaterial
	{
		protected Material(Position position) : base(position)
		{
		}
	}
}
