using UnityEngine;

namespace XposeCraft.Core.Faction.Units
{
    public class RangeSignal : MonoBehaviour
    {
        public int type;
        public UnitController cont;
        UnitController script;

        void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Unit"))
            {
                script = coll.gameObject.GetComponent<UnitController>();
                int state = cont.DetermineRelations(script.FactionIndex);
                if (state == 2)
                {
                    cont.SphereSignal(type, coll.gameObject);
                }
            }
        }
    }
}
