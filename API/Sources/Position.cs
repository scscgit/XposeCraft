using System;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game
{
    /// <summary>
    /// Map position defined by X and Y coordinates.
    /// </summary>
    [Serializable]
    public class Position
    {
        /// <summary>
        /// Representing X coordinate.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Representing Y coordinate.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Internal grid point location on the map.
        /// </summary>
        public int PointLocation { get; }

        /// <summary>
        /// Create the Position representing an internal grid point location on the map.
        /// </summary>
        /// <param name="pointLocation">Internal grid point location on the map.</param>
        /// <exception cref="ArgumentException">Thrown when the location is out of bounds within the map.</exception>
        public Position(int pointLocation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the Position from X and Y coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <exception cref="ArgumentException">Thrown when a coordinate is out of bounds within the map.</exception>
        public Position(int x, int y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a Path representation between two Positions.
        /// </summary>
        /// <param name="position">Position of the path origin.</param>
        /// <returns>Path between two Positions.</returns>
        public Path PathFrom(Position position)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Describes whether the Position is available for movement or placement purposes.
        /// </summary>
        /// <returns>True if the Position is available.</returns>
        public bool IsAvailable()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Finds a nearest available Position.
        /// </summary>
        /// <returns>Available Position.</returns>
        public Position FindNearestAvailable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a Building of the chosen type can be placed on the Position.
        /// The condition is also that the Fog is currently uncovered there.
        /// </summary>
        /// <param name="buildingType">Type of the building meant to be built.</param>
        /// <returns>True if the building can be placed on the Position.</returns>
        public bool IsValidPlacement(BuildingType buildingType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the Fog of War is uncovered on this position, making everything there visible.
        /// </summary>
        /// <returns>True if the Fog if uncovered.</returns>
        public bool IsFogUncovered()
        {
            throw new NotImplementedException();
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
