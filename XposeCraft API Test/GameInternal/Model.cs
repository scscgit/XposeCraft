using System;
using System.Collections.Generic;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Materials;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.GameInternal
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
