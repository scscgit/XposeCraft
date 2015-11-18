using UnityEngine;
using UnityEditor;
using System.Collections;

public class UGridFindPoint : EditorWindow {
	
	int loc = 0;
	int index = 0;
	UGrid grid = null;
	GameObject findPoint;
	DrawCube pointCube;
	
	[MenuItem("Window/Grid Find Point")]
	static void Init () {
		UGridFindPoint window = (UGridFindPoint)EditorWindow.GetWindow (typeof (UGridFindPoint));
	}
	
	void OnGUI () {
		if(findPoint == null){
			findPoint = new GameObject();
			findPoint.name = "Find Point";
			pointCube = findPoint.AddComponent<DrawCube>();
		}
		if(grid == null){
			grid = GameObject.Find("UGrid").GetComponent<UGrid>();
		}
		index = EditorGUILayout.IntField("Grid : ", index);
		if(index < grid.grids.Length){
			loc = EditorGUILayout.IntField("Index : ", loc);
			if(loc < grid.grids[index].grid.Length){
				GUILayout.Label("X : " + grid.grids[index].grid[loc].loc.x + 
								", Y : " + grid.grids[index].grid[loc].loc.y + 
								", Z : " + grid.grids[index].grid[loc].loc.z);
			}
		}
		if(index < grid.grids.Length){
			if(loc < grid.grids[index].grid.Length){
				findPoint.transform.position = grid.grids[index].grid[loc].loc;
				pointCube.index = index;
			}
		}
	}
}
