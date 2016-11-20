using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Buildings;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Materials;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;

namespace XposeCraft_UI_API_Prototype_Test.GameInternal
{
	/// <summary>
	/// Data structures used within a game: collections etc.
	/// </summary>
	public class Model
	{
		/// <summary>
		/// Instance of the game Model
		/// </summary>
		public static Model Instance
		{
			get;
		} = new Model();

		/// <summary>
		/// In-game Actors
		/// </summary>

		public IList<IUnit> Units
		{
			get; set;
		} = new List<IUnit>();

		public IList<IBuilding> Buildings
		{
			get; set;
		} = new List<IBuilding>();

		public IList<IMaterial> Materials
		{
			get; set;
		} = new List<IMaterial>();

		/// <summary>
		/// Currencies of the player
		/// </summary>

		public int Minerals
		{
			get; set;
		} = 80;
	}
}
