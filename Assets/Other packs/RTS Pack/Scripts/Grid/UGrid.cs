using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UGrid : MonoBehaviour {

	public Grid[] grids = new Grid[1];
	public bool generate = false;
	public int index = 0;
	public APath pathfinding;
	public Texture selectionTexture;
	
	// Use this for initialization
	void OnDrawGizmos () {
		if(gameObject.name != "UGrid"){
			gameObject.name = "UGrid";
		}
		if(generate){
			GenerateGrid(index);
			generate = false;
		}
		if(index < grids.Length){
			grids[index].DisplayGrid(0);
		}
	}
	
	public void GenerateGrid (int i){
	    // Generating the grid is a multi-method process
		grids[i].grid = new GridPoint[grids[i].size*grids[i].size];
		for(int y = 0; y < grids[i].size; y++){
			for(int x = 0; x < grids[i].size; x++){
				grids[i].grid[x+(y*grids[i].size)] = new GridPoint(new Vector3(grids[i].startLoc.x + x*grids[i].nodeDist, grids[i].startLoc.y, grids[i].startLoc.z + y*grids[i].nodeDist));
				grids[i].grid[x+(y*grids[i].size)].index = x+(y*grids[i].size);
			}
		}
		if(grids[i].displaceDown)
			DisplacePoints(i);
			
		AssignChildren(i);
		
		CheckState(i);
	}
    
    // For Edges, children are assigned to each point
    // If octile connection = false then just 4 children will be assigned, otherwise 8
	public void AssignChildren (int i){
		for(int x = 0; x < grids[i].grid.Length; x++){
			int childAmount = 0;
			bool[] child = new bool[8];
			// We First determine the amount of children necessary
			if(x-1 > 0 && x%grids[i].size != 0){
				child[0] = true;
				childAmount++;
				if(grids[i].checkSlope){
					if(Mathf.Abs(grids[i].grid[x].loc.y - grids[i].grid[x-1].loc.y) > grids[i].slopeMax){
						for(int z = 0; z < child.Length; z++){
							child[z] = false;
							childAmount--;
						}
					}
				}
			}
			if(x+1 < grids[i].grid.Length && (x+1)%grids[i].size != 0){
				child[1] = true;
				childAmount++;
				if(grids[i].checkSlope){	
					if(Mathf.Abs(grids[i].grid[x].loc.y - grids[i].grid[x+1].loc.y) > grids[i].slopeMax){
						for(int z = 0; z < child.Length; z++){
							child[z] = false;
							childAmount--;
						}
					}
				}
			}
			if(x-grids[i].size > 0){
				child[2] = true;
				childAmount++;
				if(grids[i].checkSlope){	
					if(Mathf.Abs(grids[i].grid[x].loc.y - grids[i].grid[x-grids[i].size].loc.y) > grids[i].slopeMax){
						for(int z = 0; z < child.Length; z++){
							child[z] = false;
							childAmount--;
						}
					}
				}
			}
			if(x+grids[i].size < grids[i].grid.Length){
				child[3] = true;
				childAmount++;
				if(grids[i].checkSlope){	
					if(Mathf.Abs(grids[i].grid[x].loc.y - grids[i].grid[x+grids[i].size].loc.y) > grids[i].slopeMax){
						for(int z = 0; z < child.Length; z++){
							child[z] = false;
							childAmount--;
						}
					}
				}
			}
			if(grids[i].octileConnection){
				if(x-grids[i].size-1 > 0 && x%grids[i].size != 0){
					child[4] = true;
					childAmount++;
					if(grids[i].checkSlope){	
						if(Mathf.Abs(grids[i].grid[x].loc.y - grids[i].grid[x-grids[i].size-1].loc.y) > grids[i].slopeMax){
							for(int z = 0; z < child.Length; z++){
								child[z] = false;
								childAmount--;
							}
						}
					}
				}
				if(x-grids[i].size+1 > 0 && (x+1)%grids[i].size != 0){
					child[5] = true;
					childAmount++;
					if(grids[i].checkSlope){	
						if(Mathf.Abs(grids[i].grid[x].loc.y - grids[i].grid[x-grids[i].size+1].loc.y) > grids[i].slopeMax){
							for(int z = 0; z < child.Length; z++){
								child[z] = false;
								childAmount--;
							}
						}
					}
				}
				if(x-1+grids[i].size < grids[i].grid.Length && x%grids[i].size != 0){
					child[6] = true;
					childAmount++;
					if(grids[i].checkSlope){	
						if(Mathf.Abs(grids[i].grid[x].loc.y - grids[i].grid[x-1+grids[i].size].loc.y) > grids[i].slopeMax){
							for(int z = 0; z < child.Length; z++){
								child[z] = false;
								childAmount--;
							}
						}
					}
				}
				if(x+grids[i].size+1 < grids[i].grid.Length && (x+1)%grids[i].size != 0){
					child[7] = true;
					childAmount++;
					if(grids[i].checkSlope){	
						if(Mathf.Abs(grids[i].grid[x].loc.y - grids[i].grid[x+grids[i].size+1].loc.y) > grids[i].slopeMax){
							for(int z = 0; z < child.Length; z++){
								child[z] = false;
								childAmount--;
							}
						}
					}
				}	
			}
			int y = 0;
			// Then we add those children to the point
			if(childAmount > 0){
				grids[i].grid[x].children = new int[childAmount];
				if(child[0]){
					grids[i].grid[x].children[y] = x-1;
					y++;
				}
				if(child[1]){
					grids[i].grid[x].children[y] = x+1;
					y++;
				}
				if(child[2]){
					grids[i].grid[x].children[y] = x-grids[i].size;
					y++;
				}
				if(child[3]){
					grids[i].grid[x].children[y] = x+grids[i].size;
					y++;
				}
				// Octile Generation
				if(grids[i].octileConnection){
					if(child[4]){
						grids[i].grid[x].children[y] = x-grids[i].size-1;
						y++;
					}
					if(child[5]){
						grids[i].grid[x].children[y] = x-grids[i].size+1;
						y++;
					}
					if(child[6]){
						grids[i].grid[x].children[y] = x-1+grids[i].size;
						y++;
					}
					if(child[7]){
						grids[i].grid[x].children[y] = x+grids[i].size+1;
						y++;
					}
				}
			}
			else{
				grids[i].grid[x].children = new int[0];
			}
		}
	}
	
	// Sets gridpoints y location for grid[i] equal to closest downward collision point within the proper layer
	public void DisplacePoints (int i) {
		for(int x = 0; x < grids[i].grid.Length; x++){
			RaycastHit hit = new RaycastHit();
			Physics.Raycast(grids[i].grid[x].loc, Vector3.down, out hit, 10000, grids[i].displaceLayer);
			if(hit.collider)
				grids[i].grid[x].loc = new Vector3(grids[i].grid[x].loc.x, hit.point.y, grids[i].grid[x].loc.z);
    	}
	}
	
	public void CheckState (int i){
		// TODO have grid automate open/closed state
		for(int x = 0; x < grids[i].grid.Length; x++){
			Collider[] coll = Physics.OverlapSphere(grids[i].grid[x].loc, grids[i].nodeDist/2.25f, grids[i].checkLayer);
			if(coll.Length > 1)
				grids[i].grid[x].state = 2;
		}
		for(int x = 0; x < grids[i].grid.Length; x++){
			RaycastHit hit;
			hit = new RaycastHit();
			Physics.Raycast(new Vector3(grids[i].grid[x].loc.x, grids[i].grid[x].loc.y+100, grids[i].grid[x].loc.z), Vector3.down, out hit, 10000, grids[i].checkLayer);
			if(hit.collider)
				grids[i].grid[x].state = 2;
		}
	}
	

	public int DetermineLoc (Vector3 loc, int gridI) {
		float xLoc = (loc.x-grids[gridI].startLoc.x);
		float yLoc = (loc.z-grids[gridI].startLoc.z);
		int x = Mathf.RoundToInt(xLoc/grids[gridI].nodeDist);
		int y = Mathf.RoundToInt(yLoc/grids[gridI].nodeDist);
		int nLoc = x+(y*grids[gridI].size);
		return nLoc;
	}

	public Vector3 DetermineNearestPoint (Vector3 startPoint, Vector3 point, int i){
		int loc = DetermineLoc(point, i);
		if(grids[i].grid[loc].state != 2){
			return grids[i].grid[loc].loc;
		}
		else{
			Vector3 nLoc = FindNearestPoint(startPoint, loc, i, Mathf.Infinity);
			return nLoc;
		}
		
	}
	
	public Vector3 FindNearestPoint (Vector3 startPoint, int point, int i, float dist){
		bool found = false;
		List<int> children = new List<int>();
		int childAmount = 1;
		children.Add(point);
		Vector3 finalLoc = Vector3.zero;
		int finalIndex = 0;
		int z = 0;
		bool[] checkList = new bool[grids[i].grid.Length];
		while(!found || childAmount > 0){
			for(int x = 0; x < grids[i].grid[children[0]].children.Length; x++){
				z++;
				if(grids[i].grid[grids[i].grid[children[0]].children[x]].state != 2){
					if(!found){
						found = true;
						Vector3 nLoc = grids[i].grid[grids[i].grid[children[0]].children[x]].loc;
						float curDist = (startPoint-nLoc).sqrMagnitude;
						dist = curDist;
						finalLoc = nLoc;
						finalIndex = grids[i].grid[children[0]].children[x];
					}
					else{
						Vector3 nLoc = grids[i].grid[grids[i].grid[children[0]].children[x]].loc;
						float curDist = (startPoint-nLoc).sqrMagnitude;
						if(curDist < dist){
							dist = curDist;
							finalIndex = grids[i].grid[children[0]].children[x];
							finalLoc = nLoc;
						}
					}
				}
				else{
					if(!found && !checkList[grids[i].grid[children[0]].children[x]]){
						children.Add(grids[i].grid[children[0]].children[x]);
						checkList[grids[i].grid[children[0]].children[x]] = true;
						childAmount++;
					}
				}
			}
			children.RemoveAt(0);
			childAmount--;
		}
		return finalLoc;
	}
}

