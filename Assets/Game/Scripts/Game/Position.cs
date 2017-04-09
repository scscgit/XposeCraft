using System;
using UnityEngine;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.GameInternal;
using XposeCraft.GameInternal.Helpers;
using BuildingType = XposeCraft.Game.Enums.BuildingType;

namespace XposeCraft.Game
{
    [Serializable]
    public class Position
    {
        private int? _x;
        private int? _y;
        [SerializeField] private int _pointLocation;

        public int X
        {
            get
            {
                if (!_x.HasValue)
                {
                    CalculateCoordinates(_pointLocation);
                }
                return _x.Value;
            }
        }

        public int Y
        {
            get
            {
                if (!_y.HasValue)
                {
                    CalculateCoordinates(_pointLocation);
                }
                return _y.Value;
            }
        }

        public int PointLocation
        {
            get { return _pointLocation; }
        }

        public Vector3 Location
        {
            get { return GameManager.Instance.Grid.points[_pointLocation].loc; }
        }

        public Position(int pointLocation)
        {
            CalculateCoordinates(pointLocation);
            _pointLocation = pointLocation;
        }

        public Position(int x, int y)
        {
            var gridSize = GameManager.Instance.Grid.size;
            _x = x;
            _y = y;
            _pointLocation = x + y * gridSize;
        }

        private void CalculateCoordinates(int pointLocation)
        {
            var gridSize = GameManager.Instance.Grid.size;
            _y = pointLocation / gridSize;
            _x = pointLocation - _y * gridSize;
        }

        public Path PathFrom(Position position)
        {
            return new Path(position, this);
        }

        public bool IsValidPlacement(BuildingType buildingType)
        {
            return BuildingPlacement.IsValidPlacement(
                BuildingHelper.FindBuildingInFaction(buildingType, null), this, Location, true);
        }
    }
}
