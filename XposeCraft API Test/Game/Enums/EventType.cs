using System.Collections.Generic;

namespace XposeCraft_UI_API_Prototype_Test.Game.Enums
{
	/// <summary>
	/// Game Events that can occur at any time during a gameplay and can be "hooked" with custom executable actions.
	/// </summary>
	enum EventType
	{
		MineralsChanged,
		UnitCreated,
		BuildingCreated,
		EnemyOnSight,
	}
}
