using System;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Enums
{
    [Serializable]
    public class PlaceType
    {
        public Position NearBase_;
        public Position UnderBaseRamp_;
        public Position EnemyBaseCenter_;
        public Position EnemyBasePositionLeft_;
        public Position EnemyBasePositionFront_;
        public Position EnemyBasePositionRight_;
        public Position EnemyBasePositionBack_;

        public static Position NearBase
        {
            get { return Player.CurrentPlayer.PlaceType.NearBase_; }
        }

        public static Position UnderBaseRamp
        {
            get { return Player.CurrentPlayer.PlaceType.UnderBaseRamp_; }
        }

        public static Position EnemyBaseCenter
        {
            get { return Player.CurrentPlayer.PlaceType.EnemyBaseCenter_; }
        }

        public static Position EnemyBasePositionLeft
        {
            get { return Player.CurrentPlayer.PlaceType.EnemyBasePositionLeft_; }
        }

        public static Position EnemyBasePositionFront
        {
            get { return Player.CurrentPlayer.PlaceType.EnemyBasePositionFront_; }
        }

        public static Position EnemyBasePositionRight
        {
            get { return Player.CurrentPlayer.PlaceType.EnemyBasePositionRight_; }
        }

        public static Position EnemyBasePositionBack
        {
            get { return Player.CurrentPlayer.PlaceType.EnemyBasePositionBack_; }
        }

        public PlaceType(Position[] positions)
        {
            NearBase_ = positions[0];
            UnderBaseRamp_ = positions[1];
            EnemyBaseCenter_ = positions[2];
            EnemyBasePositionLeft_ = positions[3];
            EnemyBasePositionFront_ = positions[4];
            EnemyBasePositionRight_ = positions[5];
            EnemyBasePositionBack_ = positions[6];
        }
    }
}
