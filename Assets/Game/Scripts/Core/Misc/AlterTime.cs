using UnityEngine;

namespace XposeCraft.Core.Misc
{
    public class AlterTime : MonoBehaviour
    {
        public string key = "q";

        public int NormalSpeed = 2;
        public int Modifier = 8;

        private void Start()
        {
            Time.timeScale = NormalSpeed;
        }

        private void Update()
        {
            Time.timeScale = Input.GetKey(key) ? Modifier : NormalSpeed;
        }
    }
}
