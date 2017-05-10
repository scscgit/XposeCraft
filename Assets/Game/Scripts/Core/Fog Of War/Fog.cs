using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.GameInternal;
using XposeCraft.UI.MiniMap;

namespace XposeCraft.Core.Fog_Of_War
{
    [Serializable]
    public class SignalAgents
    {
        public List<GameObject> agents;
        public List<int> agentsRadius;
        public List<float> agentsUSlope;
        public List<float> agentsDSlope;
        public List<Transform> agentsT;
        public int agentsAmount;
        public int FactionIndex;

        public SignalAgents()
        {
            agents = new List<GameObject>(0);
            agentsRadius = new List<int>(0);
            agentsUSlope = new List<float>(0);
            agentsDSlope = new List<float>(0);
            agentsT = new List<Transform>(0);
        }

        public void Remove(int x)
        {
            agents.RemoveAt(x);
            agentsT.RemoveAt(x);
            agentsRadius.RemoveAt(x);
            agentsUSlope.RemoveAt(x);
            agentsDSlope.RemoveAt(x);
            agentsAmount--;
        }

        public void Remove(GameObject obj)
        {
            agents[agents.IndexOf(obj)] = null;
        }

        public void Add(GameObject obj, int sight, float uslope, float dslope, VisionSignal signal)
        {
            agents.Add(obj);
            agentsRadius.Add(sight);
            agentsUSlope.Add(uslope);
            agentsDSlope.Add(dslope);
            agentsT.Add(obj.GetComponent<Transform>());
            agentsAmount++;
        }
    }

    [Serializable]
    public class HiddenAgents
    {
        public List<GameObject> hiddenAgent;
        public List<VisionReceiver> hiddenRend;
        public List<Transform> hiddenAgentT;
        public int[] hideSetting;
        public int hideAmount;
        public int FactionIndex;

        public HiddenAgents()
        {
            hiddenAgent = new List<GameObject>(0);
            hiddenRend = new List<VisionReceiver>(0);
            hiddenAgentT = new List<Transform>(0);
        }

        public void Add(GameObject gameObject, VisionReceiver receiver)
        {
            hiddenAgent.Add(gameObject);
            hiddenRend.Add(receiver);
            hiddenAgentT.Add(gameObject.GetComponent<Transform>());
            hideSetting = new int[hideAmount + 1];
            hideAmount++;
        }

        public void Remove(int x)
        {
            hiddenAgent.RemoveAt(x);
            hiddenAgentT.RemoveAt(x);
            hiddenRend.RemoveAt(x);
            hideAmount--;
        }

        public void Remove(GameObject obj)
        {
            if (!hiddenAgent.Contains(obj))
            {
                // Exceptions of visibility like having the same faction can cause this to be called without any need
                return;
            }
            Remove(hiddenAgent.IndexOf(obj));
        }

        public void DisplayAll()
        {
            for (int x = 0; x < hideAmount; x++)
            {
                hiddenRend[x].SetRenderer(VisionReceiver.VisionState.Vision);
            }
        }

        public void SetRenderers()
        {
            for (int x = 0; x < hideAmount; x++)
            {
                var previousState = hiddenRend[x].curState;
                var newState = (VisionReceiver.VisionState) hideSetting[x];
                if (previousState == newState)
                {
                    continue;
                }
                hiddenRend[x].SetRenderer(newState);
                // Notifies Players about the change
                if (!GameManager.Instance.ActorLookup.ContainsKey(hiddenAgent[x]))
                {
                    // If the lookup fails, it is because the game is still initializing. Events arent required here
                    continue;
                }
                var actor = GameManager.Instance.ActorLookup[hiddenAgent[x]];
                var owner = GameManager.Instance.FindPlayerOfActor(actor);
                foreach (var enemyFactionIndex in owner.Faction.EnemyFactionIndexes())
                {
                    foreach (var player in GameManager.Instance.Players)
                    {
                        if (player.FactionIndex == enemyFactionIndex)
                        {
                            player.EnemyVisibilityChanged(actor, previousState, newState);
                        }
                    }
                }
            }
        }
    }

    public class Fog : MonoBehaviour
    {
        public const string ScriptName = "Fog";

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

        private SignalAgents[] _signalAgentsFactions;

        // Much like the above lists, this keeps track of the objects you want hidden

        private HiddenAgents[] _hiddenAgentsFactions;

