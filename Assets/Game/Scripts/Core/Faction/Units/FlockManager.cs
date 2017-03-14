using System;
using UnityEngine;

namespace XposeCraft.Core.Faction.Units
{
    public class FlockManager : MonoBehaviour
    {
        public Formation[] formations;
        public Texture gridLineTexture;
    }

    [Serializable]
    public class Formation
    {
        public string name = "Formation";
        public FLine[] fLines;
        public FGrid grid = new FGrid();

        //FPoint[] fPoints;
    }

    [Serializable]
    public class FLine
    {
        public float percent = 25;
        public Vector2 pointIndex = new Vector2(0, 1);
    }

/*
[System.Serializable]
class FPoint {
	float index = 0;
	Vector2 loc = Vector2.zero;
}
*/

    [Serializable]
    public class FGrid
    {
        public Vector2 lw;
        public float pointDisp;
    }
}
