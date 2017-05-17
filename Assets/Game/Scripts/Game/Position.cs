using System;
using UnityEngine;
using XposeCraft.Game.Enums;
using XposeCraft.GameInternal;
using XposeCraft.GameInternal.Helpers;

namespace XposeCraft.Game
{
    /// <summary>
    /// Map position defined by X and Y coordinates.
    /// </summary>
    [Serializable]
    public class Position
    {
        private int? _x;
        private int? _y;
        [SerializeField] private int _pointLocation;

        /// <summary>
        /// Representing X coordinate.
        /// </summary>
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

        /// <summary>
        /// Representing Y coordinate.
        /// </summary>
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

        /// <summary>
        /// Internal grid point location on the map.
        /// </summary>
        public int PointLocation
        {
            get { return _pointLocation; }
        }

        /// <summary>
        /// Internal Vector3 representation.
        /// </summary>
        protected Vector3 Location
        {
            get { return PositionHelper.PositionToLocation(this); }
        }

        /// <summary>
        /// Create the Position representing an internal grid point location on the map.
        /// </summary>
        /// <param name="pointLocation">Internal grid point location on the map.</param>
        /// <exception cref="ArgumentException">Thrown when the location is out of bounds within the map.</exception>
        public Position(int pointLocation)
        {
            CalculateCoordinates(pointLocation);
            _pointLocation = pointLocation;
            if (!IsValid())
            {
                throw new ArgumentException("The Position is invalid and does not represent any map coordinates.");
            }
        }

        /// <summary>
        /// Create the Position from X and Y coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <exception cref="ArgumentException">Thrown when a coordinate is out of bounds within the map.</exception>
        public Position(int x, int y)
        {
            _x = x;
            _y = y;
            _pointLocation = x + y * GameManager.Instance.Grid.size;
            if (!IsValid())
            {
                throw new ArgumentException("The Position is invalid and does not represent any map coordinates.");
            }
        }

        private void CalculateCoordinates(int pointLocation)
        {
            var gridSize = GameManager.Instance.Grid.size;
            _y = pointLocation / gridSize;
            _x = pointLocation - _y * gridSize;
        }

        /// <summary>
        /// Creates a Path representation between two Positions.
        /// </summary>
        /// <param name="position">Position of the path origin.</param>
        /// <returns>Path between two Positions.</returns>
        public Path PathFrom(Position position)
        {
            return new Path(position, this);
        }

        /// <summary>
        /// Describes whether the Position is available for movement or placement purposes.
        /// </summary>
        /// <returns>True if the Position is available.</returns>
        public bool IsAvailable()
        {
            return IsValid() && GameManager.Instance.Grid.points[PointLocation].state != 2;
        }

        /// <summary>
        /// Describes whether the Position is really located on the map and can ever possibly be available.
        /// </summary>
        /// <returns>True if the Position has valid coordinates that exist n the map.</returns>
        private bool IsValid()
        {
            return GameManager.Instance.UGrid.IsValidLocation(PointLocation);
        }

        /// <summary>
        /// Finds a nearest available Position.
        /// </summary>
        /// <returns>Available Position.</returns>
        public Position FindNearestAvailable()
        {
            return IsAvailable() ? this : new Position(GameManager.Instance.UGrid.DetermineNearestLocation(this));
        }

        /// <summary>
        /// Checks whether a Building of the chosen type can be placed on the Position.
        /// The condition is also that the Fog is currently uncovered there.
        /// </summary>
        /// <param name="buildingType">Type of the building meant to be built.</param>
        /// <returns>True if the building can be placed on the Position.</returns>
        public bool IsValidPlacement(BuildingType buildingType)
        {
            return IsFogUncovered()
                   && BuildingHelper.IsValidPlacement(
                       BuildingHelper.FindBuildingInFaction(buildingType, null), this, Location, false);
        }

        /// <summary>
        /// Checks whether the Fog of War is uncovered on this position, making everything there visible.
        /// </summary>
        /// <returns>True if the Fog if uncovered.</returns>
        public bool IsFogUncovered()
        {
            return GameManager.Instance.Fog.CheckLocation(Location);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Position);
        }

        public bool Equals(Position position)
        {
            return position != null && PointLocation == position.PointLocation;
        }

        public override int GetHashCode()
        {
            return PointLocation;
        }

        public static bool operator ==(Position a, Position b)
        {
            return ReferenceEquals(a, b)
                   || (object) a != null && (object) b != null && a.PointLocation == b.PointLocation;
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a == b);
        }
    }
}
