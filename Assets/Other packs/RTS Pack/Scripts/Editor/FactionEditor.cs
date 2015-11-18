using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class FactionEditor : EditorWindow {
	
	GroupManager target;
	Group nTarget;
	Vector2 groupPosition;
	Vector2 groupPosition2;
	int groupState = 0;
	int groupId;
	string groupSearch = "Search";
	
	Vector2 unitPosition;
	string unitSearch = "Search";
	int unitState = 0;
	int unitId;
	GameObject lastObj;
	GUISkin skin;
	int menuState = 0;
	int subMenuState = 0;
	int arraySelect = 0;
	int arraySelect1 = 0;
	int arraySelect2 = 0;
	int arraySelect3 = 0;
	int miniMapState = 0;
	int helpState = 0;
	int techList = 0;
	bool techOpen;
	int maxY = 0;
	
	// The Data to display in the Help box
	string[] helpOptions = {/*0*/	"This is the help box, this is where you will find all the information for the individual options.", 
							/*1*/	"Please have at least 2 groups to work with their relations.", 
							/*2*/	"You cannot modify relations to yourself.", 
							/*3*/	"A Strength indicates an increased amount of damage to the target type, while a weakness indicates a reduced amount of damage to the target.",
	                        /*4*/   "The Technology Tree determines what techs your faction can research and what the state of those techs is at startup.",
	                        /*5*/   "The Global options define what stats your unit has.",
	                        /*6*/   "If you would like your unit to be able to fight, click the check box then set up his fighting settings",
	                        /*7*/   "If you would like your unit to be able to build, click the check box then set up his building settings per building type",
	                        /*8*/   "If you would like your unit to be able to gather resources, click the check box then set up his gathering settings per resource",
	                        /*9*/   "The GUI image is the image displayed when your object is selected, while the Objects field refers to objects to activate when the object is Selected.",
	                        /*10*/  "Here you can set up the unit's MiniMap GUI settings, although you will still need to change the layer on the MiniMap GameObject.",
	                        /*11*/  "Please add a new GameObject with the MiniMap component to the scene.",
	                        /*12*/  "Here is the Fog Of War editor, so that you can change the area of your unit's sight.",
	                        /*13*/  "The Close Width and Length refer to the gridpoints to close when the Building is placed. X = Close, W = Walkable, O = Open.",
	                        /*14*/  "The Drop Off Point is a building where your units can return to in order to place the resources they are carrying.",
	                        /*15*/  "",
	                        /*16*/  "",
	                        /*17*/  "",
	                        /*18*/  ""
	                        
	};
	Texture selectionTexture;
	Vector2 scrollPos;
	Editor objEditor;
	
	
	[MenuItem("Window/Faction Editor")]
	static void Init () {
		FactionEditor window = (FactionEditor)EditorWindow.GetWindow (typeof (FactionEditor));
	}
	
	public void Update(){
		Repaint();
	}
	
	void OnGUI () {
	    // Root GUI Display
		Vector2 defaultRect = new Vector2(1500, 750);
		Vector2 realRect = new Vector2(position.width, position.height);
		Vector2 rect = new Vector2((int)realRect.x/defaultRect.x, (int)realRect.y/defaultRect.y);
		if(!target){
			target = GameObject.Find("Faction Manager").GetComponent<GroupManager>();
			selectionTexture = target.selectionTexture;
		}
		helpState = 0;
		if(groupState == 0){
			DrawGroupGUI(rect);
		}
		else if(groupState == 1){
			if(unitState == 0){
				DrawUnitGUI(rect);
			}
			else{
				DrawBuildingGUI(rect);
			}
		}
		DrawSetup(rect);
	}
	
	void DrawSetup(Vector2 rect){
		if(groupId < 0){
			groupId = 0;
		}
		if(unitId < 0){
			unitId = 0;
		}
		string[] options = {"Faction Data", "Faction Objects"};
		groupState = EditorGUI.Popup(new Rect(0,0,200*rect.x,25*rect.y), groupState, options);
		if(target.groupList.Length*20*rect.x < 655*rect.x){
			groupPosition = GUI.BeginScrollView(new Rect(0,45*rect.y,200*rect.x,655*rect.y), groupPosition, new Rect(0, 0, 200*rect.x, 655*rect.y), false, true);
		}
		else{
			groupPosition = GUI.BeginScrollView(new Rect(0, 45*rect.y, 200*rect.x, 655*rect.y), groupPosition, new Rect(0, 0, 200*rect.x, target.groupList.Length*20*rect.y), false, true);
		}
		if(groupId >= target.groupList.Length){
			groupId = target.groupList.Length-1;
		}
		for(int x = 0; x < target.groupList.Length; x++){
			if(target.groupList[x]){
				if(isWithin(new Rect(0,x*20*rect.y, 200*rect.x, 20*rect.y))){
					groupId = x;
				}
				GUI.Label(new Rect(0,x*20*rect.y, 200*rect.x, 20*rect.y), target.groupList[x].name);
			}
			else{
				if(isWithin(new Rect(0,x*20*rect.y, 200*rect.x, 20*rect.y))){
					groupId = x;
				}
				target.groupList[x] = EditorGUI.ObjectField(new Rect(0,x*20*rect.y, 200*rect.x, 20*rect.y), target.groupList[x], typeof(GameObject), true) as GameObject;
			}
			if(groupId == x){
				GUI.DrawTexture(new Rect(0,x*20*rect.y, 200*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
			}
		}
		GUI.EndScrollView();
		if(target.groupList.Length > 0){
			if(target.groupList[groupId]){
				groupSearch = EditorGUI.TextField(new Rect(0,25*rect.y, 200*rect.x, 20*rect.y), groupSearch);
			}
			else{
				GUI.Box(new Rect(0,25*rect.y, 200*rect.x, 20*rect.y), "");
			}
		}
		if(GUI.Button(new Rect(0,700*rect.y, 100*rect.x, 50*rect.y), "+")){
			ModifyG(target.groupList.Length+1, target.groupList.Length, groupId);
		}
		if(GUI.Button(new Rect(100*rect.x,700*rect.y, 100*rect.x, 50*rect.y), "-")){
			ModifyG(target.groupList.Length-1, target.groupList.Length, groupId);
		}
		if(target.groupList.Length > 0){
			if(groupId >= target.groupList.Length){
				groupId = target.groupList.Length-1;
			}
			else if(target.groupList[groupId]){
				nTarget = target.groupList[groupId].GetComponent<Group>();
				if(nTarget){
					if(groupState == 1){
						unitSearch = EditorGUI.TextField(new Rect(200*rect.x,0,200*rect.x, 25*rect.y), unitSearch);
						
						if(GUI.Button(new Rect(200*rect.x,25*rect.y, 100*rect.x, 20*rect.y), "U")){
							unitState = 0;
						}
						if(GUI.Button(new Rect(300*rect.x,25*rect.y, 100*rect.x, 20*rect.y), "B")){
							unitState = 1;
						}
						if(unitState == 0){
							GUI.DrawTexture(new Rect(200*rect.x, 25*rect.y, 100*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
							if(nTarget.unitList.Length*20*rect.x < 655*rect.x){
								unitPosition = GUI.BeginScrollView(new Rect(200*rect.x, 45*rect.y, 200*rect.x, 655*rect.y), unitPosition, new Rect(0, 0, 200*rect.x, 655*rect.y), false, true);
							}
							else{
								unitPosition = GUI.BeginScrollView(new Rect(200*rect.x, 45*rect.y, 200*rect.x, 655*rect.y), unitPosition, new Rect(0, 0, 200*rect.x, nTarget.unitList.Length*20*rect.y), false, true);
							}
							if(unitId >= nTarget.unitList.Length){
								unitId = nTarget.unitList.Length-1;
							}
							for(int x = 0; x < nTarget.unitList.Length; x++){
								if(nTarget.unitList[x].obj != null){
									if(isWithin(new Rect(0*rect.x,x*20*rect.y, 200*rect.x, 20*rect.y))){
										unitId = x;
									}
									UnitController cont = nTarget.unitList[x].obj.GetComponent<UnitController>();
									if(cont){
										GUI.Label(new Rect(0*rect.x,x*20*rect.y, 200*rect.x, 20*rect.y), cont.name);
									}
									else{
										GUI.Label(new Rect(0*rect.x,x*20*rect.y, 200*rect.x, 20*rect.y), "New Unit");
									}
								}
								else{
									if(isWithin(new Rect(0*rect.x,x*20*rect.y, 200*rect.x, 20*rect.y))){
										unitId = x;
									}
									nTarget.unitList[x].obj = EditorGUI.ObjectField(new Rect(0*rect.x,x*20*rect.y, 200*rect.x, 20*rect.y), nTarget.unitList[x].obj, typeof(GameObject), true) as GameObject;
								}
								if(unitId == x){
									GUI.DrawTexture(new Rect(0*rect.x,x*20*rect.y, 200*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								}
							}
						}
						else if(unitState == 1){
							GUI.DrawTexture(new Rect(300*rect.x, 25*rect.y, 100*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
							if(nTarget.buildingList.Length*20*rect.x < 655*rect.x){
								unitPosition = GUI.BeginScrollView(new Rect(200*rect.x, 45*rect.y, 200*rect.x, 655*rect.y), unitPosition, new Rect(0, 0, 200*rect.x, 655*rect.y), false, true);
							}
							else{
								unitPosition = GUI.BeginScrollView(new Rect(200*rect.x, 45*rect.y, 200*rect.x, 655*rect.y), unitPosition, new Rect(0, 0, 200*rect.x, nTarget.buildingList.Length*20*rect.y), false, true);
							}
							if(unitId >= nTarget.buildingList.Length){
								unitId = nTarget.buildingList.Length-1;
							}
							for(int x = 0; x < nTarget.buildingList.Length; x++){
								if(nTarget.buildingList[x].obj != null){
									if(isWithin(new Rect(0,x*20*rect.y, 200*rect.x, 20*rect.y))){
										unitId = x;
									}
									BuildingController cont = nTarget.buildingList[x].obj.GetComponent<BuildingController>();
									if(cont){
										GUI.Label(new Rect(0,x*20*rect.y, 200*rect.x, 20*rect.y), cont.name);
									}
									else{
										GUI.Label(new Rect(0,x*20*rect.y, 200*rect.x, 20*rect.y), "New Building");
									}
								}
								else{
									if(isWithin(new Rect(0,x*20*rect.y, 200*rect.x, 20*rect.y))){
										unitId = x;
									}
									nTarget.buildingList[x].obj = EditorGUI.ObjectField(new Rect(0,x*20*rect.y, 200*rect.x, 20*rect.y), nTarget.buildingList[x].obj, typeof(GameObject), true) as GameObject;
								}
								if(unitId == x){
									GUI.DrawTexture(new Rect(0,x*20*rect.y, 200*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								}
							}
						}
						GUI.EndScrollView();
						if(GUI.Button(new Rect(200*rect.x,700*rect.y, 100*rect.x, 50*rect.y), "+")){
							if(unitState == 0){	
								ModifyGU(nTarget.unitList.Length+1, nTarget.unitList.Length, unitId);
							}
							else if(unitState == 1){
								ModifyGB(nTarget.buildingList.Length+1, nTarget.buildingList.Length, unitId);
							}
						}
						if(GUI.Button(new Rect(300*rect.x,700*rect.y, 100*rect.x, 50*rect.y), "-")){
							if(unitState == 0){
								ModifyGU(nTarget.unitList.Length-1, nTarget.unitList.Length, unitId);
								unitId--;
							}
							else if(unitState == 1){
								ModifyGB(nTarget.buildingList.Length-1, nTarget.buildingList.Length, unitId);
								unitId--;
							}
						}
					}
				}
				else{
					if(GUI.Button(new Rect(200*rect.x,0*rect.y,200*rect.x,750*rect.y), "+")){
						target.groupList[groupId].AddComponent("Group");
					}
				}
			}
		}
		if(groupState == 0){
			GUI.Box(new Rect(200*rect.x, 700*rect.y, 1300*rect.x, 50*rect.y), helpOptions[helpState]);
		}
		else{
			GUI.Box(new Rect(400*rect.x, 700*rect.y, 1100*rect.x, 50*rect.y), helpOptions[helpState]);
		}
	}

	void DrawGroupGUI (Vector2 rect) {
		GUI.Box(new Rect(200*rect.x, 25*rect.y, 1300*rect.x, 50*rect.y), "");
		if(GUI.Button(new Rect(200*rect.x, 0, 325*rect.x, 25*rect.y), "Global")){
			menuState = 0;
			subMenuState = 0;
		}
		if(GUI.Button(new Rect(525*rect.x, 0, 325*rect.x, 25*rect.y), "Relations")){
			menuState = 1;
			subMenuState = 0;
		}
		if(GUI.Button(new Rect(850*rect.x, 0, 325*rect.x, 25*rect.y), "Techs")){
			menuState = 2;
			subMenuState = 0;
		}
		if(GUI.Button(new Rect(1175*rect.x, 0, 325*rect.x, 25*rect.y), "Unit Types")){
			menuState = 3;
			subMenuState = 0;
		}
		// Global
		if(menuState == 0){
			GUI.DrawTexture(new Rect(200*rect.x, 0, 325*rect.x, 25*rect.y), selectionTexture, ScaleMode.StretchToFill);
			if(groupId < 0){
				groupId = 0;
			}
			if(target.groupList.Length > 0){
				if(target.groupList[groupId] != null){
					target.groupList[groupId].name = EditorGUI.TextField(new Rect(200*rect.x, 95*rect.y, 1300*rect.x, 25*rect.y), "Name : ", target.groupList[groupId].name);
				}
			}
			else{
				groupId = 0;
			}
		}
		// Relations
		else if(menuState == 1){
			GUI.DrawTexture(new Rect(525*rect.x, 0, 325*rect.x, 25*rect.y), selectionTexture, ScaleMode.StretchToFill);
			if(nTarget.relations.Length != target.groupList.Length){
				ModifyGR(target.groupList.Length, nTarget.relations.Length);
			}
			else{
				if(target.groupList.Length == 1){
					helpState = 1;
				}
				else{
					string[] groupNames = new string[target.groupList.Length];
					int selfLocation = groupId;
					for(int x = 0; x < target.groupList.Length; x++){
						if(target.groupList[x] != null){
							groupNames[x] = (x+1) + ". " + target.groupList[x].name;
						}
					}
					arraySelect = EditorGUI.Popup(new Rect(200*rect.x, 95*rect.y, 1300*rect.x, 25*rect.y), "Faction : ", arraySelect, groupNames);
					if(arraySelect != selfLocation){
						string[] options = {"Ally", "Neutral", "Enemy"};
						nTarget.relations[arraySelect].state = EditorGUI.Popup(new Rect(200*rect.x, 120*rect.y, 1300*rect.x, 25*rect.y), "Relation : ", nTarget.relations[arraySelect].state, options);
					}
					else{
						helpState = 2;
					}
				}
			}
		}
		// Techs
		else if(menuState == 2){
			GUI.DrawTexture(new Rect(850*rect.x, 0, 325*rect.x, 25*rect.y), selectionTexture, ScaleMode.StretchToFill);
			helpState = 4;
			if(nTarget.tech.Length > 0){
				int size = nTarget.tech.Length;
				Technology[] techs;
				techs = nTarget.tech;
				if(maxY <= 620){
					groupPosition2 = GUI.BeginScrollView(new Rect(200*rect.x,80*rect.y,1300*rect.x,620*rect.y), groupPosition2, new Rect(0, 0, 200*rect.x, 655*rect.y), false, true);
				}
				else{
					groupPosition2 = GUI.BeginScrollView(new Rect(200*rect.x,80*rect.y,1300*rect.x,620*rect.y), groupPosition2, new Rect(0, 0, 200*rect.x, maxY*rect.y), false, true);
				}
				int curY = 0;
				if(GUI.Button(new Rect(0*rect.x, curY*rect.y, 650*rect.x, 20*rect.y), "Add")){
					nTarget.tech = ModifyT(nTarget.tech.Length+1, nTarget.tech.Length, nTarget.tech);
				}
				if(GUI.Button(new Rect(650*rect.x, curY*rect.y, 650*rect.x, 20*rect.y), "Remove")){
					nTarget.tech = ModifyT(nTarget.tech.Length-1, nTarget.tech.Length, nTarget.tech);
				}
				curY += 25;
				string[] techName = new string[techs.Length];
				for(int z = 0; z < techs.Length; z++){
					techName[z] = (z+1) + ". " + techs[z].name;
				}
				techList = EditorGUI.Popup(new Rect(0*rect.x, curY*rect.y, 1300*rect.x, 20*rect.y), "Tech : ", techList, techName);
				curY += 25;
				techOpen = EditorGUI.Foldout(new Rect(0*rect.x, curY*rect.y, 1300*rect.x, 20*rect.y), techOpen, "Info : ");
				curY += 25;
				if(techOpen){
					techs[techList].name = EditorGUI.TextField(new Rect(0*rect.x, curY*rect.y, 1300*rect.x, 20*rect.y), "Name : ", techs[techList].name);
					curY += 25;
					EditorGUI.LabelField(new Rect(0*rect.x, curY*rect.y, 1300*rect.x, 20*rect.y), "Texture : ");
					curY += 20;
					techs[techList].texture = EditorGUI.ObjectField(new Rect(0*rect.x, curY*rect.y, 100*rect.x, 100*rect.y), "", techs[techList].texture, typeof(Texture2D), true) as Texture2D;
					curY += 80;
					techs[techList].active = EditorGUI.Toggle(new Rect(0*rect.x, curY*rect.y, 1300*rect.x, 20*rect.y), "Active : ", techs[techList].active);
					curY += 25;
				}
				GUI.EndScrollView();
			}
			else{
				if(GUI.Button(new Rect(200*rect.x, 80*rect.y, 650*rect.x, 20*rect.y), "Add")){
					nTarget.tech = ModifyT(nTarget.tech.Length+1, nTarget.tech.Length, nTarget.tech);
				}
				if(GUI.Button(new Rect(850*rect.x, 80*rect.y, 650*rect.x, 20*rect.y), "Remove")){
					nTarget.tech = ModifyT(nTarget.tech.Length-1, nTarget.tech.Length, nTarget.tech);
				}
			}
		}
		// Unit Types
		else if(menuState == 3){
			helpState = 3;
			GUI.DrawTexture(new Rect(1175*rect.x, 0, 325*rect.x, 25*rect.y), selectionTexture, ScaleMode.StretchToFill);
			int curY = 95;
			if(target.types.Length > 0){
				string[] typeNames = new string[target.types.Length];
				for(int x = 0; x < target.types.Length; x++){
					typeNames[x] = (x+1) + ". " + target.types[x].name;
				}
				if(GUI.Button(new Rect(200*rect.x, curY*rect.y, 650*rect.x, 15*rect.y),"Add Type")){
					ModifyGT(target.types.Length+1, target.types.Length, arraySelect);
				}
				if(GUI.Button(new Rect(850*rect.x, curY*rect.y, 650*rect.x, 15*rect.y),"Remove Type")){
					ModifyGT(target.types.Length-1, target.types.Length, arraySelect);
				}
				curY += 20;
				arraySelect = EditorGUI.Popup(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 25*rect.y), "Type : ", arraySelect, typeNames);
				curY += 15;
				if(arraySelect >= target.types.Length || arraySelect < 0){
					arraySelect = 0;
				}
				target.types[arraySelect].name = EditorGUI.TextField(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 25*rect.y), "Type Name : ", target.types[arraySelect].name);
				curY += 40;
				if(target.types[arraySelect].strengths.Length > 0){
					string[] strengthName = new string[target.types[arraySelect].strengths.Length];
					if(arraySelect1 >= target.types[arraySelect].strengths.Length){
						arraySelect1 = target.types[arraySelect].strengths.Length-1;
					}
					for(int x = 0; x < target.types[arraySelect].strengths.Length; x++){
						strengthName[x] = (x+1) + ". " + target.types[arraySelect].strengths[x].name;
					}
					GUI.Box(new Rect(200*rect.x, curY*rect.y-3, 1300*rect.x, 130*rect.y), "");
					if(GUI.Button(new Rect(200*rect.x, curY*rect.y, 650*rect.x, 15*rect.y),"Add Strength")){
						ModifyGTS(target.types[arraySelect].strengths.Length+1, target.types[arraySelect].strengths.Length, arraySelect1, arraySelect);
					}
					if(GUI.Button(new Rect(850*rect.x, curY*rect.y, 650*rect.x, 15*rect.y),"Remove Strength")){
						ModifyGTS(target.types[arraySelect].strengths.Length-1, target.types[arraySelect].strengths.Length, arraySelect1, arraySelect);
					}
					curY += 20;
					arraySelect1 = EditorGUI.Popup(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 25*rect.y), "Strength : ", arraySelect1, strengthName);
					curY += 15;
					if(arraySelect1 >= target.types[arraySelect].strengths.Length || arraySelect1 < 0){
						arraySelect1 = 0;
					}
					if(target.types[arraySelect].strengths.Length > 0){
						target.types[arraySelect].strengths[arraySelect1].name = EditorGUI.TextField(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 25*rect.y), "Name : ", target.types[arraySelect].strengths[arraySelect1].name);
						curY += 30;
						target.types[arraySelect].strengths[arraySelect1].target = EditorGUI.Popup(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 25*rect.y), "Target : ", target.types[arraySelect].strengths[arraySelect1].target, typeNames);
						target.types[arraySelect].strengths[arraySelect1].targetName = typeNames[target.types[arraySelect].strengths[arraySelect1].target];
						curY += 30;
						target.types[arraySelect].strengths[arraySelect1].amount = EditorGUI.FloatField(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 25*rect.y), "Ratio : ", target.types[arraySelect].strengths[arraySelect1].amount);
						curY += 30;
					}
				}
				else{
					GUI.Box(new Rect(200*rect.x, curY*rect.y-3, 1300*rect.x, 26*rect.y), "");
					if(GUI.Button(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 15*rect.y),"Add Strength")){
						ModifyGTS(target.types[arraySelect].strengths.Length+1, target.types[arraySelect].strengths.Length, arraySelect1, arraySelect);
					}
					curY += 20;
				}
				curY += 10;
				if(target.types[arraySelect].weaknesses.Length > 0){
					string[] weaknessName = new string[target.types[arraySelect].weaknesses.Length];
					if(arraySelect2 >= target.types[arraySelect].weaknesses.Length || arraySelect2 < 0){
						arraySelect2 = 0;
					}
					for(int x = 0; x < target.types[arraySelect].weaknesses.Length; x++){
						weaknessName[x] = (x+1) + ". " + target.types[arraySelect].weaknesses[x].name;
					}
					GUI.Box(new Rect(200*rect.x, curY*rect.y-3, 1300*rect.x, 130*rect.y), "");
					if(GUI.Button(new Rect(200*rect.x, curY*rect.y, 650*rect.x, 15*rect.y),"Add Weakness")){
						ModifyGTW(target.types[arraySelect].weaknesses.Length+1, target.types[arraySelect].weaknesses.Length, arraySelect2, arraySelect);
					}
					if(GUI.Button(new Rect(850*rect.x, curY*rect.y, 650*rect.x, 15*rect.y),"Remove Weakness")){
						ModifyGTW(target.types[arraySelect].weaknesses.Length-1, target.types[arraySelect].weaknesses.Length, arraySelect2, arraySelect);
					}
					curY += 20;
					arraySelect2 = EditorGUI.Popup(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 25*rect.y), "Weakness : ", arraySelect2, weaknessName);
					curY += 15;
					target.types[arraySelect].weaknesses[arraySelect2].name = EditorGUI.TextField(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 25*rect.y), "Name : ", target.types[arraySelect].weaknesses[arraySelect2].name);
					curY += 30;
					target.types[arraySelect].weaknesses[arraySelect2].target = EditorGUI.Popup(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 25*rect.y), "Target : ", target.types[arraySelect].weaknesses[arraySelect2].target, typeNames);
					curY += 30;
					target.types[arraySelect].weaknesses[arraySelect2].amount = EditorGUI.FloatField(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 25*rect.y), "Ratio : ", target.types[arraySelect].weaknesses[arraySelect2].amount);
					curY += 30;
				}
				else{
					GUI.Box(new Rect(200*rect.x, curY*rect.y-3, 1300*rect.x, 26*rect.y), "");
					if(GUI.Button(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 15*rect.y),"Add Weakness")){
						ModifyGTW(target.types[arraySelect].weaknesses.Length+1, target.types[arraySelect].weaknesses.Length, arraySelect2, arraySelect);
					}
					curY += 20;
				}
			}
			else{
				if(GUI.Button(new Rect(200*rect.x, curY*rect.y, 1300*rect.x, 25*rect.y),"+")){
					ModifyGT(target.types.Length+1, target.types.Length, arraySelect);
				}
			}
		}
	}
	
	void DrawUnitGUI (Vector2 rect) {
		if(nTarget != null){
			if(unitId >= nTarget.unitList.Length || unitId < 0){
				unitId = 0;
			}
			else if(nTarget.unitList.Length > 0){
				if(nTarget.unitList[unitId].obj){
					MiniMapSignal myTargetMap = nTarget.unitList[unitId].obj.GetComponent<MiniMapSignal>();
					VisionSignal myTargetVision = nTarget.unitList[unitId].obj.GetComponent<VisionSignal>();
					UnitController myTarget = nTarget.unitList[unitId].obj.GetComponent<UnitController>();
					UnitMovement moveTarget = nTarget.unitList[unitId].obj.GetComponent<UnitMovement>();
					if(myTarget){
						if(nTarget.unitList[unitId].obj != lastObj){
							objEditor = Editor.CreateEditor(nTarget.unitList[unitId].obj);
							lastObj = nTarget.unitList[unitId].obj;
						}
						objEditor.OnInteractivePreviewGUI(new Rect(945*rect.x, 50*rect.y, 555*rect.x, 650*rect.y), EditorStyles.toolbarButton);
						
						// Button Display Area
						
						if(GUI.Button(new Rect(400*rect.x, 0, 275*rect.x, 25*rect.y), "Stats")){
							menuState = 0;
							subMenuState = 0;
						}
						if(GUI.Button(new Rect(675*rect.x, 0, 275*rect.x, 25*rect.y), "Visuals")){
							menuState = 1;
							subMenuState = 0;
						}
						if(GUI.Button(new Rect(950*rect.x, 0, 275*rect.x, 25*rect.y), "Techs")){
							menuState = 2;
							subMenuState = 0;
						}
						if(GUI.Button(new Rect(1225*rect.x, 0, 275*rect.x, 25*rect.y), "Anim/Sounds")){
							menuState = 3;
							subMenuState = 0;
						}
						
						// Menu Area
						
						// The Stats Area
						if(menuState == 0){
							GUI.DrawTexture(new Rect(400*rect.x, 0, 275*rect.x, 25*rect.y), selectionTexture, ScaleMode.StretchToFill);
							if(GUI.Button(new Rect(400*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Global")){
								subMenuState = 0;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(620*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Weapon")){
								subMenuState = 1;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(840*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Build")){
								subMenuState = 2;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(1060*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Gather")){
								subMenuState = 3;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(1280*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Ratios")){
								subMenuState = 4;
								arraySelect = 0;
							}
							// Global
							if(subMenuState == 0){
								helpState = 5;
								GUI.DrawTexture(new Rect(400*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								myTarget.name = EditorGUI.TextField(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Name : ", myTarget.name);
								myTarget.maxHealth = EditorGUI.IntField(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Max Health : ", myTarget.maxHealth);
								myTarget.health = EditorGUI.IntField(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 25*rect.y), "Health : ", myTarget.health);
								moveTarget.speed = EditorGUI.IntField(new Rect(400*rect.x, 160*rect.y, 540*rect.x, 25*rect.y), "Speed : ", moveTarget.speed);
								moveTarget.rotateSpeed = EditorGUI.IntField(new Rect(400*rect.x, 190*rect.y, 540*rect.x, 25*rect.y), "Rotate Speed : ", moveTarget.rotateSpeed);
								if(target.types.Length > 0){
									string[] typeNames = new string[target.types.Length];
									for(int x = 0; x < target.types.Length; x++){
										typeNames[x] = (x+1) + ". " + target.types[x].name;
										if(target.types[x] == myTarget.type){
											arraySelect = x;
										}
									}
									arraySelect = EditorGUI.Popup(new Rect(400*rect.x, 220*rect.y, 545*rect.x, 25*rect.y), "Type : ", arraySelect, typeNames);
									myTarget.type = target.types[arraySelect];
								}
								nTarget.unitList[unitId].obj = EditorGUI.ObjectField(new Rect(400*rect.x, 250*rect.y, 540*rect.x, 25*rect.y), "Object : ", nTarget.unitList[unitId].obj, typeof(GameObject), true) as GameObject;
								
								//Add In Unit Movement Stats
							}
							// Weapon
							else if(subMenuState == 1){
								helpState = 6;
								GUI.DrawTexture(new Rect(620*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								myTarget.weapon.fighterUnit = EditorGUI.Toggle(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Fighter : ", myTarget.weapon.fighterUnit);
								if(myTarget.weapon.fighterUnit){
									myTarget.weapon.attackRate = EditorGUI.FloatField(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Attack Rate : ", myTarget.weapon.attackRate);
									myTarget.weapon.attackRange = EditorGUI.FloatField(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 25*rect.y), "Attack Range : ", myTarget.weapon.attackRange);
									myTarget.weapon.attackDamage = EditorGUI.IntField(new Rect(400*rect.x, 160*rect.y, 540*rect.x, 25*rect.y), "Attack Damage : ", myTarget.weapon.attackDamage);
									myTarget.weapon.lookRange = EditorGUI.FloatField(new Rect(400*rect.x, 190*rect.y, 540*rect.x, 25*rect.y), "Look Range : ", myTarget.weapon.lookRange);
									myTarget.weapon.attackObj = EditorGUI.ObjectField(new Rect(400*rect.x, 220*rect.y, 540*rect.x, 25*rect.y), "Attack Object : ", myTarget.weapon.attackObj, typeof(GameObject), true) as GameObject;
								}
							}
							// Build
							else if(subMenuState == 2){
								helpState = 7;
								GUI.DrawTexture(new Rect(840*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								myTarget.build.builderUnit = EditorGUI.Toggle(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Builder : ", myTarget.build.builderUnit);
								if(myTarget.build.builderUnit){
									if(myTarget.build.build.Length != nTarget.buildingList.Length){
										ModifyUB(nTarget.buildingList.Length, myTarget.build.build.Length, myTarget);
									}
									else{
										string[] buildNames = new string[nTarget.buildingList.Length];
										for(int x = 0; x < buildNames.Length; x++){
											if(nTarget.buildingList[x].obj != null){
												BuildingController cont = nTarget.buildingList[x].obj.GetComponent<BuildingController>();
												if(cont != null){
													buildNames[x] = cont.name;
												}
											}
										}
										if(arraySelect >= buildNames.Length){
											arraySelect = 0; 
										}
										else{
											arraySelect = EditorGUI.Popup(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), arraySelect, buildNames);
											if(nTarget.buildingList.Length > 0){
												if(myTarget.build.build[arraySelect].canBuild){
													GUI.Box(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 90*rect.y), "");
												}
												else{
													GUI.Box(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 25*rect.y), "");
												}
												myTarget.build.build[arraySelect].canBuild = EditorGUI.Toggle(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 25*rect.y), "Can Build : ", myTarget.build.build[arraySelect].canBuild);
												if(myTarget.build.build[arraySelect].canBuild){
													myTarget.build.build[arraySelect].amount = EditorGUI.IntField(new Rect(400*rect.x, 160*rect.y, 540*rect.x, 25*rect.y), "Build Amount : ", myTarget.build.build[arraySelect].amount);
													myTarget.build.build[arraySelect].rate = EditorGUI.FloatField(new Rect(400*rect.x, 190*rect.y, 540*rect.x, 25*rect.y), "Build Rate : ", myTarget.build.build[arraySelect].rate);
												}
											}
										}
									}
								}
							}
							// Gather
							else if(subMenuState == 3){
								helpState = 8;
								GUI.DrawTexture(new Rect(1060*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								myTarget.resource.resourceUnit = EditorGUI.Toggle(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Gatherer : ", myTarget.resource.resourceUnit);
								if(myTarget.resource.resourceUnit){
									GameObject resourceManager = GameObject.Find("Player Manager");
									if(resourceManager != null){
										ResourceManager rMScript= resourceManager.GetComponent<ResourceManager>();
										if(rMScript){
											if(myTarget.resource.behaviour.Length != rMScript.resourceTypes.Length){
												ModifyUR(rMScript.resourceTypes.Length, myTarget.resource.behaviour.Length, myTarget);
											}
											else{
												string[] resourceNames = new string[rMScript.resourceTypes.Length];
												for(int x = 0; x < resourceNames.Length; x++){
													resourceNames[x] = rMScript.resourceTypes[x].name;
												}
												if(arraySelect >= resourceNames.Length){
													arraySelect = 0; 
												}
												else{
													arraySelect = EditorGUI.Popup(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), arraySelect, resourceNames);
													if(myTarget.resource.behaviour[arraySelect].canGather){
														GUI.Box(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 150*rect.y), "");
													}
													else{
														GUI.Box(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 25*rect.y), "");
													}
													myTarget.resource.behaviour[arraySelect].canGather = EditorGUI.Toggle(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 25*rect.y), "Can Gather : ", myTarget.resource.behaviour[arraySelect].canGather);
													if(myTarget.resource.behaviour[arraySelect].canGather){
														myTarget.resource.behaviour[arraySelect].amount = EditorGUI.IntField(new Rect(400*rect.x, 160*rect.y, 540*rect.x, 25*rect.y), "Gather Amount : ", myTarget.resource.behaviour[arraySelect].amount);
														myTarget.resource.behaviour[arraySelect].rate = EditorGUI.FloatField(new Rect(400*rect.x, 190*rect.y, 540*rect.x, 25*rect.y), "Gather Rate : ", myTarget.resource.behaviour[arraySelect].rate);
														myTarget.resource.behaviour[arraySelect].returnWhenFull = EditorGUI.Toggle(new Rect(400*rect.x, 220*rect.y, 540*rect.x, 25*rect.y), "Drop Off Resource : ", myTarget.resource.behaviour[arraySelect].returnWhenFull);
													}	myTarget.resource.behaviour[arraySelect].carryCapacity = EditorGUI.IntField(new Rect(400*rect.x, 250*rect.y, 540*rect.x, 25*rect.y), "Carry Capacity : ", myTarget.resource.behaviour[arraySelect].carryCapacity);
												}
											}
										}
									}
								}
							}
							// Ratios
							else if(subMenuState == 4){
								GUI.DrawTexture(new Rect(1280*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								if(GUI.Button(new Rect(400*rect.x, 50*rect.y, 273*rect.x, 20*rect.y), "Add")){
									ModifyURA(myTarget.ratio.Length+1, myTarget.ratio.Length, myTarget);
								}
								if(GUI.Button(new Rect(673*rect.x, 50*rect.y, 272*rect.x, 20*rect.y), "Remove")){
									ModifyURA(myTarget.ratio.Length-1, myTarget.ratio.Length, myTarget);						
								}
								if(myTarget.ratio.Length > 0){
									string[] names = new string[myTarget.ratio.Length];
									for(int x = 0; x < names.Length; x++){
										names[x] = "" + (x+1) + ". " + myTarget.ratio[x].name;
									}
									if(arraySelect < names.Length){
										arraySelect = EditorGUI.Popup(new Rect(400*rect.x, 75*rect.y, 545*rect.x, 20*rect.y), "Ratio : ", arraySelect, names);
										myTarget.ratio[arraySelect].name = EditorGUI.TextField(new Rect(400*rect.x, 100*rect.y, 545*rect.x, 20*rect.y), "Enemy Name : ", myTarget.ratio[arraySelect].name);
										myTarget.ratio[arraySelect].amount = EditorGUI.FloatField(new Rect(400*rect.x, 125*rect.y, 545*rect.x, 20*rect.y), "Amount : ", myTarget.ratio[arraySelect].amount);
									}
									else{
										arraySelect = 0;
									}
								}
							}
						}
						// The Visuals Area
						else if(menuState == 1){
							GUI.DrawTexture(new Rect(675*rect.x, 0, 275*rect.x, 25*rect.y), selectionTexture, ScaleMode.StretchToFill);
							if(GUI.Button(new Rect(400*rect.x, 25*rect.y, 367*rect.x, 20*rect.y), "Selected")){
								subMenuState = 0;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(767*rect.x, 25*rect.y, 367*rect.x, 20*rect.y), "MiniMap")){
								subMenuState = 1;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(1134*rect.x, 25*rect.y, 366*rect.x, 20*rect.y), "Vision")){
								subMenuState = 2;
								arraySelect = 0;
							}
							// Selected
							if(subMenuState == 0){
								GUI.DrawTexture(new Rect(400*rect.x, 25*rect.y, 367*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								myTarget.gui.SetType("Unit");
								EditorGUI.LabelField(new Rect(400*rect.x, 50*rect.y, 545*rect.x, 100*rect.y), "GUI Image : ");
								myTarget.gui.image = EditorGUI.ObjectField(new Rect(400*rect.x, 80*rect.y, 100*rect.x, 100*rect.y), "", myTarget.gui.image, typeof(Texture2D), true) as Texture2D;
								//Modify Selected Objects
								if(GUI.Button(new Rect(400*rect.x, 180*rect.y, 273*rect.x, 25*rect.y), "Add")){
									ModifyUS(myTarget.gui.selectObjs.Length+1, myTarget.gui.selectObjs.Length, myTarget);
								}
								if(GUI.Button(new Rect(673*rect.x, 180*rect.y, 272*rect.x, 25*rect.y), "Remove")){
									ModifyUS(myTarget.gui.selectObjs.Length-1, myTarget.gui.selectObjs.Length, myTarget);
								}
								string[] list = new string[myTarget.gui.selectObjs.Length];
								for(int x = 0; x < list.Length; x++){
									if(myTarget.gui.selectObjs[x]){
										list[x] = (x+1) + ". " + myTarget.gui.selectObjs[x].name;
									}
									else{
										list[x] = (x+1) + ". Empty";
									}
								}
								arraySelect = EditorGUI.Popup(new Rect(400*rect.x, 210*rect.y, 545*rect.x, 25*rect.y), "Objects : ", arraySelect, list);
								if(arraySelect >= myTarget.gui.selectObjs.Length){
									arraySelect = 0;
								}
								else{
									myTarget.gui.selectObjs[arraySelect] = EditorGUI.ObjectField(new Rect(400*rect.x, 240*rect.y, 545*rect.x, 25*rect.y), "", myTarget.gui.selectObjs[arraySelect], typeof(GameObject)) as GameObject;
								}
								Health healthObj = nTarget.unitList[unitId].obj.GetComponent<Health>();
								if(healthObj == null){
									if(GUI.Button(new Rect(400*rect.x, 280*rect.y, 545*rect.x, 420*rect.y), "Add Health Indicator")){
										nTarget.unitList[unitId].obj.AddComponent<Health>();
									}
								}
								else{
									healthObj.backgroundBar = EditorGUI.ObjectField(new Rect(400*rect.x, 280*rect.y, 100*rect.x, 50*rect.y), healthObj.backgroundBar, typeof(Texture2D), true) as Texture2D;
									healthObj.healthBar = EditorGUI.ObjectField(new Rect(400*rect.x, 330*rect.y, 100*rect.x, 50*rect.y), healthObj.healthBar, typeof(Texture2D), true) as Texture2D;
									healthObj.yIncrease = EditorGUI.FloatField(new Rect(400*rect.x, 390*rect.y, 545*rect.x, 30*rect.y), "World-Y Increase : ", healthObj.yIncrease);
									healthObj.scale = EditorGUI.IntField(new Rect(400*rect.x, 420*rect.y, 545*rect.x, 30*rect.y), "UI-X Scale : ", healthObj.scale);
									healthObj.yScale = EditorGUI.IntField(new Rect(400*rect.x, 450*rect.y, 545*rect.x, 30*rect.y), "UI-Y Scale : ", healthObj.yScale);
									int healthLength = healthObj.element.Length;
									healthLength = EditorGUI.IntField(new Rect(400*rect.x, 485*rect.y, 545*rect.x, 30*rect.y), "Health Elements : ", healthLength);
									if(healthLength != healthObj.element.Length){
										ModifyHE(healthLength, healthObj.element.Length, healthObj);
									}
									if(healthLength > 0){
										string[] elementName = new string[healthLength];
										for(int x = 0; x < elementName.Length; x++){
											elementName[x] = "Element " + (x+1);
										}
										arraySelect1 = EditorGUI.Popup(new Rect(400*rect.x, 520*rect.y, 545*rect.x, 25*rect.y), "Element : ", arraySelect1, elementName);
										healthObj.element[arraySelect1].image = EditorGUI.ObjectField(new Rect(400*rect.x, 540*rect.y, 100*rect.x, 50*rect.y), healthObj.element[arraySelect1].image, typeof(Texture2D), true) as Texture2D;
										healthObj.element[arraySelect1].loc = EditorGUI.RectField(new Rect(400*rect.x, 600*rect.y, 545*rect.x, 50*rect.y), healthObj.element[arraySelect1].loc);
									}
									Vector2 point = new Vector2(620*rect.x, 650*rect.y);
									if(healthObj.backgroundBar != null){
										GUI.DrawTexture(new Rect(point.x, point.y, healthObj.scale*((float)1), healthObj.yScale), healthObj.backgroundBar);
									}
									if(healthObj.healthBar != null){
										GUI.DrawTexture(new Rect(point.x, point.y, healthObj.scale*((float)50/100), healthObj.yScale), healthObj.healthBar);
									}
									for(int x = 0; x < healthObj.element.Length; x++){
										GUI.DrawTexture(new Rect(point.x+healthObj.element[x].loc.x, (point.y-healthObj.element[x].loc.y), healthObj.element[x].loc.width, healthObj.element[x].loc.height), healthObj.element[x].image); 
									}
								}
								helpState = 9;
							}
							// MiniMap
							else if(subMenuState == 1){
								GUI.DrawTexture(new Rect(767*rect.x, 25*rect.y, 367*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								if(myTargetMap == null){
									if(GUI.Button(new Rect(400*rect.x, 45*rect.y, 1100*rect.x, 705*rect.y), "Add MiniMap Components")){
										nTarget.unitList[unitId].obj.AddComponent("MiniMapSignal");
									}
								}
								else{
									myTargetMap.enabled = EditorGUI.Toggle(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "MiniMap Signal : ", myTargetMap.enabled);
									if(myTargetMap.enabled){
										myTargetMap.miniMapTag = EditorGUI.TextField(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Tag : ", "" + myTargetMap.miniMapTag);
										GameObject mapObj = GameObject.Find("MiniMap");
										Color defaultColor = GUI.color;
										if(mapObj != null){
											MiniMap map = GameObject.Find("MiniMap").GetComponent<MiniMap>();
											if(map != null){
												helpState = 10;
												for(int x = 0; x < map.elements.Length; x++){
													if(map.elements[x].tag == myTargetMap.miniMapTag){
														if(map.elements[x].tints.Length > miniMapState){
															GUI.color = map.elements[x].tints[miniMapState];
														}
														else{
															GUI.color = map.elements[x].tints[0];
														}
														int size = (int)(50*rect.x);
														GUI.DrawTexture(new Rect(400*rect.x, 130*rect.y, size, size), map.elements[x].image);
														GUI.color = defaultColor;
														map.elements[x].image = EditorGUI.ObjectField(new Rect(400*rect.x, 185*rect.y, 100, 100), map.elements[x].image, typeof(Texture2D), true) as Texture2D;
														miniMapState = EditorGUI.IntField(new Rect(400*rect.x, 320*rect.y, 540*rect.x, 25*rect.y), "Group : ", miniMapState);
													}
												}
											}
											else{
												helpState = 11;
											}
										}
									}
								}
							}
							// Fog Of War
							else if(subMenuState == 2){
								GUI.DrawTexture(new Rect(1134*rect.x, 25*rect.y, 366*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								if(myTargetVision == null){
									if(GUI.Button(new Rect(400*rect.x, 45*rect.y, 1100*rect.x, 705*rect.y), "Add Vision Components")){
										nTarget.unitList[unitId].obj.AddComponent("VisionSignal");
										// Add More Components
									}
								}
								else{
									myTargetVision.enabled = EditorGUI.Toggle(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Vision Signal : ", myTargetVision.enabled);
									if(myTargetVision.enabled){
										myTargetVision.radius = EditorGUI.IntField(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Radius : ", myTargetVision.radius);
										myTargetVision.upwardSightHeight = EditorGUI.IntField(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 25*rect.y), "Upward Sight Height : ", myTargetVision.upwardSightHeight);
										myTargetVision.downwardSightHeight = EditorGUI.IntField(new Rect(400*rect.x, 160*rect.y, 540*rect.x, 25*rect.y), "Downward Sight Height : ", myTargetVision.downwardSightHeight);
									}
									helpState = 12;
								}
							}
						}
						// Technology
						else if(menuState == 2){
							GUI.DrawTexture(new Rect(950*rect.x, 0, 275*rect.x, 25*rect.y), selectionTexture, ScaleMode.StretchToFill);
							GUI.Box(new Rect(400*rect.x, 25*rect.y, 1100*rect.x, 20*rect.y), "");
							if(GUI.Button(new Rect(400*rect.x, 50*rect.y, 273*rect.x, 20*rect.y), "Add")){
								ModifyUT(myTarget.techEffect.Length+1, myTarget.techEffect.Length, myTarget);
							}
							if(GUI.Button(new Rect(673*rect.x, 50*rect.y, 272*rect.x, 20*rect.y), "Remove")){
								ModifyUT(myTarget.techEffect.Length-1, myTarget.techEffect.Length, myTarget);						
							}
							if(nTarget.tech.Length > 0 && myTarget.techEffect.Length > 0){
								int size = nTarget.tech.Length;
								string[] names = new string[size];
								string[] nameArray = new string[size];
								for(int x = 0; x < nameArray.Length; x++){
									nameArray[x] = (x+1) + ". " + nTarget.tech[x].name;
									names[x] = nTarget.tech[x].name;
								}
								string[] unitTechs = new string[myTarget.techEffect.Length];
								for(int x = 0; x < unitTechs.Length; x++){
									unitTechs[x] = (x+1) + ". " + myTarget.techEffect[x].name;
								}
								arraySelect = EditorGUI.Popup(new Rect(400*rect.x, 75*rect.y, 545*rect.x, 20*rect.y), "Techs : ", arraySelect, unitTechs);
								if(arraySelect >= myTarget.techEffect.Length){
									arraySelect = 0;
								}
								else{
									int curY = 100;
									arraySelect2 = myTarget.techEffect[arraySelect].index;
									arraySelect2 = EditorGUI.Popup(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 20*rect.y), "Tech Options : ", arraySelect2, nameArray);
									curY += 25;
									myTarget.techEffect[arraySelect].index = arraySelect2;
									if(arraySelect2 > nameArray.Length){
										arraySelect2 = 0;
									}
									myTarget.techEffect[arraySelect].name = names[arraySelect2];
									myTarget.techEffect[arraySelect].replacementObject = EditorGUI.ObjectField(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 20*rect.y), "Replacement Object : ", myTarget.techEffect[arraySelect].replacementObject, typeof(GameObject)) as GameObject;
									curY += 25;
									string[,] functions = 	new string[24,4]{
												/* 0 */		{"GetName", "SetName", "", ""}, 
												/* 1 */		{"GetMaxHealth", "SetMaxHealth", "AddMaxHealth", "SubMaxHealth"},
												/* 2 */		{"GetHealth", "SetHealth", "AddHealth", "SubHealth"},
												/* 3 */		{"GetGroup", "SetGroup", "", ""},
												/* 4 */		{"GetFighterUnit", "SetFighterUnit", "", ""},
												/* 5 */		{"GetAttackRate", "SetAttackRate", "AddAttackRate", "SubAttackRate"},
												/* 6 */		{"GetAttackRange", "SetAttackRange", "AddAttackRange", "SubAttackRange"},
												/* 7 */		{"GetAttackDamage", "SetAttackDamage", "AddAttackDamage", "SubAttackDamage"},
												/* 8 */		{"GetLookRange", "SetLookRange", "AddLookRange", "SubLookRange"},
												/* 9 */		{"GetSize", "SetSize", "AddSize", "SubSize"},
												/* 10 */	{"GetRadius", "SetRadius", "AddRadius", "SubRadius"},
												/* 11 */	{"GetResourceUnit", "SetResourceUnit", "", ""},
												/* 12 */	{"GetCanGather", "SetCanGather", "", ""},
												/* 13 */	{"GetResourceAmount", "SetResourceAmount", "AddResourceAmount", "SubResourceAmount"},
												/* 14 */	{"GetResourceRate", "SetResourceRate", "AddResourceRate", "SubResourceRate"},
												/* 15 */	{"GetBuilderUnit", "SetBuilderUnit", "AddBuilderUnit", "SubBuilderUnit"},
												/* 16 */	{"GetCanBuild", "SetCanBuild", "", ""},
												/* 17 */	{"GetBuilderAmount", "SetBuilderAmount", "AddBuilderAmount", "SubBuilderAmount"},
												/* 18 */	{"GetBuilderRate", "SetBuilderRate", "AddBuilderRate", "SubBuilderRate"},
												/* 19 */	{"GetSpeed", "SetSpeed", "AddSpeed", "SubSpeed"},
												/* 20 */	{"GetRotateSpeed", "SetRotateSpeed", "AddRotateSpeed", "SubRotateSpeed"},
												/* 21 */	{"GetMiniMapTag", "SetMiniMapTag", "AddMiniMapTag", "SubMiniMapTag"},
												/* 22 */	{"GetVisionRadius", "SetVisionRadius", "AddVisionRadius", "SubVisionRadius"},
												/* 23 */	{"GetCarryCapacity", "SetCarryCapacity", "AddCarryCapacity", "SubCarryCapacity"}
															};
									string[] variableName = {"Name", "Max Health", "Health", "Group", 
															 "Fighter Unit", "Attack Rate", "Attack Range", "Attack Damage", "Look Range", 
															 "Size", "Vision Radius",
															 "Resource Unit", "Can Gather", "Gather Amount", "Gather Rate",
															 "Builder Unit", "Can Build", "Rate", "Amount", 
															 "Speed", "Rotate Speed", "Mini Map Tag", "Vision Radius", "CarryCapacity"
															};
									for(int x = 0; x < myTarget.techEffect[arraySelect].effects.Length; x++){
										myTarget.techEffect[arraySelect].effects[x].effectName = EditorGUI.Popup(new Rect(400*rect.x, curY*rect.y, 200*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].effectName, variableName);
										int index = myTarget.techEffect[arraySelect].effects[x].effectName;
										string[] funcTypes;
										if(index == 0 || index == 3 || index == 4 || index == 11 || index == 12 || index == 15 || index == 16 || index == 21){
											funcTypes = new string[2];
											funcTypes[0] = "Get";
											funcTypes[1] = "Set";
										}
										else{
											funcTypes = new string[4];
											funcTypes[0] = "Get";
											funcTypes[1] = "Set";
											funcTypes[2] = "Add";
											funcTypes[3] = "Sub";
										}
										myTarget.techEffect[arraySelect].effects[x].funcType = EditorGUI.Popup(new Rect(600*rect.x, curY*rect.y, 100*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].funcType, funcTypes);
										myTarget.techEffect[arraySelect].effects[x].funcName = functions[index, myTarget.techEffect[arraySelect].effects[x].funcType];
										if(myTarget.techEffect[arraySelect].effects[x].funcType == 0){
											EditorGUI.LabelField(new Rect(800*rect.x, curY*rect.y, 145*rect.x, 20*rect.y), "Getter");
										}
										else{
											if(index == 0 || index == 21){
												myTarget.techEffect[arraySelect].effects[x].text = EditorGUI.TextField(new Rect(800*rect.x, curY*rect.y, 145*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].text);
											}
											else if(index == 4 || index == 11 || index == 15){
												myTarget.techEffect[arraySelect].effects[x].toggle = EditorGUI.Toggle(new Rect(800*rect.x, curY*rect.y, 145*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].toggle);
											}
											else if(index == 12 || index == 16){
												if(index == 12){
													// Resource List
													GameObject resourceManager = GameObject.Find("Player Manager");
													if(resourceManager != null){
														ResourceManager rMScript= resourceManager.GetComponent<ResourceManager>();
														if(rMScript){
															if(myTarget.resource.behaviour.Length != rMScript.resourceTypes.Length){
																ModifyUR(rMScript.resourceTypes.Length, myTarget.resource.behaviour.Length, myTarget);
															}
															else{
																string[] resourceNames = new string[rMScript.resourceTypes.Length];
																for(int y = 0; y < resourceNames.Length; y++){
																	resourceNames[y] = rMScript.resourceTypes[y].name;
																}
																if(arraySelect >= resourceNames.Length){
																	arraySelect = 0; 
																}
																else{
																	if(myTarget.techEffect[arraySelect].effects[x].index >= resourceNames.Length){
																		myTarget.techEffect[arraySelect].effects[x].index = 0;
																	}
																	myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(new Rect(700*rect.x, curY*rect.y, 100*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].index, resourceNames);
																	myTarget.techEffect[arraySelect].effects[x].toggle = EditorGUI.Toggle(new Rect(800*rect.x, curY*rect.y, 145*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].toggle);
																}
															}
														}
													}
												}
												else if(index == 16){
													// Building List
													if(myTarget.build.build.Length != nTarget.buildingList.Length){
														ModifyUB(nTarget.buildingList.Length, myTarget.build.build.Length, myTarget);
													}
													else{
														string[] buildNames = new string[nTarget.buildingList.Length];
														for(int y = 0; y < buildNames.Length; y++){
															if(nTarget.buildingList[y].obj != null){
																BuildingController cont = nTarget.buildingList[y].obj.GetComponent<BuildingController>();
																if(cont != null){
																	buildNames[y] = cont.name;
																}
															}
														}
														if(arraySelect >= buildNames.Length){
															arraySelect = 0; 
														}
														else{
															if(myTarget.techEffect[arraySelect].effects[x].index >= buildNames.Length){
																myTarget.techEffect[arraySelect].effects[x].index = 0;
															}
															myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(new Rect(700*rect.x, curY*rect.y, 100*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].index, buildNames);
															myTarget.techEffect[arraySelect].effects[x].toggle = EditorGUI.Toggle(new Rect(800*rect.x, curY*rect.y, 145*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].toggle);
														}
													}
												}
											}
											else if(index == 13 || index == 14 || index == 17 || index == 18){
												if(index == 13 || index == 14){
													// Resource List
													GameObject resourceManager = GameObject.Find("Player Manager");
													if(resourceManager != null){
														ResourceManager rMScript= resourceManager.GetComponent<ResourceManager>();
														if(rMScript){
															if(myTarget.resource.behaviour.Length != rMScript.resourceTypes.Length){
																ModifyUR(rMScript.resourceTypes.Length, myTarget.resource.behaviour.Length, myTarget);
															}
															else{
																string[] resourceNames = new string[rMScript.resourceTypes.Length];
																for(int y = 0; y < resourceNames.Length; y++){
																	resourceNames[y] = rMScript.resourceTypes[y].name;
																}
																if(arraySelect >= resourceNames.Length){
																	arraySelect = 0; 
																}
																else{
																	if(myTarget.techEffect[arraySelect].effects[x].index >= resourceNames.Length){
																		myTarget.techEffect[arraySelect].effects[x].index = 0;
																	}
																	myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(new Rect(700*rect.x, curY*rect.y, 100*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].index, resourceNames);
																	myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(new Rect(800*rect.x, curY*rect.y, 145*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].amount);
																}
															}
														}
													}
												}
												else if(index == 17 || index == 18){
													// Building List
													if(myTarget.build.build.Length != nTarget.buildingList.Length){
														ModifyUB(nTarget.buildingList.Length, myTarget.build.build.Length, myTarget);
													}
													else{
														string[] buildNames = new string[nTarget.buildingList.Length];
														for(int y = 0; y < buildNames.Length; y++){
															if(nTarget.buildingList[y].obj != null){
																BuildingController cont = nTarget.buildingList[y].obj.GetComponent<BuildingController>();
																if(cont != null){
																	buildNames[y] = cont.name;
																}
															}
														}
														if(arraySelect >= buildNames.Length){
															arraySelect = 0; 
														}
														else{
															if(myTarget.techEffect[arraySelect].effects[x].index >= buildNames.Length){
																myTarget.techEffect[arraySelect].effects[x].index = 0;
															}
															myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(new Rect(700*rect.x, curY*rect.y, 100*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].index, buildNames);
															myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(new Rect(800*rect.x, curY*rect.y, 145*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].amount);
														}
													}
												}
											}
											else{
												myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(new Rect(800*rect.x, curY*rect.y, 145*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].amount);
											}
										}
										curY += 25;
									}
									if(GUI.Button(new Rect(400*rect.x, curY*rect.y, 273*rect.x, 20*rect.y), "Add")){
										ModifyUTE(myTarget.techEffect[arraySelect].effects.Length+1, myTarget.techEffect[arraySelect].effects.Length, myTarget, arraySelect);
									}
									if(GUI.Button(new Rect(673*rect.x, curY*rect.y, 272*rect.x, 20*rect.y), "Remove")){
										ModifyUTE(myTarget.techEffect[arraySelect].effects.Length-1, myTarget.techEffect[arraySelect].effects.Length, myTarget, arraySelect);
									}
								}
							}
						}
						// Animation and Sounds
						else if(menuState == 3){
							GUI.DrawTexture(new Rect(1225*rect.x, 0, 275*rect.x, 25*rect.y), selectionTexture, ScaleMode.StretchToFill);
							GUI.Box(new Rect(400*rect.x, 25*rect.y, 1100*rect.x, 20*rect.y), "");
							Animator comp = nTarget.unitList[unitId].obj.GetComponent<Animator>();
							if(myTarget.anim.manager){
								myTarget.anim.manager.runtimeAnimatorController = EditorGUI.ObjectField(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Controller : ", myTarget.anim.manager.runtimeAnimatorController, typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
							}
							else if(comp){
								myTarget.anim.manager = comp;
							}
							else{
								if(GUI.Button(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Add Animator")){
									nTarget.unitList[unitId].obj.AddComponent<Animator>();
								}
							}
							myTarget.anim.idleAudio = EditorGUI.ObjectField(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Idle Audio : ", myTarget.anim.idleAudio, typeof(AudioClip)) as AudioClip;
							myTarget.anim.moveAudio = EditorGUI.ObjectField(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 25*rect.y), "Move Audio : ", myTarget.anim.moveAudio, typeof(AudioClip)) as AudioClip;
							myTarget.anim.gatherAudio = EditorGUI.ObjectField(new Rect(400*rect.x, 160*rect.y, 540*rect.x, 25*rect.y), "Gather Audio : ", myTarget.anim.gatherAudio, typeof(AudioClip)) as AudioClip;
							myTarget.anim.buildAudio = EditorGUI.ObjectField(new Rect(400*rect.x, 190*rect.y, 540*rect.x, 25*rect.y), "Build Audio : ", myTarget.anim.buildAudio, typeof(AudioClip)) as AudioClip;
							myTarget.anim.attackAudio = EditorGUI.ObjectField(new Rect(400*rect.x, 220*rect.y, 540*rect.x, 25*rect.y), "Attack Audio : ", myTarget.anim.attackAudio, typeof(AudioClip)) as AudioClip;
							myTarget.anim.deathObject = EditorGUI.ObjectField(new Rect(400*rect.x, 250*rect.y, 540*rect.x, 25*rect.y), "Death Object : ", myTarget.anim.deathObject, typeof(GameObject)) as GameObject;
						}
					}
					else{
						// If the Unit does not have the base unit scripts, give the user the option to add them
						if(GUI.Button(new Rect(400*rect.x, 0, 1100*rect.x, 750*rect.y), "Add Unit Controller Components")){
							myTarget = nTarget.unitList[unitId].obj.AddComponent<UnitController>();
							myTarget.gui.SetType("Unit");
							// Add More Components
						}
					}
				}
			}
		}
	}
	
	void DrawBuildingGUI (Vector2 rect) {
		if(nTarget != null){
			if(unitId >= nTarget.buildingList.Length || unitId < 0){
				unitId = 0;
			}
			else if(nTarget.buildingList.Length > 0){
				if(nTarget.buildingList[unitId].obj){
					BuildingController myTarget = nTarget.buildingList[unitId].obj.GetComponent<BuildingController>();
					MiniMapSignal myTargetMap = nTarget.buildingList[unitId].obj.GetComponent<MiniMapSignal>();
					VisionSignal myTargetVision = nTarget.buildingList[unitId].obj.GetComponent<VisionSignal>();
					if(myTarget){
						if(GUI.Button(new Rect(400*rect.x, 0, 275*rect.x, 25*rect.y), "Stats")){
							menuState = 0;
							subMenuState = 0;
						}
						if(GUI.Button(new Rect(675*rect.x, 0, 275*rect.x, 25*rect.y), "GUI")){
							menuState = 1;
							subMenuState = 0;
						}
						if(GUI.Button(new Rect(950*rect.x, 0, 275*rect.x, 25*rect.y), "Techs")){
							menuState = 2;
							subMenuState = 0;
						}
						if(GUI.Button(new Rect(1225*rect.x, 0, 275*rect.x, 25*rect.y), "Anim/Sounds")){
							menuState = 3;
							subMenuState = 0;
						}
						// Stats
						if(menuState == 0){
							if(nTarget.buildingList[unitId].obj != lastObj){
							objEditor = Editor.CreateEditor(nTarget.buildingList[unitId].obj);
							lastObj = nTarget.buildingList[unitId].obj;
						}
						if(menuState == 0 && subMenuState != 3){
							objEditor.OnInteractivePreviewGUI(new Rect(945*rect.x, 50*rect.y, 555*rect.x, 650*rect.y), EditorStyles.toolbarButton);
						}
							GUI.DrawTexture(new Rect(400*rect.x, 0, 275*rect.x, 25*rect.y), selectionTexture, ScaleMode.StretchToFill);
							if(GUI.Button(new Rect(400*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Global")){
								subMenuState = 0;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(620*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Production")){
								subMenuState = 2;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(840*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Size")){
								subMenuState = 3;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(1060*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Generate")){
								subMenuState = 4;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(1280*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Drop Off")){
								subMenuState = 5;
								arraySelect = 0;
							}
							// Global
							if(subMenuState == 0){		
								GUI.DrawTexture(new Rect(400*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								nTarget.buildingList[unitId].autoBuild = EditorGUI.Toggle(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Auto Build : ", nTarget.buildingList[unitId].autoBuild);
								nTarget.buildingList[unitId].tempObj = EditorGUI.ObjectField(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Temporary : ", nTarget.buildingList[unitId].tempObj, typeof(GameObject), true) as GameObject;
								int curY = 130;
								if(!nTarget.buildingList[unitId].autoBuild){
									nTarget.buildingList[unitId].progressObj = EditorGUI.ObjectField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Progress : ", nTarget.buildingList[unitId].progressObj, typeof(GameObject), true) as GameObject;
									curY += 30;
								}
								nTarget.buildingList[unitId].obj = EditorGUI.ObjectField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Final : ", nTarget.buildingList[unitId].obj, typeof(GameObject), true) as GameObject;
								curY += 30;
								myTarget.name = EditorGUI.TextField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Name : ", myTarget.name);
								curY += 30;
								myTarget.maxHealth = EditorGUI.IntField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Max Health : ", myTarget.maxHealth);
								curY += 30;
								myTarget.health = EditorGUI.IntField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Health : ", myTarget.health);
								curY += 30;
								if(target.types.Length > 0){
									string[] typeNames = new string[target.types.Length];
									for(int x = 0; x < target.types.Length; x++){
										typeNames[x] = (x+1) + ". " + target.types[x].name;
										if(target.types[x] == myTarget.type){
											arraySelect = x;
										}
									}
									arraySelect = EditorGUI.Popup(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Type : ", arraySelect, typeNames);
									myTarget.type = target.types[arraySelect];
									curY += 30;
								}
								if(nTarget.buildingList[unitId].progressObj){
									BuildingController progressObj = nTarget.buildingList[unitId].progressObj.GetComponent<BuildingController>();
									BuildingController objScript = nTarget.buildingList[unitId].obj.GetComponent<BuildingController>();
									if(progressObj != null){
										progressObj.progressReq = EditorGUI.FloatField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Required Build Progress : ", progressObj.progressReq);
										curY += 30;
										progressObj.progressCur = EditorGUI.FloatField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Starting Progress : ", progressObj.progressCur);
										curY += 30;
										progressObj.progressRate = EditorGUI.FloatField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Progress Rate : ", progressObj.progressRate);
										curY += 30;
										progressObj.progressPerRate = EditorGUI.FloatField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Progress Per Rate : ", progressObj.progressPerRate);
										curY += 30;
										progressObj.nextBuild = nTarget.buildingList[unitId].obj;
										progressObj.buildingType = BuildingType.ProgressBuilding;
										objScript.buildingType = BuildingType.CompleteBuilding;
									}
									else{
										progressObj = nTarget.buildingList[unitId].progressObj.AddComponent<BuildingController>();
										progressObj.buildingType = BuildingType.ProgressBuilding;
									}
								}
								ResourceManager mg = GameObject.Find("Player Manager").GetComponent<ResourceManager>();
								if(nTarget.buildingList[unitId].cost.Length != mg.resourceTypes.Length){
									nTarget.buildingList[unitId].cost = new int[mg.resourceTypes.Length];
								}
								string[] costs = new string[mg.resourceTypes.Length];
								for(int x = 0; x < costs.Length; x++){
									costs[x] = mg.resourceTypes[x].name;
								}
								arraySelect3 = EditorGUI.Popup(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Resource Type : ", arraySelect3, costs);
								curY += 25;
								if(arraySelect3 >= costs.Length){
									arraySelect3 = 0;
								}
								else{
									nTarget.buildingList[unitId].cost[arraySelect3] = EditorGUI.IntField(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Cost : ", nTarget.buildingList[unitId].cost[arraySelect3]);
									curY += 30;
								}
								GUI.skin.textArea.wordWrap = true;
								nTarget.buildingList[unitId].description = EditorGUI.TextArea(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 100*rect.y), nTarget.buildingList[unitId].description);
							}
							// Production
							else if(subMenuState == 2){
								GUI.DrawTexture(new Rect(620*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								string[] options = {"Units", "Techs"};
								arraySelect = EditorGUI.Popup(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Production Type : ", arraySelect, options);
								if(arraySelect == 0){
									myTarget.unitProduction.canProduce = EditorGUI.Toggle(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Can Produce : ", myTarget.unitProduction.canProduce);
									if(myTarget.unitProduction.canProduce){
										
										int curY = 120;
										if(myTarget.unitProduction.units.Length > 0){
											myTarget.unitProduction.canBuildAtOnce = EditorGUI.IntField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Can Produce At Once : ", myTarget.unitProduction.canBuildAtOnce);
											curY += 30;
											myTarget.unitProduction.maxAmount = EditorGUI.IntField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Max Amount : ", myTarget.unitProduction.maxAmount);
											curY += 30;
											myTarget.unitProduction.buildLoc = EditorGUI.ObjectField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Build Loc : ", myTarget.unitProduction.buildLoc, typeof(GameObject)) as GameObject;
											curY += 30;
											string[] produceNames = new string[myTarget.unitProduction.units.Length];
											string[] unitNames = new string[nTarget.unitList.Length];
											for(int x = 0; x < nTarget.unitList.Length; x++){
												if(nTarget.unitList[x].obj){
													UnitController cont = nTarget.unitList[x].obj.GetComponent<UnitController>();
													if(cont != null){
														unitNames[x] = (x+1) + ". " + cont.name;
													}
												}
											}
											for(int x = 0; x < myTarget.unitProduction.units.Length; x++){
												if(myTarget.unitProduction.units[x] != null){
													produceNames[x] = (x+1) + ". " + myTarget.unitProduction.units[x].customName;
												}
											}
											if(GUI.Button(new Rect(400*rect.x, curY*rect.y, 270*rect.x, 15*rect.y),"Add Unit")){
												ModifyGBU(myTarget.unitProduction.units.Length+1, myTarget.unitProduction.units.Length, arraySelect);
											}
											if(GUI.Button(new Rect(670*rect.x, curY*rect.y, 270*rect.x, 15*rect.y),"Remove Unit")){
												ModifyGBU(myTarget.unitProduction.units.Length-1,myTarget.unitProduction.units.Length, arraySelect);
											}
											curY += 20;
											arraySelect2 = EditorGUI.Popup(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Production : ", arraySelect2, produceNames);
											curY += 20;
											if(arraySelect2 >= myTarget.unitProduction.units.Length || arraySelect2 < 0){
												arraySelect2 = myTarget.unitProduction.units.Length-1;
											}
											else{
												myTarget.unitProduction.units[arraySelect2].customName = EditorGUI.TextField(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Custom Name : ", myTarget.unitProduction.units[arraySelect2].customName);
												curY += 30;
												EditorGUI.LabelField(new Rect(400*rect.x, curY*rect.y, 150*rect.x, 100*rect.y), "Custom Texture : ");
												myTarget.unitProduction.units[arraySelect2].customTexture = EditorGUI.ObjectField(new Rect(600*rect.x, curY*rect.y, 150*rect.x, 100*rect.y),
													myTarget.unitProduction.units[arraySelect2].customTexture, typeof(Texture2D), true) as Texture2D;
												curY += 110;
												myTarget.unitProduction.units[arraySelect2].groupIndex = EditorGUI.Popup(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Unit To Produce : ", myTarget.unitProduction.units[arraySelect2].groupIndex, unitNames);
												curY += 20;
												myTarget.unitProduction.units[arraySelect2].canProduce = EditorGUI.Toggle(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Can Produce : ", myTarget.unitProduction.units[arraySelect2].canProduce);
												curY += 20;
												myTarget.unitProduction.units[arraySelect2].dur = EditorGUI.FloatField(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Duration : ", myTarget.unitProduction.units[arraySelect2].dur);
												curY += 25;
												myTarget.unitProduction.units[arraySelect2].amount = EditorGUI.FloatField(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Amount : ", myTarget.unitProduction.units[arraySelect2].amount);
												curY += 25;
												myTarget.unitProduction.units[arraySelect2].rate = EditorGUI.FloatField(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Rate : ", myTarget.unitProduction.units[arraySelect2].rate);
												curY += 30;
												ResourceManager mg = GameObject.Find("Player Manager").GetComponent<ResourceManager>();
												if(myTarget.unitProduction.units[arraySelect2].cost.Length != mg.resourceTypes.Length){
													myTarget.unitProduction.units[arraySelect2].cost = new int[mg.resourceTypes.Length];
												}
												string[] costs = new string[mg.resourceTypes.Length];
												for(int x = 0; x < costs.Length; x++){
													costs[x] = mg.resourceTypes[x].name;
												}
												arraySelect3 = EditorGUI.Popup(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Resource Type : ", arraySelect3, costs);
												curY += 25;
												if(arraySelect3 >= costs.Length){
													arraySelect3 = 0;
												}
												else{
													myTarget.unitProduction.units[arraySelect2].cost[arraySelect3] = EditorGUI.IntField(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), 
														"Cost to Produce : ", myTarget.unitProduction.units[arraySelect2].cost[arraySelect3]);
													curY += 30;
												}
												GUI.skin.textArea.wordWrap = true;
												myTarget.unitProduction.units[arraySelect2].description = EditorGUI.TextArea(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 100*rect.y), myTarget.unitProduction.units[arraySelect2].description);
											}
										}
										else{
											if(GUI.Button(new Rect(400*rect.x, curY*rect.y, 270*rect.x, 15*rect.y),"Add Unit")){
												ModifyGBU(myTarget.unitProduction.units.Length+1, myTarget.unitProduction.units.Length, arraySelect);
											}
											if(GUI.Button(new Rect(670*rect.x, curY*rect.y, 270*rect.x, 15*rect.y),"Remove Unit")){
												ModifyGBU(myTarget.unitProduction.units.Length-1,myTarget.unitProduction.units.Length, arraySelect);
											}
										}
									}
								}
								else if(arraySelect == 1){
									myTarget.techProduction.canProduce = EditorGUI.Toggle(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Can Produce : ", myTarget.techProduction.canProduce);
									if(myTarget.techProduction.canProduce){
										
										int curY = 120;
										if(myTarget.techProduction.techs.Length > 0){
											myTarget.techProduction.canBuildAtOnce = EditorGUI.IntField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Can Produce At Once : ", myTarget.techProduction.canBuildAtOnce);
											curY += 30;
											myTarget.techProduction.maxAmount = EditorGUI.IntField(new Rect(400*rect.x, curY*rect.y, 540*rect.x, 25*rect.y), "Max Amount : ", myTarget.techProduction.maxAmount);
											curY += 30;
											string[] produceNames = new string[myTarget.techProduction.techs.Length];
											for(int x = 0; x < myTarget.techProduction.techs.Length; x++){
												if(myTarget.techProduction.techs[x] == null){
													myTarget.techProduction.techs[x] = new ProduceTech();
												}
												if(myTarget.techProduction.techs[x] != null){
													produceNames[x] = (x+1) + ". " + myTarget.techProduction.techs[x].customName;
												}
												
											}
											int size = nTarget.tech.Length;
											string[] names = new string[size];
											string[] nameArray = new string[size];
											for(int x = 0; x < nameArray.Length; x++){
												nameArray[x] = (x+1) + ". " + nTarget.tech[x].name;
												names[x] = nTarget.tech[x].name;
											}
											string[] unitTechs = new string[myTarget.techEffect.Length];
											for(int x = 0; x < unitTechs.Length; x++){
												unitTechs[x] = (x+1) + ". " + myTarget.techEffect[x].name;
											}
											if(GUI.Button(new Rect(400*rect.x, curY*rect.y, 270*rect.x, 15*rect.y),"Add Tech")){
												ModifyGBT(myTarget.techProduction.techs.Length+1, myTarget.techProduction.techs.Length, arraySelect);
											}
											if(GUI.Button(new Rect(670*rect.x, curY*rect.y, 270*rect.x, 15*rect.y),"Remove Tech")){
												ModifyGBT(myTarget.techProduction.techs.Length-1,myTarget.techProduction.techs.Length, arraySelect);
											}
											curY += 20;
											arraySelect2 = EditorGUI.Popup(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Production : ", arraySelect2, produceNames);
											curY += 20;
											if(arraySelect2 >= myTarget.techProduction.techs.Length && myTarget.techProduction.techs.Length > 0){
												arraySelect2 = myTarget.techProduction.techs.Length-1;
											}
											else{
												if(myTarget.techProduction.techs[arraySelect2] == null){
													myTarget.techProduction.techs[arraySelect2] = new ProduceTech();
												}
												if(myTarget.techProduction.techs[arraySelect2].customName == null){
													myTarget.techProduction.techs[arraySelect2].customName = "";
												}
												
												if(arraySelect2 > myTarget.techProduction.techs.Length){
													arraySelect2 = 0;
												}
												myTarget.techProduction.techs[arraySelect2].customName = EditorGUI.TextField(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Custom Name : ", myTarget.techProduction.techs[arraySelect2].customName);
												curY += 30;
												EditorGUI.LabelField(new Rect(400*rect.x, curY*rect.y, 150*rect.x, 100*rect.y), "Custom Texture : ");
												myTarget.techProduction.techs[arraySelect2].customTexture = EditorGUI.ObjectField(new Rect(600*rect.x, curY*rect.y, 150*rect.x, 100*rect.y),
													myTarget.techProduction.techs[arraySelect2].customTexture, typeof(Texture2D), true) as Texture2D;
												curY += 110;
												
												myTarget.techProduction.techs[arraySelect2].index = EditorGUI.Popup(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Tech To Produce : ", myTarget.techProduction.techs[arraySelect2].index, nameArray);
												if(myTarget.techProduction.techs[arraySelect2].index >= size || myTarget.techProduction.techs[arraySelect2].index < 0){
													myTarget.techProduction.techs[arraySelect2].index = 0;
												}
												else{
													myTarget.techProduction.techs[arraySelect2].techName = names[myTarget.techProduction.techs[arraySelect2].index];
												}
												curY += 20;
												myTarget.techProduction.techs[arraySelect2].canProduce = EditorGUI.Toggle(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Can Produce : ", myTarget.techProduction.techs[arraySelect2].canProduce);
												curY += 20;
												myTarget.techProduction.techs[arraySelect2].dur = EditorGUI.FloatField(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Duration : ", myTarget.techProduction.techs[arraySelect2].dur);
												curY += 25;
												myTarget.techProduction.techs[arraySelect2].rate = EditorGUI.FloatField(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Rate : ", myTarget.techProduction.techs[arraySelect2].rate);
												curY += 30;
												myTarget.techProduction.techs[arraySelect2].amount = EditorGUI.FloatField(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Amount : ", myTarget.techProduction.techs[arraySelect2].amount);
												curY += 30;
												ResourceManager mg = GameObject.Find("Player Manager").GetComponent<ResourceManager>();
												if(myTarget.techProduction.techs[arraySelect2].cost.Length != mg.resourceTypes.Length){
													myTarget.techProduction.techs[arraySelect2].cost = new int[mg.resourceTypes.Length];
												}
												string[] costs = new string[mg.resourceTypes.Length];
												for(int x = 0; x < costs.Length; x++){
													costs[x] = mg.resourceTypes[x].name;
												}
												arraySelect3 = EditorGUI.Popup(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), "Resource Type : ", arraySelect3, costs);
												curY += 25;
												if(arraySelect3 >= costs.Length){
													arraySelect3 = 0;
												}
												else{
													myTarget.techProduction.techs[arraySelect2].cost[arraySelect3] = EditorGUI.IntField(new Rect(400*rect.x, curY*rect.y, 545*rect.x, 25*rect.y), 
														"Cost to Produce : ", myTarget.techProduction.techs[arraySelect2].cost[arraySelect3]);
													curY += 20;
												}
											}
										}
										else{
											if(GUI.Button(new Rect(400*rect.x, curY*rect.y, 270*rect.x, 15*rect.y),"Add Tech")){
												ModifyGBT(myTarget.techProduction.techs.Length+1, myTarget.techProduction.techs.Length, arraySelect);
											}
											if(GUI.Button(new Rect(670*rect.x, curY*rect.y, 270*rect.x, 15*rect.y),"Remove Tech")){
												ModifyGBT(myTarget.techProduction.techs.Length-1,myTarget.techProduction.techs.Length, arraySelect);
											}
										}
									}
								}
							}
							// Size
							else if(subMenuState == 3){
								helpState = 13;
								GUI.DrawTexture(new Rect(840*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								nTarget.buildingList[unitId].closeWidth = EditorGUI.IntField(new Rect(400*rect.x, 70*rect.y, 1100*rect.x, 25*rect.y), "Close Width : ", nTarget.buildingList[unitId].closeWidth);
								nTarget.buildingList[unitId].closeLength = EditorGUI.IntField(new Rect(400*rect.x, 100*rect.y, 1100*rect.x, 25*rect.y), "Close Length : ", nTarget.buildingList[unitId].closeLength);
								int arraySize = (nTarget.buildingList[unitId].closeWidth*2+1)*(nTarget.buildingList[unitId].closeLength*2+1);
								if(nTarget.buildingList[unitId].closePoints.Length != arraySize){
									nTarget.buildingList[unitId].closePoints = new int[arraySize];
									for(int x = 0; x < arraySize; x++){
										nTarget.buildingList[unitId].closePoints[x] = 2;
									}
								}
								groupPosition2 = GUI.BeginScrollView(new Rect(400*rect.x, 130*rect.y, 1100*rect.x, 570*rect.y), groupPosition2, new Rect(0, 0, 30*(nTarget.buildingList[unitId].closeWidth*2+1)+60, 30*(nTarget.buildingList[unitId].closeLength*2+1)+60));
								for(int x = 0; x < nTarget.buildingList[unitId].closeWidth*2+1; x++){
									GUI.Label(new Rect(30+30*x, 0, 30, 30), "" + x);
								}
								for(int y = 0; y < nTarget.buildingList[unitId].closeLength*2+1; y++){
									GUI.Label(new Rect(0, 30+30*y, 30, 30), "" + y);
								}
								for(int x = 0; x < nTarget.buildingList[unitId].closeWidth*2+1; x++){
									for(int y = 0; y < nTarget.buildingList[unitId].closeLength*2+1; y++){
										int i = x*(nTarget.buildingList[unitId].closeLength*2+1)+y;
										if(nTarget.buildingList[unitId].closePoints[i] == 0){
											if(GUI.Button(new Rect(30+30*x, 30+30*y, 30, 30), "O")){
												nTarget.buildingList[unitId].closePoints[i] = 1;	
											}
										}
										else if(nTarget.buildingList[unitId].closePoints[i] == 1){
											if(GUI.Button(new Rect(30+30*x, 30+30*y, 30, 30), "W")){
												nTarget.buildingList[unitId].closePoints[i] = 2;	
											}
										}
										else if(nTarget.buildingList[unitId].closePoints[i] == 2){
											if(GUI.Button(new Rect(30+30*x, 30+30*y, 30, 30), "X")){
												nTarget.buildingList[unitId].closePoints[i] = 0;	
											}
										}
									}
								}
								for(int x = 0; x < nTarget.buildingList[unitId].closeWidth*2+1; x++){
									GUI.Label(new Rect(30+30*x, 30+30*(nTarget.buildingList[unitId].closeLength*2+1), 30, 30), "" + x);
								}
								for(int y = 0; y < nTarget.buildingList[unitId].closeLength*2+1; y++){
									GUI.Label(new Rect(30+30*(nTarget.buildingList[unitId].closeWidth*2+1), 30+30*y, 30, 30), "" + y);
								}
								GUI.EndScrollView();
							}
							// Generate
							else if(subMenuState == 4){
								GUI.DrawTexture(new Rect(1060*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								// Put Button Here for Resource Generate
								ResourceGenerate gen = nTarget.buildingList[unitId].obj.GetComponent<ResourceGenerate>();
								if(gen != null){
									ResourceManager rm = GameObject.Find("Player Manager").GetComponent<ResourceManager>();
									if(rm != null){
										if(gen.resource.Length != rm.resourceTypes.Length){
											gen.resource = new ResourceG[rm.resourceTypes.Length];
										}
										else{
											string[] types = new string[rm.resourceTypes.Length];
											for(int x = 0; x < types.Length; x++){
												types[x] = rm.resourceTypes[x].name;
											}
											arraySelect = EditorGUI.Popup(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Resource Type : ", arraySelect, types);
											if(arraySelect >= types.Length){
												arraySelect = 0;
											}
											else{
												gen.resource[arraySelect].amount = EditorGUI.IntField(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Amount : ", gen.resource[arraySelect].amount);
												gen.resource[arraySelect].rate = EditorGUI.FloatField(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 25*rect.y), "Rate : ", gen.resource[arraySelect].rate);
											}
										}
									}
								}
								else{
									if(GUI.Button(new Rect(400*rect.x, 50*rect.y, 540*rect.x, 650*rect.y), "Add Resource Generator")){
										nTarget.buildingList[unitId].obj.AddComponent<ResourceGenerate>();
									}
								}
							}
							else if(subMenuState == 5){
								GUI.DrawTexture(new Rect(1280*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								ResourceDropOff dropOff = nTarget.buildingList[unitId].obj.GetComponent<ResourceDropOff>();
								if(dropOff != null){
									ResourceManager rm = GameObject.Find("Player Manager").GetComponent<ResourceManager>();
									if(rm != null){
										if(dropOff.type.Length != rm.resourceTypes.Length){
											dropOff.type = new bool[rm.resourceTypes.Length];
										}
										else{
											string[] types = new string[rm.resourceTypes.Length];
											for(int x = 0; x < types.Length; x++){
												types[x] = rm.resourceTypes[x].name;
											}
											arraySelect = EditorGUI.Popup(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Resource Type : ", arraySelect, types);
											if(arraySelect >= types.Length){
												arraySelect = 0;
											}
											else{
												dropOff.type[arraySelect] = EditorGUI.Toggle(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Drop Off : ", dropOff.type[arraySelect]);
											}
										}
									}
								}
								else{
									if(GUI.Button(new Rect(400*rect.x, 50*rect.y, 540*rect.x, 650*rect.y), "Add Drop Off")){
										nTarget.buildingList[unitId].obj.AddComponent<ResourceDropOff>();
									}
								}
							}
						}
						// GUI
						else if(menuState == 1){
							if(nTarget.buildingList[unitId].obj != lastObj){
								objEditor = Editor.CreateEditor(nTarget.buildingList[unitId].obj);
								lastObj = nTarget.buildingList[unitId].obj;
							}
							objEditor.OnInteractivePreviewGUI(new Rect(945*rect.x, 50*rect.y, 555*rect.x, 650*rect.y), EditorStyles.toolbarButton);
							GUI.DrawTexture(new Rect(675*rect.x, 0, 275*rect.x, 25*rect.y), selectionTexture, ScaleMode.StretchToFill);
							if(GUI.Button(new Rect(400*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Selected")){
								subMenuState = 0;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(620*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Production GUI")){
								subMenuState = 1;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(840*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "MiniMap")){
								subMenuState = 2;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(1060*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Vision")){
								subMenuState = 3;
								arraySelect = 0;
							}
							if(GUI.Button(new Rect(1280*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), "Progress")){
								subMenuState = 4;
								arraySelect = 0;
							}
							// Selected
							if(subMenuState == 0){
								GUI.DrawTexture(new Rect(400*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								myTarget.gui.SetType("Unit");
								EditorGUI.LabelField(new Rect(400*rect.x, 50*rect.y, 545*rect.x, 100*rect.y), "GUI Image : ");
								myTarget.gui.image = EditorGUI.ObjectField(new Rect(400*rect.x, 80*rect.y, 100*rect.x, 100*rect.y), "", myTarget.gui.image, typeof(Texture2D), true) as Texture2D;
								//Modify Selected Objects
								if(GUI.Button(new Rect(400*rect.x, 180*rect.y, 273*rect.x, 25*rect.y), "Add")){
									ModifyBS(myTarget.gui.selectObjs.Length+1, myTarget.gui.selectObjs.Length, myTarget);
								}
								if(GUI.Button(new Rect(673*rect.x, 180*rect.y, 272*rect.x, 25*rect.y), "Remove")){
									ModifyBS(myTarget.gui.selectObjs.Length-1, myTarget.gui.selectObjs.Length, myTarget);
								}
								string[] list = new string[myTarget.gui.selectObjs.Length];
								for(int x = 0; x < list.Length; x++){
									if(myTarget.gui.selectObjs[x]){
										list[x] = (x+1) + ". " + myTarget.gui.selectObjs[x].name;
									}
									else{
										list[x] = (x+1) + ". Empty";
									}
								}
								arraySelect = EditorGUI.Popup(new Rect(400*rect.x, 210*rect.y, 545*rect.x, 25*rect.y), "Objects : ", arraySelect, list);
								if(arraySelect >= myTarget.gui.selectObjs.Length){
									arraySelect = 0;
								}
								else{
									myTarget.gui.selectObjs[arraySelect] = EditorGUI.ObjectField(new Rect(400*rect.x, 240*rect.y, 545*rect.x, 25*rect.y), "", myTarget.gui.selectObjs[arraySelect], typeof(GameObject)) as GameObject;
								}
								Health healthObj = nTarget.buildingList[unitId].obj.GetComponent<Health>();
								if(healthObj == null){
									if(GUI.Button(new Rect(400*rect.x, 280*rect.y, 545*rect.x, 420*rect.y), "Add Health Indicator")){
										nTarget.buildingList[unitId].obj.AddComponent<Health>();
									}
								}
								else{
									healthObj.backgroundBar = EditorGUI.ObjectField(new Rect(400*rect.x, 280*rect.y, 100*rect.x, 50*rect.y), healthObj.backgroundBar, typeof(Texture2D), true) as Texture2D;
									healthObj.healthBar = EditorGUI.ObjectField(new Rect(400*rect.x, 330*rect.y, 100*rect.x, 50*rect.y), healthObj.healthBar, typeof(Texture2D), true) as Texture2D;
									healthObj.yIncrease = EditorGUI.FloatField(new Rect(400*rect.x, 390*rect.y, 545*rect.x, 30*rect.y), "World-Y Increase : ", healthObj.yIncrease);
									healthObj.scale = EditorGUI.IntField(new Rect(400*rect.x, 420*rect.y, 545*rect.x, 30*rect.y), "UI-X Scale : ", healthObj.scale);
									healthObj.yScale = EditorGUI.IntField(new Rect(400*rect.x, 450*rect.y, 545*rect.x, 30*rect.y), "UI-Y Scale : ", healthObj.yScale);
									int healthLength = healthObj.element.Length;
									healthLength = EditorGUI.IntField(new Rect(400*rect.x, 485*rect.y, 545*rect.x, 30*rect.y), "Health Elements : ", healthLength);
									if(healthLength != healthObj.element.Length){
										ModifyHE(healthLength, healthObj.element.Length, healthObj);
									}
									if(healthLength > 0){
										string[] elementName = new string[healthLength];
										for(int x = 0; x < elementName.Length; x++){
											elementName[x] = "Element " + (x+1);
										}
										arraySelect1 = EditorGUI.Popup(new Rect(400*rect.x, 520*rect.y, 545*rect.x, 25*rect.y), "Element : ", arraySelect1, elementName);
										healthObj.element[arraySelect1].image = EditorGUI.ObjectField(new Rect(400*rect.x, 540*rect.y, 100*rect.x, 50*rect.y), healthObj.element[arraySelect1].image, typeof(Texture2D), true) as Texture2D;
										healthObj.element[arraySelect1].loc = EditorGUI.RectField(new Rect(400*rect.x, 600*rect.y, 545*rect.x, 50*rect.y), healthObj.element[arraySelect1].loc);
									}
									Vector2 point = new Vector2(620*rect.x, 650*rect.y);
									if(healthObj.backgroundBar != null){
										GUI.DrawTexture(new Rect(point.x, point.y, healthObj.scale*((float)1), healthObj.yScale), healthObj.backgroundBar);
									}
									if(healthObj.healthBar != null){
										GUI.DrawTexture(new Rect(point.x, point.y, healthObj.scale*((float)50/100), healthObj.yScale), healthObj.healthBar);
									}
									for(int x = 0; x < healthObj.element.Length; x++){
										GUI.DrawTexture(new Rect(point.x+healthObj.element[x].loc.x, (point.y-healthObj.element[x].loc.y), healthObj.element[x].loc.width, healthObj.element[x].loc.height), healthObj.element[x].image); 
									}
								}
								helpState = 9;
							}
							// Production GUI
							else if(subMenuState == 1){
								GUI.DrawTexture(new Rect(620*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								string[] name = {"Units", "Techs", "Jobs"};
								arraySelect = EditorGUI.Popup(new Rect(400*rect.x, 50*rect.y, 550*rect.x, 20*rect.y), "Type : ", arraySelect, name);
								if(arraySelect == 0){
									myTarget.bGUI.unitGUI.startPos = EditorGUI.Vector2Field(new Rect(400*rect.x, 75*rect.y, 545*rect.x, 25*rect.y), "Start Pos : ", myTarget.bGUI.unitGUI.startPos);
									myTarget.bGUI.unitGUI.buttonSize = EditorGUI.Vector2Field(new Rect(400*rect.x, 120*rect.y, 545*rect.x, 25*rect.y), "Button Size : ", myTarget.bGUI.unitGUI.buttonSize);
									myTarget.bGUI.unitGUI.buttonPerRow = EditorGUI.IntField(new Rect(400*rect.x, 165*rect.y, 545*rect.x, 25*rect.y), "Button Per Row : ", myTarget.bGUI.unitGUI.buttonPerRow);
									myTarget.bGUI.unitGUI.displacement = EditorGUI.Vector2Field(new Rect(400*rect.x, 210*rect.y, 545*rect.x, 25*rect.y), "Displacement : ", myTarget.bGUI.unitGUI.displacement);
								}
								else if(arraySelect == 1){
									myTarget.bGUI.technologyGUI.startPos = EditorGUI.Vector2Field(new Rect(400*rect.x, 75*rect.y, 545*rect.x, 25*rect.y), "Start Pos : ", myTarget.bGUI.technologyGUI.startPos);
									myTarget.bGUI.technologyGUI.buttonSize = EditorGUI.Vector2Field(new Rect(400*rect.x, 120*rect.y, 545*rect.x, 25*rect.y), "Button Size : ", myTarget.bGUI.technologyGUI.buttonSize);
									myTarget.bGUI.technologyGUI.buttonPerRow = EditorGUI.IntField(new Rect(400*rect.x, 165*rect.y, 545*rect.x, 25*rect.y), "Button Per Row : ", myTarget.bGUI.technologyGUI.buttonPerRow);
									myTarget.bGUI.technologyGUI.displacement = EditorGUI.Vector2Field(new Rect(400*rect.x, 210*rect.y, 545*rect.x, 25*rect.y), "Displacement : ", myTarget.bGUI.technologyGUI.displacement);
								}
								else if(arraySelect == 2){
									myTarget.bGUI.jobsGUI.startPos = EditorGUI.Vector2Field(new Rect(400*rect.x, 75*rect.y, 545*rect.x, 25*rect.y), "Start Pos : ", myTarget.bGUI.jobsGUI.startPos);
									myTarget.bGUI.jobsGUI.buttonSize = EditorGUI.Vector2Field(new Rect(400*rect.x, 120*rect.y, 545*rect.x, 25*rect.y), "Button Size : ", myTarget.bGUI.jobsGUI.buttonSize);
									myTarget.bGUI.jobsGUI.buttonPerRow = EditorGUI.IntField(new Rect(400*rect.x, 165*rect.y, 545*rect.x, 25*rect.y), "Button Per Row : ", myTarget.bGUI.jobsGUI.buttonPerRow);
									myTarget.bGUI.jobsGUI.displacement = EditorGUI.Vector2Field(new Rect(400*rect.x, 210*rect.y, 545*rect.x, 25*rect.y), "Displacement : ", myTarget.bGUI.jobsGUI.displacement);
								}
							}
							// MiniMap
							else if(subMenuState == 2){
								GUI.DrawTexture(new Rect(840*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								if(myTargetMap == null){
									if(GUI.Button(new Rect(400*rect.x, 45*rect.y, 1100*rect.x, 705*rect.y), "Add MiniMap Components")){
										nTarget.buildingList[unitId].obj.AddComponent("MiniMapSignal");
									}
								}
								else{
									myTargetMap.enabled = EditorGUI.Toggle(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "MiniMap Signal : ", myTargetMap.enabled);
									if(myTargetMap.enabled){
										myTargetMap.miniMapTag = EditorGUI.TextField(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Tag : ", myTargetMap.miniMapTag);
										GameObject mapObj = GameObject.Find("MiniMap");
										Color defaultColor = GUI.color;
										if(mapObj != null){
											MiniMap map = GameObject.Find("MiniMap").GetComponent<MiniMap>();
											if(map != null){
												helpState = 10;
												for(int x = 0; x < map.elements.Length; x++){
													if(map.elements[x].tag == myTargetMap.miniMapTag){
														if(map.elements[x].tints.Length > miniMapState){
															GUI.color = map.elements[x].tints[miniMapState];
														}
														else{
															GUI.color = map.elements[x].tints[0];
														}
														int size = (int)(50*rect.x);
														GUI.DrawTexture(new Rect(400*rect.x, 130*rect.y, size, size), map.elements[x].image);
														GUI.color = defaultColor;
														map.elements[x].image = EditorGUI.ObjectField(new Rect(400*rect.x, 185*rect.y, 100, 100), map.elements[x].image, typeof(Texture2D), true) as Texture2D;
														miniMapState = EditorGUI.IntField(new Rect(400*rect.x, 320*rect.y, 540*rect.x, 25*rect.y), "Group : ", miniMapState);
													}
												}
											}
											else{
												helpState = 11;
											}
										}
									}
								}
							}
							// Fog Of War
							else if(subMenuState == 3){
								GUI.DrawTexture(new Rect(1060*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								if(myTargetVision == null){
									if(GUI.Button(new Rect(400*rect.x, 45*rect.y, 1100*rect.x, 705*rect.y), "Add Vision Components")){
										nTarget.buildingList[unitId].obj.AddComponent("VisionSignal");
										// Add More Components
									}
								}
								else{
									myTargetVision.enabled = EditorGUI.Toggle(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Vision Signal : ", myTargetVision.enabled);
									if(myTargetVision.enabled){
										myTargetVision.radius = EditorGUI.IntField(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Radius : ", myTargetVision.radius);
										myTargetVision.upwardSightHeight = EditorGUI.IntField(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 25*rect.y), "Upward Sight Height : ", myTargetVision.upwardSightHeight);
										myTargetVision.downwardSightHeight = EditorGUI.IntField(new Rect(400*rect.x, 160*rect.y, 540*rect.x, 25*rect.y), "Downward Sight Height : ", myTargetVision.downwardSightHeight);
									}
									helpState = 12;
								}
							}
							// Progress Indicator
							else if(subMenuState == 4){
								GUI.DrawTexture(new Rect(1280*rect.x, 25*rect.y, 220*rect.x, 20*rect.y), selectionTexture, ScaleMode.StretchToFill);
								Progress progressObj = nTarget.buildingList[unitId].progressObj.GetComponent<Progress>();
								if(progressObj == null){
									if(GUI.Button(new Rect(400*rect.x, 50*rect.y, 545*rect.x, 750*rect.y), "Add Progress Indicator")){
										nTarget.buildingList[unitId].progressObj.AddComponent<Progress>();
									}
								}
								else{
									progressObj.yIncrease = EditorGUI.FloatField(new Rect(400*rect.x, 70*rect.y, 545*rect.x, 30*rect.y), "World-Y Increase : ", progressObj.yIncrease);
									progressObj.scale = EditorGUI.IntField(new Rect(400*rect.x, 100*rect.y, 545*rect.x, 30*rect.y), "UI-X Scale : ", progressObj.scale);
									progressObj.yScale = EditorGUI.IntField(new Rect(400*rect.x, 130*rect.y, 545*rect.x, 30*rect.y), "UI-Y Scale : ", progressObj.yScale);
									progressObj.color = EditorGUI.ColorField(new Rect(400*rect.x, 160*rect.y, 545*rect.x, 30*rect.y), "Color : ", progressObj.color);
									int nl = progressObj.texture.Length;
									nl = EditorGUI.IntField(new Rect(400*rect.x, 190*rect.y, 545*rect.x, 30*rect.y), "Textures : ", nl);
									if(nl != progressObj.texture.Length){
										ModifyP(nl, progressObj.texture.Length, progressObj);
									}
									if(nl > 0){
										string[] textureName = new string[nl];
										for(int x = 0; x < nl; x++){
											textureName[x] = "Texture " + (x+1);
										}
										if(arraySelect1 > nl){
											arraySelect1 = nl-1;
										}
										arraySelect1 = EditorGUI.Popup(new Rect(400*rect.x, 220*rect.y, 545*rect.x, 25*rect.y), "Element : ", arraySelect1, textureName);
										progressObj.texture[arraySelect1] = EditorGUI.ObjectField(new Rect(400*rect.x, 250*rect.y, 100*rect.x, 50*rect.y), progressObj.texture[arraySelect1], typeof(Texture2D), true) as Texture2D;
									}
								}
							}
						}
						// Techs
						else if(menuState == 2){
							GUI.DrawTexture(new Rect(950*rect.x, 0, 275*rect.x, 25*rect.y), selectionTexture, ScaleMode.StretchToFill);
							GUI.Box(new Rect(400*rect.x, 25*rect.y, 1100*rect.x, 20*rect.y), "");
							if(GUI.Button(new Rect(400*rect.x, 50*rect.y, 550*rect.x, 20*rect.y), "Add")){
								ModifyBT(myTarget.techEffect.Length+1, myTarget.techEffect.Length, myTarget);
							}
							if(GUI.Button(new Rect(950*rect.x, 50*rect.y, 550*rect.x, 20*rect.y), "Remove")){
								ModifyBT(myTarget.techEffect.Length-1, myTarget.techEffect.Length, myTarget);						
							}
							if(nTarget.tech.Length > 0 && myTarget.techEffect.Length > 0){
								int size = nTarget.tech.Length;
								string[] nameArray = new string[size];
								string[] names = new string[size];
								for(int x = 0; x < nameArray.Length; x++){
									nameArray[x] = (x+1) + ". " + nTarget.tech[x].name;
									names[x] = nTarget.tech[x].name;
								}
								string[] unitTechs = new string[myTarget.techEffect.Length];
								for(int x = 0; x < unitTechs.Length; x++){
									unitTechs[x] = (x+1) + ". " + myTarget.techEffect[x].name;
								}
								arraySelect = EditorGUI.Popup(new Rect(400*rect.x, 75*rect.y, 1100*rect.x, 20*rect.y), "Techs : ", arraySelect, unitTechs);
								if(arraySelect >= myTarget.techEffect.Length){
									arraySelect = 0;
								}
								int curY = 100;
								arraySelect2 = myTarget.techEffect[arraySelect].index;
								arraySelect2 = EditorGUI.Popup(new Rect(400*rect.x, curY*rect.y, 1100*rect.x, 20*rect.y), "Tech Options : ", arraySelect2, nameArray);
								curY += 25;
								myTarget.techEffect[arraySelect].index = arraySelect2;
								if(arraySelect2 > nameArray.Length){
									arraySelect2 = 0;
								}
								myTarget.techEffect[arraySelect].name = names[arraySelect2];
								myTarget.techEffect[arraySelect].replacementObject = EditorGUI.ObjectField(new Rect(400*rect.x, curY*rect.y, 1100*rect.x, 20*rect.y), "Replacement Object : ", myTarget.techEffect[arraySelect].replacementObject, typeof(GameObject)) as GameObject;
								curY += 25;
								string[,] functions = 	new string[18,4]{			
								/* 0 */		{"GetName", "SetName", "", ""}, 
								/* 1 */		{"GetMaxHealth", "SetMaxHealth", "AddMaxHealth", "SubMaxHealth"},
								/* 2 */		{"GetHealth", "SetHealth", "AddHealth", "SubHealth"},
								/* 3 */		{"GetGroup", "SetGroup", "", ""},
								/* 4 */		{"GetUCanProduce", "SetUCanProduce", "", ""},
								/* 5 */		{"GetUICanProduce", "SetUICanProduce", "", ""},
								/* 6 */		{"GetUCost", "SetUCost", "AddUCost", "SubUCost"},
								/* 7 */		{"GetUDur", "SetUDur", "AddUDur", "SubUDur"},
								/* 8 */		{"GetURate", "SetURate", "AddURate", "SubURate"},
								/* 9 */		{"GetUCanBuildAtOnce", "SetUCanBuildAtOnce", "AddUCanBuildAtOnce", "SubUCanBuildAtOnce"},
								/* 10 */	{"GetUMaxAmount", "SetUMaxAmount", "AddUMaxAmount", "SubUMaxAmount"},
								/* 11 */	{"GetTCanProduce", "SetTCanProduce", "", ""},
								/* 12 */	{"GetTCost", "SetTCost", "AddTCost", "SubTCost"},
								/* 13 */	{"GetTICanProduce", "SetTICanProduce", "", ""},
								/* 14 */	{"GetTDur", "SetTDur", "AddTDur", "SubTDur"},
								/* 15 */	{"GetTRate", "SetTRate", "AddTRate", "SubTRate"},
								/* 16 */	{"GetTMaxAmount", "SetTMaxAmount", "AddTMaxAmount", "SubTMaxAmount"},
								/* 17 */	{"GetTCanBuildAtOnce", "SetTCanBuildAtOnce", "AddTCanBuildAtOnce", "SubTCanBuildAtOnce"},
											};
								string[] variableName = {"Name", "Max Health", "Health", "Group", 
														 "UCanProduce", "UICanProduce", "UCost", "UDur", "URate", 
														 "UCanBuildAtOnce", "UMaxAmount",
														 "TCanProduce", "TCost", "TICanProduce", "TDur",
														 "TRate", "TMaxAmount", "TCanBuildAtOnce"
														};
								for(int x = 0; x < myTarget.techEffect[arraySelect].effects.Length; x++){
									if(myTarget.techEffect[arraySelect].effects[x] == null){
										myTarget.techEffect[arraySelect].effects[x] = new Effects();
									}
									myTarget.techEffect[arraySelect].effects[x].effectName = EditorGUI.Popup(new Rect(400*rect.x, curY*rect.y, 400*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].effectName, variableName);
									int index = myTarget.techEffect[arraySelect].effects[x].effectName;
									//{
									string[] funcTypes;
									if(index == 0 || index == 3 || index == 4 || index == 5 || index == 11 || index == 13){
										funcTypes = new string[2];
										funcTypes[0] = "Get";
										funcTypes[1] = "Set";
									}
									else{
										funcTypes = new string[4];
										funcTypes[0] = "Get";
										funcTypes[1] = "Set";
										funcTypes[2] = "Add";
										funcTypes[3] = "Sub";
									}
									myTarget.techEffect[arraySelect].effects[x].funcType = EditorGUI.Popup(new Rect(800*rect.x, curY*rect.y, 200*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].funcType, funcTypes);
									myTarget.techEffect[arraySelect].effects[x].funcName = functions[index, myTarget.techEffect[arraySelect].effects[x].funcType];
									//}
									if(myTarget.techEffect[arraySelect].effects[x].funcType == 0){
										EditorGUI.LabelField(new Rect(1000*rect.x, curY*rect.y, 290*rect.x, 20*rect.y), "Getter");
									}
									else{
										string[] produceNames = new string[myTarget.unitProduction.units.Length];
										for(int y = 0; y < myTarget.unitProduction.units.Length; y++){
											if(myTarget.unitProduction.units[y] != null){
												produceNames[y] = (y+1) + ". " + myTarget.unitProduction.units[y].customName;
											}
										}
										string[] produceTNames = new string[myTarget.techProduction.techs.Length];
										for(int y = 0; y < myTarget.techProduction.techs.Length; y++){
											if(myTarget.techProduction.techs[y] != null){
												produceTNames[y] = (y+1) + ". " + myTarget.techProduction.techs[y].customName;
											}
										}
										if(index == 0){
											myTarget.techEffect[arraySelect].effects[x].text = EditorGUI.TextField(new Rect(1000*rect.x, curY*rect.y, 290*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].text);
										}
										else if(index == 4 || index == 11){
											myTarget.techEffect[arraySelect].effects[x].toggle = EditorGUI.Toggle(new Rect(1000*rect.x, curY*rect.y, 290*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].toggle);
										}
										else if(index == 5){
											myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(new Rect(1290*rect.x, curY*rect.y, 210*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].index, produceNames);
											myTarget.techEffect[arraySelect].effects[x].toggle = EditorGUI.Toggle(new Rect(1000*rect.x, curY*rect.y, 290*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].toggle);
										}
										else if(index == 13){
											myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(new Rect(1290*rect.x, curY*rect.y, 210*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].index, produceTNames);
											myTarget.techEffect[arraySelect].effects[x].toggle = EditorGUI.Toggle(new Rect(1000*rect.x, curY*rect.y, 290*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].toggle);
										}
										else if(index == 7 || index == 8){
											myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(new Rect(1290*rect.x, curY*rect.y, 210*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].index, produceNames);
											myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(new Rect(1000*rect.x, curY*rect.y, 290*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].amount);
										}
										else if(index == 14 || index == 15){
											myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(new Rect(1290*rect.x, curY*rect.y, 210*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].index, produceTNames);
											myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(new Rect(1000*rect.x, curY*rect.y, 290*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].amount);
										}
										else if(index == 6 || index == 12){
											// Resource List
											GameObject resourceManager = GameObject.Find("Player Manager");
											if(resourceManager != null){
												ResourceManager rMScript= resourceManager.GetComponent<ResourceManager>();
												if(rMScript){
													string[] resourceNames = new string[rMScript.resourceTypes.Length];
													for(int y = 0; y < resourceNames.Length; y++){
														resourceNames[y] = rMScript.resourceTypes[y].name;
													}
													if(arraySelect >= resourceNames.Length){
														arraySelect = 0; 
													}
													else{
														if(myTarget.techEffect[arraySelect].effects[x].index >= resourceNames.Length){
															myTarget.techEffect[arraySelect].effects[x].index = 0;
														}
														if(index == 6){
															myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(new Rect(1290*rect.x, curY*rect.y, 100*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].index, produceNames);
															myTarget.techEffect[arraySelect].effects[x].index1 = EditorGUI.Popup(new Rect(1390*rect.x, curY*rect.y, 110*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].index1, resourceNames);
															myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(new Rect(1000*rect.x, curY*rect.y, 290*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].amount);
														}
														else{
															myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(new Rect(1290*rect.x, curY*rect.y, 100*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].index, produceTNames);
															myTarget.techEffect[arraySelect].effects[x].index1 = EditorGUI.Popup(new Rect(1390*rect.x, curY*rect.y, 110*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].index1, resourceNames);
															myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(new Rect(1000*rect.x, curY*rect.y, 290*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].amount);
														}
													}
												}
											}
										}
										else{
											myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(new Rect(1000*rect.x, curY*rect.y, 290*rect.x, 20*rect.y), "", myTarget.techEffect[arraySelect].effects[x].amount);
										}
									}
									curY += 25;
								}
								if(GUI.Button(new Rect(400*rect.x, curY*rect.y, 273*rect.x, 20*rect.y), "Add")){
									ModifyBTE(myTarget.techEffect[arraySelect].effects.Length+1, myTarget.techEffect[arraySelect].effects.Length, myTarget, arraySelect);
								}
								if(GUI.Button(new Rect(673*rect.x, curY*rect.y, 272*rect.x, 20*rect.y), "Remove")){
									ModifyBTE(myTarget.techEffect[arraySelect].effects.Length-1, myTarget.techEffect[arraySelect].effects.Length, myTarget, arraySelect);
								}
							}
						}
						// Anim/Sounds
						else if(menuState == 3){
							if(nTarget.buildingList[unitId].obj != lastObj){
								objEditor = Editor.CreateEditor(nTarget.buildingList[unitId].obj);
								lastObj = nTarget.buildingList[unitId].obj;
							}
							objEditor.OnInteractivePreviewGUI(new Rect(945*rect.x, 50*rect.y, 555*rect.x, 650*rect.y), EditorStyles.toolbarButton);
							GUI.DrawTexture(new Rect(1225*rect.x, 0, 275*rect.x, 25*rect.y), selectionTexture, ScaleMode.StretchToFill);
							GUI.Box(new Rect(400*rect.x, 25*rect.y, 1100*rect.x, 20*rect.y), "");
							Animator comp = nTarget.buildingList[unitId].obj.GetComponent<Animator>();
							if(myTarget.anim.manager){
								myTarget.anim.manager.runtimeAnimatorController = EditorGUI.ObjectField(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Controller : ", myTarget.anim.manager.runtimeAnimatorController, typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
							}
							else if(comp){
								myTarget.anim.manager = comp;
							}
							else{
								if(GUI.Button(new Rect(400*rect.x, 70*rect.y, 540*rect.x, 25*rect.y), "Add Animator")){
									nTarget.buildingList[unitId].obj.AddComponent<Animator>();
								}
							}
							myTarget.anim.idleAudio = EditorGUI.ObjectField(new Rect(400*rect.x, 100*rect.y, 540*rect.x, 25*rect.y), "Idle Audio : ", myTarget.anim.idleAudio, typeof(AudioClip)) as AudioClip;
							myTarget.anim.buildUnitAudio = EditorGUI.ObjectField(new Rect(400*rect.x, 130*rect.y, 540*rect.x, 25*rect.y), "Build Unit : ", myTarget.anim.buildUnitAudio, typeof(AudioClip)) as AudioClip;
							myTarget.anim.buildTechAudio = EditorGUI.ObjectField(new Rect(400*rect.x, 160*rect.y, 540*rect.x, 25*rect.y), "Build Tech : ", myTarget.anim.buildTechAudio, typeof(AudioClip)) as AudioClip;
						}
					}
					else{
						if(GUI.Button(new Rect(400*rect.x, 0, 1100*rect.x, 750*rect.y), "+")){
							nTarget.buildingList[unitId].obj.AddComponent<BuildingController>();
						}
					}
				}
			}
		}
	}
	
	bool isWithin (Rect loc){
		Event e = Event.current;
		if(e.button == 0 && e.isMouse){
			bool rValue = true;
			if(e.mousePosition.x < loc.x){
				rValue = false;
			}
			if(e.mousePosition.x > loc.x+loc.width){
				rValue = false;
			}
			if(e.mousePosition.y < loc.y){
				rValue = false;
			}
			if(e.mousePosition.y > loc.y+loc.height){
				rValue = false;
			}
			return rValue;
		}
		else{
			return false;
		}
	}
	
	// Functions for Modifying custom class arrays
	
	// Group
	void ModifyG (int nl, int ol, int curLoc){
		GameObject[] copyArr = new GameObject[ol];
		for(int x = 0; x < copyArr.Length; x++){
			if(target.groupList[x]){	
				copyArr[x] = target.groupList[x];
			}
		}
		target.groupList = new GameObject[nl];
		int y = 0;
		if(nl < ol){
			for(int x = 0; x < copyArr.Length; x++){
				if(x != curLoc){
					if(copyArr[x]){
						target.groupList[y] = copyArr[x];
					}
					y++;
				}
			}
		}
		else{
			for(int x = 0; x < target.groupList.Length; x++){
				if(x != curLoc+1){
					if(copyArr[y]){
						target.groupList[x] = copyArr[y];	
					}
					y++;
				}
			}
		}
	}
	
	// Modify Techs
	Technology[] ModifyT (int nl, int ol, Technology[] techs){
		Technology[] copyArr = new Technology[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = techs[x];
		}
		techs = new Technology[nl];
		int y = 0;
		if(nl < ol){
			for(int x = 0; x < techs.Length; x++){
				techs[y] = copyArr[x];
				y++;
			}
		}
		else{
			for(int x = 0; x < copyArr.Length; x++){
				techs[x] = copyArr[x];
			}
			techs[techs.Length-1] = new Technology();
		}
		return techs;
	}
	
	// Group Unit
	void ModifyGU (int nl, int ol, int curLoc){
		Unit[] copyArr = new Unit[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = new Unit();
			copyArr[x] = nTarget.unitList[x];
		}
		nTarget.unitList = new Unit[nl];
		int y = 0;
		if(nl < ol){
			for(int x = 0; x < copyArr.Length; x++){
				if(x != curLoc){
					nTarget.unitList[y] = copyArr[x];
					y++;
				}
			}
			for(int x = 0; x < nTarget.unitList.Length; x++){
				if(nTarget.unitList[x] == null){
					nTarget.unitList[x] = new Unit();
				}
			}
		}
		else{
			y = 0;
			for(int x = 0; x < nTarget.unitList.Length; x++){
				if(x != curLoc+1){
					nTarget.unitList[x] = new Unit();
					nTarget.unitList[x] = copyArr[y];	
					y++;
				}
				else{
					nTarget.unitList[x] = new Unit();
				}
			}
		}
	}
	
	// Group Types
	void ModifyGT (int nl, int ol, int curLoc){
		Type[] copyArr = new Type[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = new Type();
			copyArr[x] = target.types[x];
		}
		target.types = new Type[nl];
		int y = 0;
		if(nl < ol){
			for(int x = 0; x < copyArr.Length; x++){
				if(x != curLoc){
					target.types[y] = copyArr[x];
					y++;
				}
			}
		}
		else{
			y = 0;
			for(int x = 0; x < target.types.Length; x++){
				if(x != curLoc+1){
					target.types[x] = new Type();
					target.types[x] = copyArr[y];	
					y++;
				}
				else{
					target.types[x] = new Type();
				}
			}
		}
	}
	
	// Group Types Strengths
	void ModifyGTS (int nl, int ol, int curLoc, int loc){
		Ratio[] copyArr = new Ratio[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = new Ratio();
			copyArr[x] = target.types[loc].strengths[x];
		}
		target.types[loc].strengths = new Ratio[nl];
		int y = 0;
		if(nl < ol){
			for(int x = 0; x < copyArr.Length; x++){
				if(x != curLoc){
					target.types[loc].strengths[y] = copyArr[x];
					y++;
				}
			}
		}
		else{
			y = 0;
			for(int x = 0; x < target.types[loc].strengths.Length; x++){
				if(x != curLoc+1){
					target.types[loc].strengths[x] = new Ratio();
					target.types[loc].strengths[x] = copyArr[y];	
					y++;
				}
				else{
					target.types[loc].strengths[x] = new Ratio();
				}
			}
		}
	}
	
	// Group Types Weaknesses
	void ModifyGTW (int nl, int ol, int curLoc, int loc){
		Ratio[] copyArr = new Ratio[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = new Ratio();
			copyArr[x] = target.types[loc].weaknesses[x];
		}
		target.types[loc].weaknesses = new Ratio[nl];
		int y = 0;
		if(nl < ol){
			for(int x = 0; x < copyArr.Length; x++){
				if(x != curLoc){
					target.types[loc].weaknesses[y] = copyArr[x];
					y++;
				}
			}
		}
		else{
			y = 0;
			for(int x = 0; x < target.types[loc].weaknesses.Length; x++){
				if(x != curLoc+1){
					target.types[loc].weaknesses[x] = new Ratio();
					target.types[loc].weaknesses[x] = copyArr[y];	
					y++;
				}
				else{
					target.types[loc].weaknesses[x] = new Ratio();
				}
			}
		}
	}
	
	// Group Buildings
	void ModifyGB (int nl, int ol, int curLoc){
		Building[] copyArr = new Building[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = new Building();
			copyArr[x] = nTarget.buildingList[x];
		}
		nTarget.buildingList = new Building[nl];
		int y = 0;
		if(nl < ol){
			for(int x = 0; x < copyArr.Length; x++){
				if(x != curLoc){
					nTarget.buildingList[y] = copyArr[x];
					y++;
				}
			}
			for(int x = 0; x < nTarget.buildingList.Length; x++){
				if(nTarget.buildingList[x] == null){
					nTarget.buildingList[x] = new Building();
				}
			}
		}
		else{
			y = 0;
			for(int x = 0; x < nTarget.buildingList.Length; x++){
				if(x != curLoc+1){
					nTarget.buildingList[x] = new Building();
					nTarget.buildingList[x] = copyArr[y];	
					y++;
				}
				else{
					nTarget.buildingList[x] = new Building();
				}
			}
		}
	}
	
	// Group Buildings Unit (Production)
	void ModifyGBU (int nl, int ol, int curLoc){
		BuildingController buildCont = nTarget.buildingList[unitId].obj.GetComponent<BuildingController>();
		ProduceUnit[] copyArr = new ProduceUnit[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = buildCont.unitProduction.units[x];
		}
		buildCont.unitProduction.units = new ProduceUnit[nl];
		int y = 0;
		if(nl < ol){
			for(int x = 0; x < copyArr.Length; x++){
				if(x != curLoc){
					buildCont.unitProduction.units[y] = copyArr[x];
					y++;
				}
			}
		}
		else{
			for(int x = 0; x < buildCont.unitProduction.units.Length; x++){
				if(x != curLoc+1){
					buildCont.unitProduction.units[x] = copyArr[y];	
					y++;
				}
			}
		}
	}

	// Group Buildings Technology (Production)
	void ModifyGBT (int nl, int ol, int curLoc){
		BuildingController buildCont = nTarget.buildingList[unitId].obj.GetComponent<BuildingController>();
		ProduceTech[] copyArr = new ProduceTech[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = buildCont.techProduction.techs[x];
		}
		buildCont.techProduction.techs = new ProduceTech[nl];
		int y = 0;
		if(nl < ol){
			for(int x = 0; x < copyArr.Length; x++){
				if(x != curLoc){
					buildCont.techProduction.techs[y] = copyArr[x];
					y++;
				}
			}
		}
		else{
			for(int x = 0; x < buildCont.techProduction.techs.Length; x++){
				if(x != curLoc+1){
					buildCont.techProduction.techs[x] = copyArr[y];	
					y++;
				}
			}
		}
	}
	
	// Unit Gathering
	void ModifyUR (int nl, int ol, UnitController targ){
		ResourceBehaviour[] copyArr = new ResourceBehaviour[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = targ.resource.behaviour[x];
		}
		targ.resource.behaviour = new ResourceBehaviour[nl];
		if(nl > ol){
			for(int x = 0; x < copyArr.Length; x++){
				targ.resource.behaviour[x] = copyArr[x];
			}
			targ.resource.behaviour[nl-1] = new ResourceBehaviour();
		}
		else{
			for(int x = 0; x < targ.resource.behaviour.Length; x++){
				targ.resource.behaviour[x] = copyArr[x];	
			}
		}
	}
	
	// Unit Tech Effects
	void ModifyUT (int nl, int ol, UnitController targ){
		TechEffect[] copyArr = new TechEffect[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = targ.techEffect[x];
		}
		targ.techEffect = new TechEffect[nl];
		if(nl > ol){
			for(int x = 0; x < copyArr.Length; x++){
				targ.techEffect[x] = copyArr[x];
			}
			targ.techEffect[nl-1] = new TechEffect();
		}
		else{
			for(int x = 0; x < targ.techEffect.Length; x++){
				targ.techEffect[x] = copyArr[x];	
			}
		}
	}
	
	// Unit Ratios
	void ModifyURA (int nl, int ol, UnitController targ){
		SRatio[] copyArr = new SRatio[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = targ.ratio[x];
		}
		targ.ratio = new SRatio[nl];
		if(nl > ol){
			for(int x = 0; x < copyArr.Length; x++){
				targ.ratio[x] = copyArr[x];
			}
			targ.ratio[nl-1] = new SRatio();
		}
		else{
			for(int x = 0; x < targ.ratio.Length; x++){
				targ.ratio[x] = copyArr[x];	
			}
		}
	}
	
	// Unit Building
	void ModifyUB (int nl, int ol, UnitController targ){
		BuildBehaviour[] copyArr = new BuildBehaviour[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = targ.build.build[x];
		}
		targ.build.build = new BuildBehaviour[nl];
		if(nl > ol){
			for(int x = 0; x < copyArr.Length; x++){
				targ.build.build[x] = copyArr[x];
			}
			targ.build.build[nl-1] = new BuildBehaviour();
		}
		else{
			for(int x = 0; x < targ.build.build.Length; x++){
				targ.build.build[x] = copyArr[x];	
			}
		}
	}
	
	// Unit Selected Objects
	void ModifyUS (int nl, int ol, UnitController targ){
		GameObject[] copyArr = new GameObject[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = targ.gui.selectObjs[x];
		}
		targ.gui.selectObjs = new GameObject[nl];
		if(nl > ol){
			for(int x = 0; x < copyArr.Length; x++){
				targ.gui.selectObjs[x] = copyArr[x];
			}
		}
		else{
			for(int x = 0; x < targ.gui.selectObjs.Length; x++){
				targ.gui.selectObjs[x] = copyArr[x];	
			}
		}
	}
	
	// Unit Technology Effects.... Effects
	void ModifyUTE (int nl, int ol, UnitController targ, int index){
		Effects[] copyArr = new Effects[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = targ.techEffect[index].effects[x];
		}
		targ.techEffect[index].effects = new Effects[nl];
		if(nl > ol){
			for(int x = 0; x < copyArr.Length; x++){
				targ.techEffect[index].effects[x] = copyArr[x];
			}
			targ.techEffect[index].effects[nl-1] = new Effects();
		}
		else{
			for(int x = 0; x < targ.techEffect[index].effects.Length; x++){
				targ.techEffect[index].effects[x] = copyArr[x];	
			}
		}
	}
	
	// Building Tech Effects
	void ModifyBT (int nl, int ol, BuildingController targ){
		TechEffect[] copyArr = new TechEffect[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = targ.techEffect[x];
		}
		targ.techEffect = new TechEffect[nl];
		if(nl > ol){
			for(int x = 0; x < copyArr.Length; x++){
				targ.techEffect[x] = copyArr[x];
			}
			targ.techEffect[nl-1] = new TechEffect();
		}
		else{
			for(int x = 0; x < targ.techEffect.Length; x++){
				targ.techEffect[x] = copyArr[x];	
			}
		}
	}
	
	// Unit Technology Effects.... Effects
	void ModifyBTE (int nl, int ol, BuildingController targ, int index){
		Effects[] copyArr = new Effects[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = targ.techEffect[index].effects[x];
		}
		targ.techEffect[index].effects = new Effects[nl];
		if(nl > ol){
			for(int x = 0; x < copyArr.Length; x++){
				targ.techEffect[index].effects[x] = copyArr[x];
			}
		}
		else{
			for(int x = 0; x < targ.techEffect[index].effects.Length; x++){
				targ.techEffect[index].effects[x] = copyArr[x];	
			}
		}
	}
	
	// Building Selected Objects
	void ModifyBS (int nl, int ol, BuildingController targ){
		GameObject[] copyArr = new GameObject[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = targ.gui.selectObjs[x];
		}
		targ.gui.selectObjs = new GameObject[nl];
		if(nl > ol){
			for(int x = 0; x < copyArr.Length; x++){
				targ.gui.selectObjs[x] = copyArr[x];
			}
		}
		else{
			for(int x = 0; x < targ.gui.selectObjs.Length; x++){
				targ.gui.selectObjs[x] = copyArr[x];	
			}
		}
	}
	
	
	void ModifyGR (int nl, int ol){
		Relation[] copyArr = new Relation[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = nTarget.relations[x];
		}
		nTarget.relations = new Relation[nl];
		if(nl > ol){
			for(int x = 0; x < copyArr.Length; x++){
				nTarget.relations[x] = copyArr[x];
			}
			nTarget.relations[nl-1] = new Relation();
		}
		else{
			for(int x = 0; x < nTarget.relations.Length; x++){
				nTarget.relations[x] = copyArr[x];	
			}
		}
	}
	
	void ModifyHE (int nl, int ol, Health health){
		HealthElement[] copyArr = new HealthElement[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = health.element[x];
		}
		health.element = new HealthElement[nl];
		if(nl > ol){
			for(int x = 0; x < copyArr.Length; x++){
				health.element[x] = copyArr[x];
			}
			health.element[nl-1] = new HealthElement();
		}
		else{
			for(int x = 0; x < health.element.Length; x++){
				health.element[x] = copyArr[x];	
			}
		}
	}
	
	void ModifyP (int nl, int ol, Progress progress){
		Texture[] copyArr = new Texture[ol];
		for(int x = 0; x < copyArr.Length; x++){
			copyArr[x] = progress.texture[x];
		}
		progress.texture = new Texture[nl];
		if(nl > ol){
			for(int x = 0; x < copyArr.Length; x++){
				progress.texture[x] = copyArr[x];
			}
			progress.texture[nl-1] = null;
		}
		else{
			for(int x = 0; x < progress.texture.Length; x++){
				progress.texture[x] = copyArr[x];	
			}
		}
	}
	
	// There are a lot of them
}