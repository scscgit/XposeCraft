namespace XposeCraft.Game.Enums
{
    /// <summary>
    /// Game Events that can occur at any time during a gameplay and can be "hooked" with custom executable actions.
    /// </summary>
    public enum GameEventType
    {
        MineralsChanged,
        UnitProduced,
        BuildingStartedConstruction,
        BuildingCreated,
        EnemyUnitsOnSight,
        EnemyBuildingsOnSight,
        UnitReceivedFire,
        BuildingReceivedFire,

        // Currently not implemented; will work slowly over time, near buildings or by a specific healer unit
        UnitGainedHealth,
    }
}
