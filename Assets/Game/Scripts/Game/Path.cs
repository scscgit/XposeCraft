using System;

namespace XposeCraft.Game
{
    [Serializable]
    public class Path
    {
        public Position From { get; private set; }
        public Position To { get; private set; }

        public Path(Position from, Position to)
        {
            From = from;
            To = to;
        }

        public static bool operator <(Path left, Path right)
        {
            return true;
        }

        public static bool operator >(Path left, Path right)
        {
            return false;
        }
    }
}
