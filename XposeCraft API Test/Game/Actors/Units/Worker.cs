using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game.Actors.Units
{
	/// <summary>
	/// Can gather various materials (mainly minerals) and build various buildings - based on a current API level.
	/// </summary>
	public class Worker : Unit
	{
		public bool IsGathering
		{
			get; private set;
		} = false;

		public bool SendGather()
		{
			return IsGathering = true;
		}
	}
}
