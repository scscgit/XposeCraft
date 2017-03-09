namespace XposeCraft.Game.Enums
{
    public class PlaceType
    {
        static PlaceType()
        {
            NearBase = new Position();
            UnderBaseRamp = new Position();
            EnemyBaseCenter = new Position();
            EnemyBasePositionLeft = new Position();
            EnemyBasePositionFront = new Position();
            EnemyBasePositionRight = new Position();
            EnemyBasePositionBack = new Position();
        }

        public static Position NearBase { get; private set; }
        public static Position UnderBaseRamp { get; private set; }
        public static Position EnemyBaseCenter { get; private set; }
        public static Position EnemyBasePositionLeft { get; private set; }
        public static Position EnemyBasePositionFront { get; private set; }
        public static Position EnemyBasePositionRight { get; private set; }
        public static Position EnemyBasePositionBack { get; private set; }
    }
}
