using System;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Enums
{
    [Serializable]
    public class PlaceType
    {
        public Position Center;
        public Position Left;
        public Position Right;
        public Position Front;
        public Position Back;
        public Position UnderRamp;

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
            UnderRamp = positions[5];
        }
    }
}
