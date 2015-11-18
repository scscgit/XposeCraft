using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class UnitMovement : MonoBehaviour {
	Group group;
	public int speed;
	public int rotateSpeed;
	public Vector3 target;
	public UGrid gridScript;
	public int gridI;
	public UPath myPath;
	public int depth = 0;
	public bool pathComplete = false;
	int curPoint = 0;
	public AStarManager pathing;
	public float checkDist = 1;
	public int layer;
	bool originalState = false;
	int lastLoc = 0;
	Transform myTransform;
	
	void Start () {
		Physics.IgnoreLayerCollision(layer, layer, true);
		pathing = GameObject.Find("A*").GetComponent<AStarManager>();
		gridScript = GameObject.Find("UGrid").GetComponent<UGrid>();
		myTransform = GetComponent<Transform>();
	}

	// Update is called once per frame
	void FixedUpdate () {
		if(!pathComplete){
			if(myPath != null){
				if(myPath.list.Length > 0){
					bool skip = false;
					for(int x = 1; x < myPath.list.Length-1; x++){
						if(gridScript.grids[gridI].grid[myPath.list[x]].state == 2){
							GetPath(target);
							skip = true;
							break;
						}
					}
					if(!skip){
						// Lerp Rotation
						Quaternion targetRotation = Quaternion.LookRotation(new Vector3(gridScript.grids[gridI].grid[myPath.list[curPoint]].loc.x, 0, gridScript.grids[gridI].grid[myPath.list[curPoint]].loc.z) - 
																			new Vector3(myTransform.position.x, 0, myTransform.position.z));
						myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
						myTransform.Translate(Vector3.forward*speed*Time.deltaTime);
						float distFromPlace = (new Vector3(gridScript.grids[gridI].grid[myPath.list[curPoint]].loc.x, 0, gridScript.grids[gridI].grid[myPath.list[curPoint]].loc.z) - 
											   new Vector3(myTransform.position.x, 0, myTransform.position.z)).sqrMagnitude;
						if(distFromPlace < checkDist){
							curPoint++;
							if(curPoint == myPath.list.Length){
								pathComplete = true;
							}
						}
					}
				}
			}
		}
	}

	public void GetPath (Vector3 target){
		myPath = null;
		pathing.RequestPath(myTransform.position, target, gameObject, gridI);
		curPoint = 0;
		pathComplete = false;
	}
	
	void OnDrawGizmosSelected () {
		if(myPath != null){
			if(myPath.list.Length > 0){
				myPath.color = Color.green;
				myPath.DisplayPath(curPoint, gridScript.grids[gridI].grid, gridScript.grids[gridI].nodeDist);
			}
		}
	}
	void DetermineGroup () {
		
	}
}