using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors;

namespace XposeCraft_UI_API_Prototype_Test.GameInternal
{
	/// <summary>
	/// Data structures used within a game: collections etc.
	/// </summary>
	public class Model
	{
		public static Model Instance
		{
			get;
		} = new Model();

		public IList<Unit> Units
		{
			get; set;
		}
	}
}
