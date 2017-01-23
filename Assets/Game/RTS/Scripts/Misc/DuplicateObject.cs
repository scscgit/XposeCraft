using UnityEngine;
using System.Collections;

public class DuplicateObject : MonoBehaviour {

	public GameObject obj;
	public Vector2 dispAmount;
	public int xMax;
	public int zMax;
	public string objName;
	public bool generate;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(generate){
			GameObject parent = new GameObject();
			parent.name = "Parent";
			int y = 0;
			for(int z = 0; z < zMax; z++){
				for(int x = 0; x < xMax; x++){
					RaycastHit hit;
					Physics.Raycast(new Vector3(gameObject.transform.position.x+x*dispAmount.x, gameObject.transform.position.y+100, gameObject.transform.position.z+z*dispAmount.y), Vector3.down, out hit, 1000);
					GameObject clone = Instantiate(obj, new Vector3(hit.point.x, hit.point.y+1, hit.point.z), Quaternion.identity) as GameObject;
					clone.name = objName + " " + y;
					clone.transform.parent = parent.transform;
					y++;
				}
			}
			generate = false;
		}
	}
}
