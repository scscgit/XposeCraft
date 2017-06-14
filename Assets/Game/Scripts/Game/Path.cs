using System;
using UnityEngine;
using XposeCraft.Core.Required;
using XposeCraft.GameInternal;
using XposeCraft.GameInternal.Helpers;

namespace XposeCraft.Game
{
    /// <summary>
    /// Path between two Positions that uses pathfinding mainly for length comparison purposes.
    /// </summary>
    [Serializable]
    public class Path
    {
        private const int InvalidPathLength = 0;

        internal int _gridIndex = GameManager.Instance.UGrid.index;

        [SerializeField] private Position _from;
        [SerializeField] private Position _to;
        private int? _length;
        private int[] _pointLocations;

        /// <summary>
        /// Closest available Position to the one used as a Path source.
        /// </summary>
        public Position From
        {
            get { return _from.FindNearestAvailable(); }
        }

        /// <summary>
        /// Closest available Position to the one used as a Path destination.
        /// </summary>
        public Position To
        {
            get { return _to.FindNearestAvailable(); }
        }

        /// <summary>
        /// Length representing the number of available Positions between the two Positions (inclusive),
        /// or some available that are closest to them.
        /// Value 0 means the Path is invalid, which is most likely to happen when both Positions are unavailable.
        /// </summary>
        public int Length
        {
            get
            {
                if (!_length.HasValue)
                {
                    CalculateLength();
                }
                return _length.Value;
            }
        }

        /// <summary>
        /// Continuous locations of the Path.
        /// This API is experimental and may be turned to Positions or removed in a later release.
        /// </summary>
        public int[] PointLocations
        {
            get
            {
                if (_pointLocations == null)
                {
                    CalculateLength();
                }
                return _pointLocations;
            }
        }

        /// <summary>
        /// Create a Path between two Positions.
        /// </summary>
        /// <param name="from">Source Position.</param>
        /// <param name="to">Destination Position.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the Positions is null.</exception>
        public Path(Position from, Position to)
        {
            if (from == null || to == null)
            {
                throw new ArgumentNullException();
            }
            _from = from;
            _to = to;
        }

        /// <summary>
        /// Calculates Path up to a certain Length, cancelling the calculation if it gets exceeded.
        /// </summary>
        /// <param name="length">Maximum Path Length.</param>
        /// <returns>Length if it is less than <paramref name="length"/>, null if it is larger.</returns>
        internal int? IsLengthLessThan(int length)
        {
            var uPath = CreateAPath()
                .FindPathShorterThan(
                    PositionHelper.PositionToLocation(To),
                    PositionHelper.PositionToLocation(From),
                    length
                );
            if (uPath == null)
            {
                return null;
            }
            StoreLength(uPath);
            return Length;
        }

        private void CalculateLength()
        {
            //var uPath = aPath.FindNormalPath(From.PointLocation, To.PointLocation);
            // Finding the Path between the first available Positions between them.
            StoreLength(CreateAPath()
                .FindPath(
                    PositionHelper.PositionToLocation(To),
                    PositionHelper.PositionToLocation(From)));
        }

        private APath CreateAPath()
        {
            var aPath = new APath {gridScript = GameManager.Instance.UGrid, gridI = _gridIndex};
            aPath.InitializeGrid();
            return aPath;
        }

        /// <summary>
        /// Stores the calculated values to the Path instance.
        /// </summary>
        /// <param name="uPath"></param>
        private void StoreLength(UPath uPath)
        {
            if (uPath == null)
            {
                _length = InvalidPathLength;
                _pointLocations = new int[0];
                Log.w(this,
                    "Created from " + From.PointLocation + "=" + From.X + ":" + From.Y
                    + " to " + To.PointLocation + "=" + To.X + ":" + To.Y
                    + " with invalid Length");
            }
            else
            {
                _length = uPath.list.Length;
                _pointLocations = uPath.list;
                Log.d(this,
                    "Created from " + From.PointLocation + "=" + From.X + ":" + From.Y
                    + " to " + To.PointLocation + "=" + To.X + ":" + To.Y
                    + " with Length " + Length);
            }
        }

        public static bool operator <(Path left, Path right)
        {
            return left.Length < right.Length;
        }

        public static bool operator >(Path left, Path right)
        {
            return left.Length > right.Length;
        }

        public static bool operator <=(Path left, Path right)
        {
            return !(left > right);
        }

        public static bool operator >=(Path left, Path right)
        {
            return !(left < right);
        }
    }
}
