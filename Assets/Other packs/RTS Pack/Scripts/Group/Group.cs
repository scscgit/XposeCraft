using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Group : MonoBehaviour {
	
	public Relation[] relations = new Relation[0];
	public FGUI gui;
	public Unit[] unitList = new Unit[0];
	public Building[] buildingList = new Building[0];
	public Technology[] tech = new Technology[0];
	public Color color;
	Type[] types = new Type[0];
	List<List<UnitController>> units;
	List<int> unitsAmount;
	List<List<BuildingController>> buildings;
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
}