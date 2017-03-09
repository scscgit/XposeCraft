using UnityEngine;
using UnityEngine.Serialization;

public class Faction : MonoBehaviour
{
    [FormerlySerializedAs("relations")] public Relation[] Relations = new Relation[0];
    [FormerlySerializedAs("gui")] public FGUI Gui;
    [FormerlySerializedAs("unitList")] public Unit[] UnitList = new Unit[0];
    [FormerlySerializedAs("buildingList")] public Building[] BuildingList = new Building[0];
    [FormerlySerializedAs("tech")] public Technology[] Tech = new Technology[0];
    [FormerlySerializedAs("color")] public Color Color;
}
