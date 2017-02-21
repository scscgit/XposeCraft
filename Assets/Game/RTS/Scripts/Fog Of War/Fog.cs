using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class Fog : MonoBehaviour
{
    // This is the basis script of the fog of war system
    // Without this, the VisionSignal, VisionBlocker, and VisionReceiver scripts won't do anything

    // This should be set to the fill up the overall size of your map
    // Width and Length will be the size of the fog texture you want
    // While the displacement is used to fill in the rest

    // Ex. if you have a map that is 500x500, and want your fog texture to be 100x100, the disp will be 5

    public int width;
    public int length;
    public int disp;

    // You will not directly access these, however these lists keep track of your units on the screen

    List<GameObject> agents = new List<GameObject>(0);
    List<int> agentsRadius = new List<int>(0);
    int agentsAmount;
    List<float> agentsUSlope = new List<float>(0);
    List<float> agentsDSlope = new List<float>(0);
    List<Transform> agentsT = new List<Transform>(0);

    // Much like the above lists, this keeps track of the objects you want hidden

    List<GameObject> hiddenAgent = new List<GameObject>(0);
    List<VisionReceiver> hiddenRend = new List<VisionReceiver>(0);
    List<Transform> hiddenAgentT = new List<Transform>(0);
    int[] hideSetting;
    int hideAmount;

    // The fog variable is the texture you are actually modifying for the fog system
    Texture2D fog;

    //int[] fogState = new int[0];

    public Vector3 startPos = Vector3.zero;
    Color32[] fogColor;
    float[] fogHeight;
    Vector3[] locA;
    Vector3[] locH;
    [HideInInspector] public MiniMap map;
    public Color revealed;
    public Color visited;
    public Color hidden;
    public Material terrainMat;
    public Texture transparentTexture;
    public LayerMask terrainLayer;
    bool[][] visitedPoints;
    public int blendRate = 5;
    bool funActive;

    void OnDrawGizmos()
    {
        // This is required so that the terrain's mat will be reset after the game ends. This is only for Unity testing issues
        if (!Application.isPlaying)
        {
            if (terrainMat != null)
            {
                terrainMat.SetTexture("_FOWTex", transparentTexture);
            }
        }

        if (gameObject.name != "Fog")
        {
            gameObject.name = "Fog";
        }
    }

    void Awake()
    {
        fog = new Texture2D(width, length, TextureFormat.ARGB32, false);
        fogColor = new Color32[width * length];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                fog.SetPixel(x, y, hidden);
                fogColor[x + y * width] = hidden;
            }
        }
        fog.Apply();
        if (map == null)
        {
            GameObject obj = GameObject.Find("MiniMap");
            if (obj)
            {
                map = obj.GetComponent<MiniMap>();
            }
        }
        if (map)
        {
            map.fogTexture = fog;
        }
        terrainMat.SetTexture("_FOWTex", fog);
        //fogState = new int[width*length];
        fogHeight = new float[width * length];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                RaycastHit hit;
                Physics.Raycast(new Vector3(x * disp, 1000, y * disp), Vector3.down, out hit, 10000, terrainLayer);
                fogHeight[x + y * width] = hit.point.y;
            }
        }
    }

    void FixedUpdate()
    {
        // If the function is already running then we don't need to run another function
        if (!funActive)
        {
            UpdateFog();
        }
    }

    // The main fog function, it is executed whenever the thread is clear
    void UpdateFog()
    {
        locA = new Vector3[agentsAmount];
        for (int x = 0; x < agentsAmount; x++)
        {
            if (agents[x] == null)
            {
                agents.RemoveAt(x);
                agentsT.RemoveAt(x);
                agentsRadius.RemoveAt(x);
                agentsUSlope.RemoveAt(x);
                agentsDSlope.RemoveAt(x);
                agentsAmount--;
                x--;
            }
            locA[x] = agentsT[x].position;
        }
        locH = new Vector3[hideAmount];
        for (int x = 0; x < hideAmount; x++)
        {
            if (hiddenAgent[x] == null)
            {
                hiddenAgent.RemoveAt(x);
                hiddenAgentT.RemoveAt(x);
                hiddenRend.RemoveAt(x);
                hideAmount--;
                x--;
            }
            locH[x] = hiddenAgentT[x].position;
        }
        // MultiThreaded request. This keeps the many processes off the main thread and runs them in the background
        ThreadPool.QueueUserWorkItem(ModifyFog);
        // Finally, we set and apply the new colors to the fog texture
        fog.SetPixels32(fogColor);
        fog.Apply(false);

        // And we set the hidden objects
        for (int x = 0; x < hideAmount; x++)
        {
            hiddenRend[x].SetRenderer(hideSetting[x]);
        }
    }


    void ModifyFog(object obj)
    {
        funActive = true;
        int[] fogVision = new int[fogColor.Length];
        Color32[] fColor = new Color32[fogColor.Length];
        for (int x = 0; x < fogColor.Length; x++)
        {
            if (fogColor[x] == revealed)
            {
                fogVision[x] = 0;
                Color tColor = fogColor[x];
                tColor[3] += (0.003921568627451f) * blendRate;
                fColor[x] = tColor;
                Color visitedColor = visited;
                if (tColor[3] > visitedColor[3])
                {
                    fColor[x] = visited;
                }
            }
            else if (fogColor[x] == hidden)
            {
                fogVision[x] = 2;
                fColor[x] = hidden;
            }
            else
            {
                fogVision[x] = 1;
                Color tColor = fogColor[x];
                tColor[3] += (0.003921568627451f) * blendRate;
                fColor[x] = tColor;
                Color visitedColor = visited;
                if (tColor[3] > visitedColor[3])
                {
                    fColor[x] = visited;
                }
            }
        }
        for (int x = 0; x < agentsAmount; x++)
        {
            if (x >= locA.Length)
            {
                continue;
            }
            Vector2 loc = DetermineLoc(locA[x]);
            int rad = agentsRadius[x];
            int locX = (int) loc.x;
            int locY = (int) loc.y;
            float uslope = agentsUSlope[x];
            float dslope = agentsDSlope[x];
            for (int z = -agentsRadius[x]; z <= agentsRadius[x]; z++)
            {
                Line(locX, locY, locX + z, locY - rad, rad, ref fogVision, ref fColor, locA[x].y, uslope, dslope);
                Line(locX, locY, locX + z, locY + rad, rad, ref fogVision, ref fColor, locA[x].y, uslope, dslope);
                Line(locX, locY, locX - rad, locY + z, rad, ref fogVision, ref fColor, locA[x].y, uslope, dslope);
                Line(locX, locY, locX + rad, locY + z, rad, ref fogVision, ref fColor, locA[x].y, uslope, dslope);
            }
        }
        fogColor = fColor;
        try
        {
            CheckRenderer(fogVision);
        }
        catch (Exception e)
        {
            // Unity does not catch exceptions that occur in threads other than the main thread
            Debug.LogError(e);
            throw;
        }
        finally
        {
            funActive = false;
        }
    }

    // Bresenham's Line Algorithm at work for shadow casting
    void Line(int x, int y, int x2, int y2, int radius, ref int[] fogVision, ref Color32[] fColor, float locY,
        float upwardSlopeHeight, float downwardSlopeHeight)
    {
        int orPosX = x;
        int orPosY = y;
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1;
        else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1;
        else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1;
        else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1;
            else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            float dist = new Vector2(orPosX - x, orPosY - y).sqrMagnitude;
            if (x < width && x >= 0)
            {
                if (x + y * width > 0 && x + y * width < fogHeight.Length)
                {
                    if (fogHeight[x + y * width] == -1)
                    {
                        fogVision[x + y * width] = 0;
                        fColor[x + y * width] = LerpColor(fogColor[x + y * width]);
                        break;
                    }
                    if (dist < radius * radius - 2)
                    {
                        if (locY > fogHeight[x + y * width])
                        {
                            if (locY - fogHeight[x + y * width] <= downwardSlopeHeight)
                            {
                                fogVision[x + y * width] = 0;
                                fColor[x + y * width] = LerpColor(fogColor[x + y * width]);
                            }
                            else
                            {
                                fogVision[x + y * width] = 0;
                                fColor[x + y * width] = LerpColor(fogColor[x + y * width]);
                                break;
                            }
                        }
                        else
                        {
                            if (fogHeight[x + y * width] - locY <= upwardSlopeHeight)
                            {
                                fogVision[x + y * width] = 0;
                                fColor[x + y * width] = LerpColor(fogColor[x + y * width]);
                            }
                            else
                            {
                                fogVision[x + y * width] = 0;
                                fColor[x + y * width] = LerpColor(fogColor[x + y * width]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
    }

    // A function to lerp the alpha values between two colors for the fade effect
    Color32 LerpColor(Color32 curColor)
    {
        Color tColor = curColor;
        tColor[3] -= (0.003921568627451f) * blendRate;
        Color32 rColor = tColor;
        Color visitedColor = revealed;
        if (tColor[3] < visitedColor[3])
        {
            rColor = revealed;
        }
        return rColor;
    }

    void CheckRenderer(int[] fogVision)
    {
        for (int x = 0; x < hideAmount; x++)
        {
            if (x >= locH.Length)
            {
                continue;
            }
            Vector2 loc = DetermineLoc(locH[x]);
            int setting = 2;
            setting = fogVision[(int) (loc.x + loc.y * width)];
            if (setting != 0)
            {
                for (int y = 0; y < hiddenRend[x].anchors.Length; y++)
                {
                    if (fogVision[
                            (int) (loc.x + hiddenRend[x].anchors[y].x +
                                   (loc.y + hiddenRend[x].anchors[y].y) * width)] < setting)
                    {
                        setting = fogVision[
                            (int) (loc.x + hiddenRend[x].anchors[y].x +
                                   (loc.y + hiddenRend[x].anchors[y].y) * width)];
                    }
                }
            }
            hideSetting[x] = setting;
        }
    }

    public void AddAgent(GameObject obj, int sight, float uslope, float dslope, VisionSignal signal)
    {
        agents.Add(obj);
        agentsRadius.Add(sight);
        agentsUSlope.Add(uslope);
        agentsDSlope.Add(dslope);
        agentsT.Add(obj.GetComponent<Transform>());
        agentsAmount++;
    }

    public void ClosePoints(int range, Vector3 loc3d, int height)
    {
        Vector2 loc = DetermineLoc(loc3d);
        fogHeight[(int) loc.x + ((int) loc.y) * width] = -1;
        for (int x = -range; x < range; x++)
        {
            for (int y = -range; y < range; y++)
            {
                float dist = Mathf.Sqrt(x * x + y * y);
                if (dist <= range)
                {
                    fogHeight[(int) loc.x + x + ((int) loc.y + y) * width] = height;
                }
            }
        }
    }

    public void AddRenderer(GameObject obj, VisionReceiver receiver)
    {
        hiddenAgent.Add(obj);
        hiddenRend.Add(receiver);
        hiddenAgentT.Add(obj.GetComponent<Transform>());
        hideSetting = new int[hideAmount + 1];
        hideAmount++;
    }

    Vector2 DetermineLoc(Vector3 loc)
    {
        float xLoc = loc.x - startPos.x;
        float yLoc = loc.z - startPos.z;
        int x = Mathf.RoundToInt(xLoc / disp);
        int y = Mathf.RoundToInt(yLoc / disp);
        return new Vector2(x, y);
    }

    public bool CheckLocation(Vector3 loc)
    {
        Vector2 point = DetermineLoc(loc);
        return fogColor[(int) point.x + ((int) point.y) * width] != hidden;
    }
}
