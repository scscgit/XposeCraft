using UnityEngine;
using UnityEngine.Serialization;
using XposeCraft.Core.Required;

namespace XposeCraft.Core.Faction
{
    public class FactionManager : MonoBehaviour
    {
        [FormerlySerializedAs("selectionTexture")] public Texture SelectionTexture;
        [FormerlySerializedAs("groupList")] public GameObject[] FactionList = new GameObject[0];
        [FormerlySerializedAs("types")] public UnitType[] UnitTypes = new UnitType[0];

        void OnDrawGizmosSelected()
        {
            if (gameObject.name != "Faction Manager")
            {
                gameObject.name = "Faction Manager";
            }
        }
    }
}
