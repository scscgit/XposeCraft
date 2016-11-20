using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Materials;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors.Units
{
	/// <summary>
	/// Can gather various materials (mainly minerals) and build various buildings - based on a current API level.
	/// </summary>
	class Worker : Unit
	{
		public Worker(Position position) : base(position)
		{
		}

		public IMaterial Gathering
		{
			get; private set;
		} = null;

		public void SendGather(IMaterial material)
		{
			Gathering = material;
		}
	}
}
