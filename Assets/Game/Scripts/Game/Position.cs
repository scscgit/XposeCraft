using System;
using UnityEngine;
using XposeCraft.Core.Grids;
using XposeCraft.GameInternal;

namespace XposeCraft.Game
{
    [Serializable]
    public class Position
    {
        private static UGrid _uGrid;

        [SerializeField] private int _pointLocation;

        public int PointLocation
        {
            get { return _pointLocation; }
        }

        public Vector3 Location
        {
            get
            {
                if (_uGrid == null)
                {
                    _uGrid = GameObject.Find(GameManager.ScriptName).GetComponent<GameManager>().UGrid;
                }
                return _uGrid.grids[_uGrid.index].points[_pointLocation].loc;
            }
        }

        public Position(int pointLocation)
        {
            _pointLocation = pointLocation;
        }

        public static bool operator <(Position left, Position right)
        {
            return true;
        }

        public static bool operator >(Position left, Position right)
        {
            return false;
        }
    }
}
