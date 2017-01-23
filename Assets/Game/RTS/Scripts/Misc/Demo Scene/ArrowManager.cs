using UnityEngine;
using System.Collections;

public class ArrowManager : MonoBehaviour {

	public Arrow[] arrows;
	
	public void Attack (GameObject obj) {
		for(int x = 0; x < arrows.Length; x++){
			if(arrows[x].landed){
				arrows[x].transform.position = transform.position;
				arrows[x].landed = false;
				arrows[x].Attack(obj);
				arrows[x].gameObject.SetActive(true);
				break;
			}
		}
	}
}
