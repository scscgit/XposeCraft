namespace XposeCraft.Game.Enums
{
	/// <summary>
	/// Game Events that can occur at any time during a gameplay and can be "hooked" with custom executable actions.
	/// </summary>
	public enum EventType
	{
		MineralsChanged,
		UnitCreated,
		BuildingCreated,
		EnemyUnitsOnSight,
		EnemyBuildingsOnSight,
		UnitReceivedFire,
		BuildingReceivedFire,
		UnitGainedHealth,
	}
}