        // The fog variable is the texture you are actually modifying for the fog system
        Texture2D fog;

        //int[] fogState = new int[0];

        public int FactionIndexDisplay;
        public Vector3 startPos = Vector3.zero;
        Color32[] fogColor;
        float[] fogHeight;
        Vector3[] locA;
        Vector3[] locH;
        public MiniMap map { get; set; }
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
            if (Application.isPlaying)
            {
                return;
            }
            // This is required so that the terrain's mat will be reset after the game ends
            ResetTransparentTerrain();

            if (gameObject.name != ScriptName)
            {
                gameObject.name = ScriptName;
            }
        }

        void OnDestroy()
        {
            // Unity Editor sometimes does not rollback the Texture change when the game is stopped, this is a failsafe
            ResetTransparentTerrain();
        }

        /// <summary>
        /// This is only for Unity testing issues
        /// </summary>
        void ResetTransparentTerrain()
        {
            if (terrainMat != null)
            {
                terrainMat.SetTexture("_FOWTex", transparentTexture);
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
            var miniMapObj = GameObject.Find("MiniMap");
            if (miniMapObj)
            {
                map = miniMapObj.GetComponent<MiniMap>();
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

        private void Start()
        {
            var factions = GameManager.Instance.Factions.Length;
            _signalAgentsFactions = new SignalAgents[factions];
            _hiddenAgentsFactions = new HiddenAgents[factions];
            for (var factionIndex = 0; factionIndex < factions; factionIndex++)
            {
                _signalAgentsFactions[factionIndex] = new SignalAgents {FactionIndex = factionIndex};
                _hiddenAgentsFactions[factionIndex] = new HiddenAgents {FactionIndex = factionIndex};
            }
        }

        void FixedUpdate()
        {
            // If the function is already running then we don't need to run another function
            if (!funActive)
            {
                funActive = true;
                UpdateFog();
            }
        }

        // The main fog function, it is executed whenever the thread is clear
        void UpdateFog()
        {
            if (FactionIndexDisplay < 0 || _signalAgentsFactions.Length <= FactionIndexDisplay)
            {
                funActive = false;
                throw new Exception("Invalid Fog index of displayed faction");
            }
            locA = new Vector3[_signalAgentsFactions[FactionIndexDisplay].agentsAmount];
            for (int x = 0; x < _signalAgentsFactions[FactionIndexDisplay].agentsAmount; x++)
            {
                if (_signalAgentsFactions[FactionIndexDisplay].agents[x] == null)
                {
                    _signalAgentsFactions[FactionIndexDisplay].Remove(x);
                    x--;
                }
                else
                {
                    locA[x] = _signalAgentsFactions[FactionIndexDisplay].agentsT[x].position;
                }
            }
            locH = new Vector3[_hiddenAgentsFactions[FactionIndexDisplay].hideAmount];
            for (int x = 0; x < _hiddenAgentsFactions[FactionIndexDisplay].hideAmount; x++)
            {
                if (_hiddenAgentsFactions[FactionIndexDisplay].hiddenAgent[x] == null)
                {
                    _hiddenAgentsFactions[FactionIndexDisplay].Remove(x);
                    x--;
                }
                else
                {
                    locH[x] = _hiddenAgentsFactions[FactionIndexDisplay].hiddenAgentT[x].position;
                }
            }
            // MultiThreaded request. This keeps the many processes off the main thread and runs them in the background
            ThreadPool.QueueUserWorkItem(ModifyFog);
            // Finally, we set and apply the new colors to the fog texture
            fog.SetPixels32(fogColor);
            fog.Apply(false);

            // Redundant visibility enabling, that will match owned units too, only for dynamic faction switch purpose
            foreach (var hiddenAgents in _hiddenAgentsFactions)
            {
                hiddenAgents.DisplayAll();
            }
            // And we set the hidden objects as they should be seen by the displayed faction
            _hiddenAgentsFactions[FactionIndexDisplay].SetRenderers();
        }

        void ModifyFog(object obj)
        {
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
            try
            {
                for (int x = 0; x < _signalAgentsFactions[FactionIndexDisplay].agentsAmount; x++)
                {
                    if (x >= locA.Length)
                    {
                        continue;
                    }
                    Vector2 loc = DetermineLoc(locA[x]);
                    int rad = _signalAgentsFactions[FactionIndexDisplay].agentsRadius[x];
                    int locX = (int) loc.x;
                    int locY = (int) loc.y;
                    float uslope = _signalAgentsFactions[FactionIndexDisplay].agentsUSlope[x];
                    float dslope = _signalAgentsFactions[FactionIndexDisplay].agentsDSlope[x];
                    for (int z = -rad; z <= rad; z++)
                    {
                        Line(locX, locY, locX + z, locY - rad, rad, ref fogVision, ref fColor, locA[x].y, uslope,
                            dslope);
                        Line(locX, locY, locX + z, locY + rad, rad, ref fogVision, ref fColor, locA[x].y, uslope,
                            dslope);
                        Line(locX, locY, locX - rad, locY + z, rad, ref fogVision, ref fColor, locA[x].y, uslope,
                            dslope);
                        Line(locX, locY, locX + rad, locY + z, rad, ref fogVision, ref fColor, locA[x].y, uslope,
                            dslope);
                    }
                }
                fogColor = fColor;
                for (int factionIndex = 0; factionIndex < GameManager.Instance.Factions.Length; factionIndex++)
                {
                    CheckRenderer(fogVision, factionIndex);
                }
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
                if (numerator >= longest)
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
            tColor[3] -= 0.003921568627451f * blendRate;
            Color32 rColor = tColor;
            Color visitedColor = revealed;
            if (tColor[3] < visitedColor[3])
            {
                rColor = revealed;
            }
            return rColor;
        }

        void CheckRenderer(int[] fogVision, int factionIndex)
        {
            for (int x = 0; x < _hiddenAgentsFactions[factionIndex].hideAmount; x++)
            {
                if (x >= locH.Length)
                {
                    continue;
                }
                Vector2 loc = DetermineLoc(locH[x]);
                int setting = 2;
                setting = fogVision[(int) (loc.x + loc.y * width)];
                var hiddenRenderer = _hiddenAgentsFactions[factionIndex].hiddenRend[x];
                if (setting != 0)
                {
                    for (int y = 0; y < hiddenRenderer.anchors.Length; y++)
                    {
                        if (fogVision[
                                (int) (loc.x + hiddenRenderer.anchors[y].x +
                                       (loc.y + hiddenRenderer.anchors[y].y) * width)] < setting)
                        {
                            setting = fogVision[
                                (int) (loc.x + hiddenRenderer.anchors[y].x +
                                       (loc.y + hiddenRenderer.anchors[y].y) * width)];
                        }
                    }
                }
                _hiddenAgentsFactions[factionIndex].hideSetting[x] = setting;
            }
        }

        public void AddSignalAgent(GameObject obj, int sight, float uslope, float dslope, VisionSignal signal)
        {
            _signalAgentsFactions[FindFactionIndex(obj)].Add(obj, sight, uslope, dslope, signal);
        }

        public void RemoveSignalAgent(GameObject obj)
        {
            _signalAgentsFactions[FindFactionIndex(obj)].Remove(obj);
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
            int ownerFaction;
            var unit = obj.GetComponent<UnitController>();
            if (unit != null)
            {
                ownerFaction = unit.FactionIndex;
            }
            else
            {
                var building = obj.GetComponent<BuildingController>();
                if (building != null)
                {
                    ownerFaction = building.FactionIndex;
                }
                else
                {
                    throw new Exception("Actor cannot determine its faction index for visibility exception purposes");
                }
            }
            for (var factionIndex = 0; factionIndex < GameManager.Instance.Factions.Length; factionIndex++)
            {
                if (factionIndex == ownerFaction)
                {
                    // Player that owns the unit will not be included in its hiding
                    continue;
                }
                _hiddenAgentsFactions[factionIndex].Add(obj, receiver);
            }
        }

        public void RemoveRenderer(GameObject obj)
        {
            for (var factionIndex = 0; factionIndex < GameManager.Instance.Factions.Length; factionIndex++)
            {
                _hiddenAgentsFactions[factionIndex].Remove(obj);
            }
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

        private int FindFactionIndex(GameObject obj)
        {
            var unitController = obj.GetComponent<UnitController>();
            if (unitController != null)
            {
                return unitController.FactionIndex;
            }
            var buildingController = obj.GetComponent<BuildingController>();
            if (buildingController != null)
            {
                return buildingController.FactionIndex;
            }
            throw new Exception("Cannot find Controller containing faction index needed for signal agent");
        }
    }
}
