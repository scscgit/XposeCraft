using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public MiniMapElement[] elements = new MiniMapElement[0];
    public MiniMapElement cameraElement;
    public Camera cam;
    public GameObject cameraController;
    public LayerMask camZoomLayer;
    public Texture background;
    public Rect localBounds { get; set; }
    public Rect realWorldBounds;
    public Texture fogTexture { get; set; }
    public Color fogColor = Color.clear;
    Rect size;
    Rect cameraLoc = new Rect(0, 0, 0, 0);
    bool moveCamera;

    public void SetSize()
    {
        size = new Rect(
            localBounds.x / realWorldBounds.x,
            localBounds.y / realWorldBounds.y,
            localBounds.width / realWorldBounds.width,
            localBounds.height / realWorldBounds.height);
        for (var x = 0; x < elements.Length; x++)
        {
            MiniMapElement element = elements[x];
            for (int y = 0; y < element.objAmount; y++)
            {
                element.ModifyLoc(y, Determine2dLoc(element.elementTransform[y].position));
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (gameObject.name != "MiniMap")
        {
            gameObject.name = "MiniMap";
        }
        if (elements.Length <= 1)
        {
            return;
        }
        for (int x = 1; x < elements.Length; x++)
        {
            if (elements[x].moveUp)
            {
                elements[x].moveUp = false;
                var backupElement = elements[x - 1];
                elements[x - 1] = elements[x];
                elements[x] = backupElement;
            }
        }
    }

    public bool AddElement(GameObject obj, string tag, MiniMapSignal map, int factionIndex)
    {
        bool found = false;
        int index = 0;
        for (int x = 0; x < elements.Length; x++)
        {
            if (elements[x].tag == tag)
            {
                index = x;
                found = true;
                break;
            }
        }
        if (found)
        {
            elements[index].AddElement(obj, tag, map, factionIndex, Determine2dLoc(obj.transform.position));
        }
        return found;
    }

    void FixedUpdate()
    {
        for (var x = 0; x < elements.Length; x++)
        {
            MiniMapElement element = elements[x];
            for (int y = 0; y < element.objAmount; y++)
            {
                if (element.elementObj[y] == null)
                {
                    element.elementObj.RemoveAt(y);
                    element.elementMap.RemoveAt(y);
                    element.elementTransform.RemoveAt(y);
                    element.elementLoc.RemoveAt(y);
                    element.elementFaction.RemoveAt(y);
                    element.objAmount--;
                    y--;
                }
            }
        }

        for (var x = 0; x < elements.Length; x++)
        {
            MiniMapElement element = elements[x];
            for (int y = 0; y < element.objAmount; y++)
            {
                if (!element.elementMap[y].isStatic)
                {
                    element.elementLoc[y] = Determine2dLoc(element.elementTransform[y].position);
                }
            }
        }

        //Vector2 loc = Determine2dLoc(cam.transform.position);

        RaycastHit hitTopLeft;
        Ray ray = cam.ScreenPointToRay(new Vector2(0, Screen.height));
        Physics.Raycast(ray, out hitTopLeft, 1000, camZoomLayer);
        RaycastHit hitTopRight;
        ray = cam.ScreenPointToRay(new Vector2(Screen.width, Screen.height));
        Physics.Raycast(ray, out hitTopRight, 1000, camZoomLayer);
        RaycastHit hitBottomRight;
        ray = cam.ScreenPointToRay(new Vector2(Screen.width, 0));
        Physics.Raycast(ray, out hitBottomRight, 1000, camZoomLayer);
        if (hitTopLeft.collider != null && hitTopRight.collider != null && hitBottomRight.collider != null)
        {
            var topLeft = Determine2dLoc(hitTopLeft.point);
            var topRight = Determine2dLoc(hitTopRight.point);
            var bottomRight = Determine2dLoc(hitBottomRight.point);
            cameraLoc = new Rect(topLeft.x, topLeft.y, topRight.x - topLeft.x, bottomRight.y - topLeft.y);
        }
        else
        {
            Debug.LogWarning("Minimap could not detect its camera location, check your Cam Zoom Layer");
        }
        if (!moveCamera)
        {
            return;
        }
        cameraController.transform.position = Determine3dLoc(
            cameraController.transform.position.y,
            new Vector2(
                Input.mousePosition.x,
                Screen.height - Input.mousePosition.y + cameraLoc.height / 2));
    }

    void OnGUI()
    {
        // Saves a lot of processing
#if !UNITY_EDITOR
        useGUILayout = false;
#endif
        // The useGUILayout = false does not honor GUI.depth, this usually breaks when Editor hotswaps the script
        GUI.depth = 0;

        // For Moving around the Camera
        moveCamera = GUI.RepeatButton(localBounds, "") && Input.GetButton("LMB");

        // Draw the Map Background
        if (background != null)
        {
            GUI.DrawTexture(localBounds, background);
        }

        // Draw the Elements on the Map
        for (int x = 0; x < elements.Length; x++)
        {
            MiniMapElement element = elements[x];
            Vector2 halfSize = new Vector2(element.size.x / 2, element.size.y / 2);
            for (int y = 0; y < element.objAmount; y++)
            {
                Vector2 loc = element.elementLoc[y];
                if (element.elementMap[y].display)
                {
                    GUI.color = element.tints[element.elementFaction[y]];
                    GUI.DrawTexture(
                        new Rect(loc.x - halfSize.x, loc.y - halfSize.y, element.size.x, element.size.y),
                        element.image);
                }
            }
        }

        // Drawing the fog
        GUI.color = fogColor;
        GUI.DrawTexture(localBounds, fogTexture);
        if (cameraElement.image)
        {
            // Draw the Camera
            GUI.color = cameraElement.tints[0];
            GUI.DrawTexture(cameraLoc, cameraElement.image, ScaleMode.StretchToFill);
        }
    }

    // Determines the Location on the MiniMap for the loc
    Vector2 Determine2dLoc(Vector3 loc)
    {
        return new Vector2(
            (loc.x - realWorldBounds.x) * size.width + localBounds.x,
            localBounds.y + localBounds.height - (loc.z - realWorldBounds.y) * size.height);
    }

    Vector3 Determine3dLoc(float cameraHeight, Vector2 loc)
    {
        Rect size = new Rect(
            realWorldBounds.x / localBounds.x,
            realWorldBounds.y / localBounds.y,
            realWorldBounds.width / localBounds.width,
            realWorldBounds.height / localBounds.height);
        return new Vector3(
            realWorldBounds.x + (loc.x - localBounds.x) * size.width,
            cameraHeight,
            realWorldBounds.y + realWorldBounds.height - (loc.y - localBounds.y) * size.height);
    }
}
