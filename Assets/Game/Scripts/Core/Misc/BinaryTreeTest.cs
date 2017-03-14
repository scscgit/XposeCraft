using UnityEngine;
using XposeCraft.Core.Required;

namespace XposeCraft.Core.Misc
{
    public class BinaryTreeTest : MonoBehaviour
    {
        public bool remove;
        public bool recalculate;
        public int indexChanged;
        public bool add;
        public int numberToAdd;
        public int indexToAdd;
        public int lowestNumber;
        public BinaryHeap heap;

        void OnDrawGizmos()
        {
            if (add)
            {
                numberToAdd = Random.Range(0, 99);
                indexToAdd = numberToAdd;
                heap.Add(numberToAdd, indexToAdd);
                add = false;
            }
            if (remove)
            {
                heap.Remove();
                remove = false;
            }
            if (recalculate)
            {
                heap.Recalculate(indexChanged);
                recalculate = false;
            }
            int lw = 100;
            for (int x = 0; x < heap.numberOfItems; x++)
            {
                if (heap.binaryHeap[x].cost < lw)
                {
                    lw = heap.binaryHeap[x].cost;
                }
            }
            lowestNumber = lw;
        }
    }
}
