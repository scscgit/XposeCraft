using UnityEngine;

namespace XposeCraft.Core.Grids
{
    public class DelayScript : MonoBehaviour
    {
        public int amountCur;
        public float delayAmount = 0.01f;
        public int increment = 5;
        public int total;
        public int clearDelayAmount;

        public float GetDelay()
        {
            if (amountCur >= increment - 1)
            {
                total++;
                amountCur = 0;
            }
            else
            {
                amountCur++;
            }
            return delayAmount * total;
        }

        public void Update()
        {
            if (clearDelayAmount != total * increment + amountCur)
            {
                return;
            }
            amountCur = 0;
            total = 0;
            clearDelayAmount = 0;
        }

        public void ClearDelay()
        {
            clearDelayAmount++;
        }
    }
}
