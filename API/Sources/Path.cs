using System;

namespace XposeCraft.Game
{
    /// <summary>
    /// Path between two Positions mainly for length comparison purposes.
    /// </summary>
    [Serializable]
    public class Path
    {
        /// <summary>
        /// Closest available Position to the one used as a Path source.
        /// </summary>
        public Position From
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Closest available Position to the one used as a Path destination.
        /// </summary>
        public Position To
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Length representing the number of available Positions between the two Positions (inclusive),
        /// or some available that are closest to them.
        /// Value 0 means the Path is invalid, which is most likely to happen when both Positions are unavailable.
        /// </summary>
        public int Length
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Continuous locations of the Path.
        /// This API is experimental and may be turned to Positions or removed in a later release.
        /// </summary>
        public int[] PointLocations
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Create a Path between two Positions.
        /// </summary>
        /// <param name="from">Source Position.</param>
        /// <param name="to">Destination Position.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the Positions is null.</exception>
        public Path(Position from, Position to)
        {
            throw new NotImplementedException();
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
