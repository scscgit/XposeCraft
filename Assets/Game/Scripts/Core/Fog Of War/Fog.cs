using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors;
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

        /// <summary>
        /// List of all positions seen by an agent. Array index represents an agent, list enumerates locations.
        /// </summary>
        public List<int>[] LocationsSeenTemporary;

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
        public int[] hideNextSetting;
        public int[] hideCurrentSetting;
        public bool hideSettingCalculated;
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
            hideNextSetting = new int[hideAmount + 1];
            hideCurrentSetting = new int[hideAmount + 1];
            hideSettingCalculated = false;
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
                hideCurrentSetting[x] = (int) VisionReceiver.VisionState.Vision;
            }
        }

        /// <summary>
        /// Updates the hidden renderers based on their respective hideNextSetting.
        /// This triggers the visibility change events for signal agents.
        /// </summary>
        /// <param name="renderOnScreen"></param>
        /// <param name="fog"></param>
        /// <param name="signalAgents">Their LocationsSeenTemporary array will be used to check and trigger the
        /// visibility change event of the actor represented by the specific SignalAgent. This event will
        /// inform him of renderers that he sees. For the Attack Move purpose, this will be checked always.</param>
        public void SetRenderers(bool renderOnScreen, Fog fog, SignalAgents signalAgents)
        {
            for (int x = 0; x < hideAmount; x++)
            {
                var previousState = (VisionReceiver.VisionState) hideCurrentSetting[x];
                var newState = (VisionReceiver.VisionState) hideNextSetting[x];
                if (previousState != newState)
                {
                    if (renderOnScreen)
                    {
                        hiddenRend[x].SetRenderer(newState);
                    }
                    hideCurrentSetting[x] = hideNextSetting[x];
                }
                // Find all agents that saw the hidden agent or his change in state
                var signalIndexesSawHidden = new List<int>();
                var location = fog.DetermineLocInteger(hiddenAgentT[x].position);
                if (signalAgents.LocationsSeenTemporary == null)
                {
                    Log.w("Fog hidden agent cannot find locations of nearby signal agents");
                    return;
                }
                for (var signalIndex = 0; signalIndex < signalAgents.LocationsSeenTemporary.Length; signalIndex++)
                {
                    try
                    {
                        var agentSeenPositions = signalAgents.LocationsSeenTemporary[signalIndex];
                        if (agentSeenPositions != null && agentSeenPositions.Contains(location))
                        {
                            signalIndexesSawHidden.Add(signalIndex);
                        }
                    }
                    catch (Exception e)
                    {
                        // This problem seems to be fixed as LocationsSeenTemporary[agentIndex] just wasn't initialized
                        Log.w("LocationsSeenTemporary problem: " + e.Message);
                    }
                }

                // Conversion to Lists of GameObjects and Actors
                var signalAgentsSawHidden = new List<GameObject>();
                signalIndexesSawHidden.ForEach(signalIndex =>
                {
                    // For an unknown reason, sometimes the index would try to get accessed above the agent count
                    if (signalIndex < signalAgents.agents.Count)
                    {
                        signalAgentsSawHidden.Add(signalAgents.agents[signalIndex]);
                    }
                });
                var agentActorsSawHidden = new List<Actor>();
                signalAgentsSawHidden.ForEach(signalAgent => agentActorsSawHidden.Add(
                    GameManager.Instance.ActorLookup[signalAgent]));

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
                            player.EnemyOnSight(
                                hiddenAgent[x],
                                actor,
                                signalAgentsSawHidden,
                                agentActorsSawHidden,
                                previousState,
                                newState);
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
        private Texture2D _fog;

        //int[] fogState = new int[0];
        //bool[][] visitedPoints;

        public int FactionIndexDisplay;
        private int? _previousFactionIndexDisplay;
        public Vector3 startPos = Vector3.zero;
        Color32[][] _fogFactionColor;
        float[] fogHeight;
        private Vector3[][] _signalAgentsFactionLocations;
        private Vector3[][] _hiddenAgentsFactionLocations;
        public MiniMap map { get; set; }
        public Color revealed;
        public Color visited;
        public Color hidden;
        public Material terrainMat;
        public Texture transparentTexture;
        public LayerMask terrainLayer;
        public int blendRate = 5;
        bool funActive;
        private volatile int[][] _fogVisionEmptyPool;
        private volatile Color32[][] _fogColorEmptyPool;
        private int _fogEmptyPoolIndex;
        private volatile Thread _previousLoaderThread;

        private void OnDrawGizmos()
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

        private void OnDestroy()
        {
            // Unity Editor sometimes does not rollback the Texture change when the game is stopped, this is a failsafe
            ResetTransparentTerrain();
        }

        /// <summary>
        /// This is only for Unity testing issues
        /// </summary>
        private void ResetTransparentTerrain()
        {
            if (terrainMat != null)
            {
                terrainMat.SetTexture("_FOWTex", transparentTexture);
            }
        }

        private void Awake()
        {
            _fog = new Texture2D(width, length, TextureFormat.ARGB32, false);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    _fog.SetPixel(x, y, hidden);
                }
            }
            _fog.Apply();
            var miniMapObj = GameObject.Find("MiniMap");
            if (miniMapObj)
            {
                map = miniMapObj.GetComponent<MiniMap>();
            }
            if (map)
            {
                map.fogTexture = _fog;
            }
            terrainMat.SetTexture("_FOWTex", _fog);
            //fogState = new int[width*length];
            fogHeight = new float[width * length];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    RaycastHit hit;
                    Physics.Raycast(
                        new Vector3(x * disp + startPos.x, 1000, y * disp + startPos.z),
                        Vector3.down,
                        out hit,
                        10000,
                        terrainLayer);
                    fogHeight[x + y * width] = hit.point.y;
                }
            }
            var factions = GameManager.Instance.Factions.Length;
            _signalAgentsFactions = new SignalAgents[factions];
            _hiddenAgentsFactions = new HiddenAgents[factions];
            for (var factionIndex = 0; factionIndex < factions; factionIndex++)
            {
                _signalAgentsFactions[factionIndex] = new SignalAgents {FactionIndex = factionIndex};
                _hiddenAgentsFactions[factionIndex] = new HiddenAgents {FactionIndex = factionIndex};
            }
        }

        private void OnEnable()
        {
            // Two-dimensional array is more complicated to persist during hot-swap, so it will get re-initialized,
            // returning an entire Fog to black for a duration of one frame as a side-effect
            var factionAmount = GameManager.Instance.Factions.Length;
            _fogFactionColor = new Color32[factionAmount][];
            for (var factionIndex = 0; factionIndex < factionAmount; factionIndex++)
            {
                _fogFactionColor[factionIndex] = new Color32[width * length];
                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < length; y++)
                    {
                        _fogFactionColor[factionIndex][x + y * width] = hidden;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            // If the function is already running then we don't need to run another function
            if (!funActive)
            {
                funActive = true;
                UpdateFog();
            }
        }

        // The main fog function, it is executed whenever the thread is clear
        private void UpdateFog()
        {
            if (FactionIndexDisplay < 0 || _signalAgentsFactions.Length <= FactionIndexDisplay)
            {
                funActive = false;
                throw new Exception("Invalid Fog index of displayed faction");
            }
            // Copies locations of all agents into arrays, so they can be accessed from other than main thread
            CopyAgentLocations();
            // MultiThreaded request. This keeps the many processes off the main thread and runs them in the background
            ThreadPool.QueueUserWorkItem(ModifyFog);
            // Finally, we set and apply the new colors to the fog texture
            // Only the Faction meant to be displayed will be applied
            _fog.SetPixels32(_fogFactionColor[FactionIndexDisplay]);
            _fog.Apply(false);
            // Redundant visibility enabling, that will match owned units too, only for dynamic faction switch purpose
            // This happens before the game start too, in order to set the friendly units to visible
            if (_previousFactionIndexDisplay != FactionIndexDisplay)
            {
                foreach (var hiddenAgents in _hiddenAgentsFactions)
                {
                    hiddenAgents.DisplayAll();
                }
                _previousFactionIndexDisplay = FactionIndexDisplay;
            }
            // And we set the hidden objects as they should be seen by the displayed faction
            // We don't make the calculations when not needed though
            // Critically important is not to run on empty array, as that would trigger events based on default states
            if (!_hiddenAgentsFactions[FactionIndexDisplay].hideSettingCalculated)
            {
                return;
            }
            for (var factionIndex = 0; factionIndex < GameManager.Instance.Factions.Length; factionIndex++)
            {
                _hiddenAgentsFactions[factionIndex]
                    .SetRenderers(factionIndex == FactionIndexDisplay, this, _signalAgentsFactions[factionIndex]);
            }
        }

        private void CopyAgentLocations()
        {
            var factionsAmount = GameManager.Instance.Factions.Length;
            _signalAgentsFactionLocations = new Vector3[factionsAmount][];
            _hiddenAgentsFactionLocations = new Vector3[factionsAmount][];
            for (var factionIndex = 0; factionIndex < factionsAmount; factionIndex++)
            {
                var signalAgentsAmount = _signalAgentsFactions[factionIndex].agentsAmount;
                _signalAgentsFactionLocations[factionIndex] = new Vector3[signalAgentsAmount];
                for (var signalAgentIndex = 0; signalAgentIndex < signalAgentsAmount; signalAgentIndex++)
                {
                    if (_signalAgentsFactions[factionIndex].agents[signalAgentIndex] == null)
                    {
                        _signalAgentsFactions[factionIndex].Remove(signalAgentIndex);
                        signalAgentIndex--;
                        signalAgentsAmount--;
                        continue;
                    }
                    _signalAgentsFactionLocations[factionIndex][signalAgentIndex] =
                        _signalAgentsFactions[factionIndex].agentsT[signalAgentIndex].position;
                }
                var hiddenAgentsAmount = _hiddenAgentsFactions[factionIndex].hideAmount;
                _hiddenAgentsFactionLocations[factionIndex] = new Vector3[hiddenAgentsAmount];
                for (var hiddenAgentIndex = 0; hiddenAgentIndex < hiddenAgentsAmount; hiddenAgentIndex++)
                {
                    if (_hiddenAgentsFactions[factionIndex].hiddenAgent[hiddenAgentIndex] == null)
                    {
                        _hiddenAgentsFactions[factionIndex].Remove(hiddenAgentIndex);
                        hiddenAgentIndex--;
                        hiddenAgentsAmount--;
                        continue;
                    }
                    _hiddenAgentsFactionLocations[factionIndex][hiddenAgentIndex] =
                        _hiddenAgentsFactions[factionIndex].hiddenAgentT[hiddenAgentIndex].position;
                }
            }
        }

        private void ModifyFog(object obj)
        {
            try
            {
                // Fog of every Faction gets calculated individually
                for (var factionIndex = 0; factionIndex < GameManager.Instance.Factions.Length; factionIndex++)
                {
                    var fogColor = _fogFactionColor[factionIndex];
                    int[] fogVisionTemporary;
                    Color32[] fogColorTemporary;
                    GetPooledEmptyArrays(out fogVisionTemporary, out fogColorTemporary);
                    for (int x = 0; x < fogColor.Length; x++)
                    {
                        if (fogColor[x] == revealed)
                        {
                            fogVisionTemporary[x] = 0;
                            Color tColor = fogColor[x];
                            tColor[3] += 0.003921568627451f * blendRate;
                            fogColorTemporary[x] = tColor;
                            Color visitedColor = visited;
                            if (tColor[3] > visitedColor[3])
                            {
                                fogColorTemporary[x] = visited;
                            }
                        }
                        else if (fogColor[x] == hidden)
                        {
                            fogVisionTemporary[x] = 2;
                            fogColorTemporary[x] = hidden;
                        }
                        else
                        {
                            fogVisionTemporary[x] = 1;
                            Color tColor = fogColor[x];
                            tColor[3] += 0.003921568627451f * blendRate;
                            fogColorTemporary[x] = tColor;
                            Color visitedColor = visited;
                            if (tColor[3] > visitedColor[3])
                            {
                                fogColorTemporary[x] = visited;
                            }
                        }
                    }

                    // Display signal agents in the Fog
                    var signalAgents = _signalAgentsFactions[factionIndex];
                    // NOTE: agentsAmount changes during the iteration!
                    signalAgents.LocationsSeenTemporary = new List<int>[signalAgents.agentsAmount];
                    var locationsSeenTemporary = signalAgents.LocationsSeenTemporary;
                    for (var signalAgentIndex = 0; signalAgentIndex < locationsSeenTemporary.Length; signalAgentIndex++)
                    {
                        // For an unknown reason, this didn't work if used just as a for condition
                        if (signalAgentIndex >= _signalAgentsFactionLocations[factionIndex].Length)
                        {
                            break;
                        }
                        locationsSeenTemporary[signalAgentIndex] = new List<int>();
                        ModifyFogBySignalAgent(
                            factionIndex, signalAgentIndex, ref fogVisionTemporary, ref fogColorTemporary);
                    }

                    // Update hidden agents based on the Fog
                    var hiddenAgentsAmount = _hiddenAgentsFactions[factionIndex].hideAmount;
                    for (var hiddenAgentIndex = 0; hiddenAgentIndex < hiddenAgentsAmount; hiddenAgentIndex++)
                    {
                        if (hiddenAgentIndex >= _hiddenAgentsFactionLocations[factionIndex].Length)
                        {
                            break;
                        }
                        CheckRenderer(fogVisionTemporary, factionIndex, hiddenAgentIndex);
                    }

                    _hiddenAgentsFactions[factionIndex].hideSettingCalculated = true;
                    _fogFactionColor[factionIndex] = fogColorTemporary;
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

        /// <summary>
        /// Initializes the pool of temporary arrays used in parallel thread in order to save time later,
        //  as this previously took very large amount of time. It is assumed that all factions have the same size.
        /// </summary>
        private void GetPooledEmptyArrays(out int[] fogVisionTemporary, out Color32[] fogColorTemporary)
        {
            const int poolCount = 10;
            if (_fogVisionEmptyPool == null || _fogColorEmptyPool == null)
            {
                _fogVisionEmptyPool = new int[poolCount][];
                _fogColorEmptyPool = new Color32[poolCount][];
            }
            var loaderThread = new Thread(() =>
            {
                // Thread always waits for the previous one
                if (_previousLoaderThread != null)
                {
                    _previousLoaderThread.Join();
                }
                _previousLoaderThread = Thread.CurrentThread;
                for (var index = 0; index < poolCount; index++)
                {
                    if (_fogVisionEmptyPool[index] != null && _fogColorEmptyPool[index] != null)
                    {
                        continue;
                    }
                    _fogVisionEmptyPool[index] = new int[width * length];
                    _fogColorEmptyPool[index] = new Color32[width * length];
                }
                _previousLoaderThread = null;
            });
            loaderThread.Start();
            if (_fogVisionEmptyPool[_fogEmptyPoolIndex] == null || _fogColorEmptyPool[_fogEmptyPoolIndex] == null)
            {
                Log.d("Pooled Fog arrays are waiting for the thread to load index " + _fogEmptyPoolIndex);
                loaderThread.Join();
                Log.d("Pooled Fog arrays thread joined");
            }
            fogVisionTemporary = _fogVisionEmptyPool[_fogEmptyPoolIndex];
            _fogVisionEmptyPool[_fogEmptyPoolIndex] = null;
            fogColorTemporary = _fogColorEmptyPool[_fogEmptyPoolIndex];
            _fogColorEmptyPool[_fogEmptyPoolIndex] = null;
            if (fogVisionTemporary == null || fogColorTemporary == null)
            {
                Log.w("Fog array pool broke, generating failsafe arrays");
                fogVisionTemporary = new int[width * length];
                fogColorTemporary = new Color32[width * length];
            }
            _fogEmptyPoolIndex = (_fogEmptyPoolIndex + 1) % poolCount;
        }

        private void ModifyFogBySignalAgent(
            int factionIndex, int agentIndex, ref int[] fogVisionTemporary, ref Color32[] fogColorTemporary)
        {
            var signalAgents = _signalAgentsFactions[factionIndex];
            var location = _signalAgentsFactionLocations[factionIndex][agentIndex];
            Vector2 loc = DetermineLoc(location);
            int rad = signalAgents.agentsRadius[agentIndex];
            int locX = (int) loc.x;
            int locY = (int) loc.y;
            float uslope = signalAgents.agentsUSlope[agentIndex];
            float dslope = signalAgents.agentsDSlope[agentIndex];
            for (int z = -rad; z <= rad; z++)
            {
                Line(locX, locY, locX + z, locY - rad, rad, ref fogVisionTemporary, ref fogColorTemporary,
                    _fogFactionColor[factionIndex], location.y, uslope, dslope, agentIndex, signalAgents);
                Line(locX, locY, locX + z, locY + rad, rad, ref fogVisionTemporary, ref fogColorTemporary,
                    _fogFactionColor[factionIndex], location.y, uslope, dslope, agentIndex, signalAgents);
                Line(locX, locY, locX - rad, locY + z, rad, ref fogVisionTemporary, ref fogColorTemporary,
                    _fogFactionColor[factionIndex], location.y, uslope, dslope, agentIndex, signalAgents);
                Line(locX, locY, locX + rad, locY + z, rad, ref fogVisionTemporary, ref fogColorTemporary,
                    _fogFactionColor[factionIndex], location.y, uslope, dslope, agentIndex, signalAgents);
            }
        }

        // Bresenham's Line Algorithm at work for shadow casting
        private void Line(
            int x, int y, int x2, int y2, int radius, ref int[] fogVisionTemporary, ref Color32[] fogColorTemporary,
            Color32[] fogColor, float locY, float upwardSlopeHeight, float downwardSlopeHeight, int agentIndex,
            SignalAgents signalAgents)
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
            if (longest <= shortest)
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
                            MarkVisible(
                                x, y, agentIndex, signalAgents, ref fogVisionTemporary, ref fogColorTemporary,
                                fogColor);
                            break;
                        }
                        if (dist >= radius * radius - 2)
                        {
                            break;
                        }
                        if (locY > fogHeight[x + y * width])
                        {
                            if (locY - fogHeight[x + y * width] <= downwardSlopeHeight)
                            {
                                MarkVisible(
                                    x, y, agentIndex, signalAgents, ref fogVisionTemporary, ref fogColorTemporary,
                                    fogColor);
                            }
                            else
                            {
                                MarkVisible(
                                    x, y, agentIndex, signalAgents, ref fogVisionTemporary, ref fogColorTemporary,
                                    fogColor);
                                break;
                            }
                        }
                        else if (fogHeight[x + y * width] - locY <= upwardSlopeHeight)
                        {
                            MarkVisible(
                                x, y, agentIndex, signalAgents, ref fogVisionTemporary, ref fogColorTemporary,
                                fogColor);
                        }
                        else
                        {
                            MarkVisible(
                                x, y, agentIndex, signalAgents, ref fogVisionTemporary, ref fogColorTemporary,
                                fogColor);
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

        private void MarkVisible(
            int x, int y, int byAgentIndex, SignalAgents signalAgents, ref int[] fogVisionTemporary,
            ref Color32[] fogColorTemporary, Color32[] fogColor)
        {
            fogVisionTemporary[x + y * width] = 0;
            fogColorTemporary[x + y * width] = LerpColor(fogColor[x + y * width]);
            signalAgents.LocationsSeenTemporary[byAgentIndex].Add(x + y * width);
        }

        // A function to lerp the alpha values between two colors for the fade effect
        private Color32 LerpColor(Color32 curColor)
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

        private void CheckRenderer(int[] fogVisionTemporary, int factionIndex, int hiddenAgentIndex)
        {
            var location = _hiddenAgentsFactionLocations[factionIndex][hiddenAgentIndex];
            Vector2 loc = DetermineLoc(location);
            int setting = 2;
            setting = fogVisionTemporary[(int) (loc.x + loc.y * width)];
            var hiddenRenderer = _hiddenAgentsFactions[factionIndex].hiddenRend[hiddenAgentIndex];
            if (setting != 0)
            {
                for (int y = 0; y < hiddenRenderer.anchors.Length; y++)
                {
                    if (fogVisionTemporary[
                            (int) (loc.x + hiddenRenderer.anchors[y].x +
                                   (loc.y + hiddenRenderer.anchors[y].y) * width)] < setting)
                    {
                        setting = fogVisionTemporary[
                            (int) (loc.x + hiddenRenderer.anchors[y].x +
                                   (loc.y + hiddenRenderer.anchors[y].y) * width)];
                    }
                }
            }
            _hiddenAgentsFactions[factionIndex].hideNextSetting[hiddenAgentIndex] = setting;
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
            for (var factionIndex = 0; factionIndex < GameManager.Instance.Factions.Length; factionIndex++)
            {
                if (factionIndex == FindFactionIndex(obj))
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

        public int DetermineLocInteger(Vector3 loc)
        {
            var location = DetermineLoc(loc);
            return (int) (location.x + location.y * width);
        }


        public Vector2 DetermineLoc(Vector3 loc)
        {
            float xLoc = loc.x - startPos.x;
            float yLoc = loc.z - startPos.z;
            int x = Mathf.RoundToInt(xLoc / disp);
            int y = Mathf.RoundToInt(yLoc / disp);
            return new Vector2(x, y);
        }

        public bool CheckLocation(Vector3 loc)
        {
            var point = DetermineLoc(loc);
            // This uses current Player's context
            var factionIndex = Player.CurrentPlayer.FactionIndex;
            return _fogFactionColor[factionIndex][(int) point.x + ((int) point.y) * width] != hidden;
        }

        private static int FindFactionIndex(GameObject obj)
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
            throw new Exception("Cannot find Controller containing faction index needed for signal agent or renderer");
        }
    }
}