[System.Serializable]
public class Grid { 
	// base grid information
	public int size;
	public GridPoint[] grid;
	public Vector3 startLoc;
	public bool octileConnection = false;
	public float nodeDist;
	// specific grid options
	public bool displayGrid = false;
	public bool displayLines = false;
	public bool displaceDown = true;
	public bool checkSlope = false;
	public float slopeMax = 5;
	public LayerMask checkLayer;
	public LayerMask displaceLayer;

	public void DisplayGrid (int depth) {
		if(displayGrid){
			for(int x = 1; x < grid.Length; x++){
				if(grid[x].state == 2)
					Gizmos.color = Color.red;
				else
					Gizmos.color = Color.green;
				Gizmos.DrawCube(new Vector3(grid[x].loc.x, grid[x].loc.y, grid[x].loc.z), new Vector3(nodeDist, nodeDist, nodeDist));
			}
		}
		if(displayLines){
			for(int x = 0; x < grid.Length; x++){
				for(int y = 0; y < grid[x].children.Length; y++){
					if(grid[x].state == 2){
						Gizmos.color = Color.red;
					}
					else{
						Gizmos.color = Color.green;
						Gizmos.DrawLine(new Vector3(grid[x].loc.x, grid[x].loc.y, grid[x].loc.z), new Vector3(grid[grid[x].children[y]].loc.x, grid[grid[x].children[y]].loc.y, grid[grid[x].children[y]].loc.z));
					}
				}
			}
		}
	}	
}

[System.Serializable]
public class GridPoint {
	public Vector3 loc = Vector3.zero;
	//public bool check = false;
	//public bool added = false;
	//public float g_cost = 0;
	//public float f_cost = 0;
	public int parent;
	public int state = 0;
	// State 0 = Open
	// State 1 = Walkable
	// State 2 = Closed
	public bool unit = false;
	// Convert to INT
 	public int[] children;
	//public int[] sector;
	public int index = 0;
	
	public GridPoint () {
	}
	
	public GridPoint (Vector3 nLoc) {
		loc = nLoc;
	}
	
	
	public GridPoint (GridPoint gridPoint){
		loc = gridPoint.loc;
		state = gridPoint.state;
		children = gridPoint.children;
		index = gridPoint.index;
	}
}