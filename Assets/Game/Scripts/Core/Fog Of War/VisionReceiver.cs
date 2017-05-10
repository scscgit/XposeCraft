using UnityEngine;
using XposeCraft.GameInternal;
using XposeCraft.UI.Cursor;

namespace XposeCraft.Core.Fog_Of_War
{
    public class VisionReceiver : MonoBehaviour
    {
        public enum VisionState
        {
            Vision = 0,
            Discovered = 1,
            Undiscovered = 2
        }

        // This code determines how a unit is displayed when in the In Vision, Discovered, and Undiscovered areas
        // The pastVisibleMat is displayed when the object has been seen at one time but not at this moment
        // The defaultMat is used for the unit if they are within vision at that time

        public Renderer[] renderers;
        public Color[] defaultMat;
        public Color[] pastVisibleMat = {Color.gray};
        public bool hideObjectWhenNotSeen;
        public bool hideCursorIcon = true;
        public bool hideObject = true;
        public Vector2[] anchors;
        CursorObject cursorObj;
        public VisionState curState;
        private bool _quitting;

        private void Awake()
        {
            int matAmount = 0;
            for (int x = 0; x < renderers.Length; x++)
            {
                matAmount += renderers[x].materials.Length;
            }
            if (defaultMat.Length < matAmount)
            {
                defaultMat = new Color[matAmount];
                InitializeMaterialColors(defaultMat);
            }
            if (pastVisibleMat.Length < matAmount)
            {
                pastVisibleMat = new Color[matAmount];
                InitializeMaterialColors(pastVisibleMat);
            }
            if (hideCursorIcon)
            {
                cursorObj = gameObject.GetComponent<CursorObject>();
            }
        }

        private void InitializeMaterialColors(Color[] materialColors)
        {
            int z = 0;
            for (int x = 0; x < renderers.Length; x++)
            {
                for (int y = 0; y < renderers[x].materials.Length; y++)
                {
                    materialColors[z] = renderers[x].materials[y].color;
                    z++;
                }
            }
        }

        public void OnEnable()
        {
            GameManager.Instance.Fog.AddRenderer(gameObject, this);
        }

        private void OnApplicationQuit()
        {
            _quitting = true;
        }

        public void OnDisable()
        {
            if (_quitting)
            {
                return;
            }
            GameManager.Instance.Fog.RemoveRenderer(gameObject);
        }

        public void SetRenderer(VisionState state)
        {
            curState = state;
            switch (state)
            {
                // In Vision
                case VisionState.Vision:
                    for (int x = 0; x < renderers.Length; x++)
                    {
                        if (x == 0 && renderers[x].material.color == defaultMat[x] && renderers[x].enabled)
                        {
                            break;
                        }
                        renderers[x].material.color = defaultMat[x];
                        renderers[x].enabled = true;
                        if (hideCursorIcon && cursorObj != null)
                        {
                            cursorObj.enabled = true;
                        }
                    }
                    break;
                // Discovered
                case VisionState.Discovered:
                    for (int x = 0; x < renderers.Length; x++)
                    {
                        if (x == 0 && renderers[x].material.color == pastVisibleMat[x] && renderers[x].enabled)
                        {
                            break;
                        }
                        if (pastVisibleMat.Length > x)
                        {
                            renderers[x].material.color = pastVisibleMat[x];
                        }
                        renderers[x].enabled = !hideObjectWhenNotSeen;
                        if (hideCursorIcon && cursorObj != null)
                        {
                            cursorObj.enabled = true;
                        }
                    }
                    break;
                // Undiscovered
                case VisionState.Undiscovered:
                    for (int x = 0; x < renderers.Length; x++)
                    {
                        if (x == 0 && !renderers[x].enabled)
                        {
                            break;
                        }
                        if (hideObject)
                        {
                            renderers[x].enabled = false;
                        }
                        if (hideCursorIcon && cursorObj != null)
                        {
                            cursorObj.enabled = false;
                        }
                    }
                    break;
            }
        }
    }
}
