using UnityEngine;
using System.Collections;

public class BuildingGUI : MonoBehaviour {

	public Group groupManager;
	public Rect guiSize;
	public BuildingPlacement place;

	void OnGUI () {
		int y = 0;
		int z = 0;
		for(int x = 0; x < groupManager.buildingList.Length; x++){
			if(GUI.Button(new Rect(guiSize.x+z*guiSize.width, guiSize.y+y*guiSize.height, guiSize.width, guiSize.height), groupManager.buildingList[x].obj.GetComponent<BuildingController>().name)){
				place.BeginPlace(groupManager.buildingList[x]);
			}
			z = z + 1;
			if(z == y){
				y++;
				z = 0;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
