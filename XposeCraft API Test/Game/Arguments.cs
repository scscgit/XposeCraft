using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game
{
	class Arguments
	{
		public IDictionary<string, string> StringMap { get; private set; } = new Dictionary<string, string>();

		public int Minerals { get; set; }
	}
}
