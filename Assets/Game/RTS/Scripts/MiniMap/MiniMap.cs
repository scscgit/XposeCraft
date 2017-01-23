using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour {
	public MiniMapElement[] elements = new MiniMapElement[0];
	public MiniMapElement cameraElement;
	public GameObject cam;
	public GameObject cameraController;
	public LayerMask camZoomLayer;
	public Texture background;
	public Rect localBounds;
	public Rect realWorldBounds;
	[HideInInspector]
	public Texture fogTexture;
	Vector2 lastPoint1;
	Vector2 lastPoint2;
	public Color fogColor = Color.clear;
	Rect size;
	Rect cameraLoc = new Rect(0,0,0,0);
	bool moveCamera = false;
	
	public void SetSize () {
		size = new Rect(localBounds.x/realWorldBounds.x, localBounds.y/realWorldBounds.y, localBounds.width/realWorldBounds.width, localBounds.height/realWorldBounds.height);
		for(int x = 0; x < elements.Length; x++){
			for(int y = 0; y < elements[x].objAmount; y++){
				elements[x].ModifyLoc(y, Determine2dLoc(elements[x].elementTransform[y].position));
			}
		}	
	}
	
	public void OnDrawGizmosSelected () {
		if(gameObject.name != "MiniMap"){
			gameObject.name = "MiniMap";
		}
		MiniMapElement backupElement;
		if(elements.Length > 1){
			for(int x = 1; x < elements.Length; x++){
				if(elements[x].moveUp){
					elements[x].moveUp = false;
					backupElement = elements[x-1];
					elements[x-1] = elements[x];
					elements[x] = backupElement;
				}
			}
		}
	}

	public bool AddElement (GameObject obj, string tag, MiniMapSignal map, int group) {
		bool found = false;
		int index = 0;
		for(int x = 0; x < elements.Length; x++){
			if(elements[x].tag == tag){
				index = x;
				found = true;
				break;
			}
		}
		if(found){
			elements[index].AddElement(obj, tag, map, group, Determine2dLoc(obj.transform.position));
			return true;
		}
		else{
			return false;
		}
	}

	void FixedUpdate () {
		for(int x = 0; x < elements.Length; x++){
			for (int y = 0; y < elements[x].objAmount; y++) {
				if(elements[x].elementObj[y] == null){
					elements[x].elementObj.RemoveAt(y);
					elements[x].elementMap.RemoveAt(y);
					elements[x].elementTransform.RemoveAt(y);
					elements[x].elementLoc.RemoveAt(y);
					elements[x].elementGroup.RemoveAt(y);
					elements[x].objAmount--;
					y--;
				}
			}
		}
		
		for(int x = 0; x < elements.Length; x++){
			for(int y = 0; y < elements[x].objAmount; y++){
				if(!elements[x].elementMap[y].isStatic){
					elements[x].elementLoc[y] = Determine2dLoc(elements[x].elementTransform[y].position);
				}
			}
		}
		
		//Vector2 loc = Determine2dLoc(cam.transform.position);
		MiniMapElement details = cameraElement;
		if(details.image){
			GUI.color = details.tints[0];
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(new Vector2(0, Screen.height));
			Physics.Raycast(ray, out hit, 1000, camZoomLayer);
			bool check = true;
			if(hit.collider == null){
				check = false;
			}
			RaycastHit hit2;
			ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width, 0));
			Physics.Raycast(ray, out hit2, 1000, camZoomLayer);
			if(hit2.collider == null){
				check = false;
			}
			if(check){
				lastPoint1 = Determine2dLoc(hit.point);
				lastPoint2 = Determine2dLoc(hit2.point);	
			}
			cameraLoc = new Rect(lastPoint1.x, lastPoint1.y, lastPoint2.x-lastPoint1.x, lastPoint2.y-lastPoint1.y);
			if(moveCamera){
				Vector3 pos = Determine3dLoc(new Vector2(Input.mousePosition.x+cameraLoc.width/4, Screen.height-Input.mousePosition.y+cameraLoc.height/2));
				cameraController.transform.position = new Vector3(pos.x, cameraController.transform.position.y, pos.z);
			}
		}
		
	}

	void OnGUI () {
		
		// Saves a lot of processing
		useGUILayout = false;
		GUI.depth = 0;
		moveCamera = false;
		// For Moving around the Camera
		if (GUI.RepeatButton (localBounds, "")) {
				if (Input.GetButton ("LMB")) {
						moveCamera = true;
				}
		}
		// Draw the Map Background
		if(background != null){
			GUI.DrawTexture(localBounds, background);
		}

		Vector2 loc;
		MiniMapElement details;
		Vector2 size = Vector2.zero;
		// Draw the Elements on the Map 
		for(int x = 0; x < elements.Length; x++){
			details = elements[x];
			size = new Vector2(details.size.x/2, details.size.y/2);
			for(int y = 0; y < elements[x].objAmount; y++){
				loc = elements[x].elementLoc[y];
				if(elements[x].elementMap[y].display){
					GUI.color = details.tints[elements[x].elementGroup[y]];
					GUI.DrawTexture(new Rect(loc.x-size.x, loc.y-size.y, details.size.x, details.size.y), details.image);
				}
			}
		}

		// Drawing the fog
		GUI.color = fogColor;
		GUI.DrawTexture(localBounds, fogTexture);
		details = cameraElement;
		if(details.image){
			// Draw the Camera
			GUI.DrawTexture(cameraLoc, details.image, ScaleMode.StretchToFill);
		}
	}

	// Determines the Location on the MiniMap for the loc
	Vector2 Determine2dLoc (Vector3 loc){
		loc = new Vector2((loc.x-realWorldBounds.x)*size.width+localBounds.x, localBounds.y+localBounds.height-(loc.z-realWorldBounds.y)*size.height);
		return loc;
	}
	
	Vector3 Determine3dLoc (Vector2 loc){
		Rect size = new Rect(realWorldBounds.x/localBounds.x, realWorldBounds.y/localBounds.y, realWorldBounds.width/localBounds.width, realWorldBounds.height/localBounds.height);
		Vector3 lLoc = new Vector3((loc.x-localBounds.x)*size.width, 100, realWorldBounds.y+realWorldBounds.height-(loc.y-localBounds.y)*size.height);
		return lLoc;
	}
}