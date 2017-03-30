namespace XposeCraft.Game.Actors.Buildings
{
    /// <summary>
    /// A base building that creates new workers and receives collected materials.
    /// </summary>
    public class BaseCenter : Building
    {
        public bool CreateWorker()
        {
            return false;
        }
    }
}
