using UnityEngine;
using System.Collections;

public class GroupManager : MonoBehaviour {
	[HideInInspector]
	public Texture selectionTexture;
	[HideInInspector]
	public GameObject[] groupList = new GameObject[0];
	[HideInInspector]
	public Type[] types = new Type[0];
	
	// Use this for initialization
	void OnDrawGizmosSelected () {
		if(gameObject.name != "Faction Manager"){
			gameObject.name = "Faction Manager";
		}
	}
}