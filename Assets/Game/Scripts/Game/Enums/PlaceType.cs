using System;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Enums
{
    /// <summary>
    /// Useful places near a Base.
    /// </summary>
    [Serializable]
    public class PlaceType
    {
        public Position Center;
        public Position Left;
        public Position Right;
        public Position Front;
        public Position Back;
        public Position UnderRampLeft;
        public Position UnderRampRight;

        public static PlaceType MyBase
        {
            get { return Player.CurrentPlayer.MyBase; }
        }

        public static PlaceType EnemyBase
        {
            get { return Player.CurrentPlayer.EnemyBase; }
        }

        public PlaceType(Position[] positions)
        {
            Center = positions[0];
            Left = positions[1];
            Right = positions[2];
            Front = positions[3];
            Back = positions[4];
            UnderRampLeft = positions[5];
            UnderRampRight = positions[6];
        }
    }
}
