using UnityEngine;
using System.Collections;

public class RangeSignal : MonoBehaviour {

	public int type = 0;
	public UnitController cont;
	UnitController script = null;
	
	void OnTriggerEnter (Collider coll) {
		if (coll.gameObject.CompareTag("Unit")) {
			script = coll.gameObject.GetComponent<UnitController>();
			int state = cont.DetermineRelations(script.group);
			if(state == 2){
				cont.SphereSignal(type, coll.gameObject);
			}
		}
	}
}
