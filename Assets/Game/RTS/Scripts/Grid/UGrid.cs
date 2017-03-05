using UnityEngine;
using System.Collections.Generic;

public class UGrid : MonoBehaviour
{
    public Grid[] grids = new Grid[1];
    public bool generate;
    public int index;
    public APath pathfinding;
    public Texture selectionTexture;

    void OnDrawGizmos()
    {
        if (gameObject.name != "UGrid")
        {
            gameObject.name = "UGrid";
        }
        // Regenerating the Grid when the Editor Scene view loads
        if (grids[index].points == null)
        {
            generate = true;
        }
        if (generate)
        {
            GenerateGrid(index);
            generate = false;
        }
        if (index < grids.Length)
        {
            grids[index].DisplayGrid(0);
        }
    }

    void OnEnable()
    {
        // Regenerating the Grid when the Game starts
        GenerateGrid(index);
    }

    public void GenerateGrid(int i)
    {
        // Generating the grid is a multi-method process
        int gridSize = grids[i].size;
        grids[i].points = new GridPoint[gridSize * gridSize];
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                grids[i].points[x + y * gridSize] =
                    new GridPoint(new Vector3(
                        grids[i].startLoc.x + x * grids[i].nodeDist,
                        grids[i].startLoc.y,
                        grids[i].startLoc.z + y * grids[i].nodeDist))
                    {
                        index = x + y * gridSize
                    };
            }
        }
        if (grids[i].displaceDown)
        {
            DisplacePoints(i);
        }

        AssignChildren(i);

        CheckState(i);
    }

    // For Edges, children are assigned to each point
    // If octile connection = false then just 4 children will be assigned, otherwise 8
    public void AssignChildren(int i)
    {
        int gridSize = grids[i].size;
        for (int x = 0; x < grids[i].points.Length; x++)
        {
            GridPoint point = grids[i].points[x];
            int childAmount = 0;
            bool[] child = new bool[8];
            // We First determine the amount of children necessary
            if (x - 1 > 0 && x % gridSize != 0)
            {
                child[0] = true;
                childAmount++;
                if (grids[i].checkSlope
                    && Mathf.Abs(point.loc.y - grids[i].points[x - 1].loc.y) > grids[i].slopeMax)
                {
                    for (int z = 0; z < child.Length; z++)
                    {
                        child[z] = false;
                        childAmount--;
                    }
                }
            }
            if (x + 1 < grids[i].points.Length && (x + 1) % gridSize != 0)
            {
                child[1] = true;
                childAmount++;
                if (grids[i].checkSlope
                    && Mathf.Abs(point.loc.y - grids[i].points[x + 1].loc.y) > grids[i].slopeMax)
                {
                    for (int z = 0; z < child.Length; z++)
                    {
                        child[z] = false;
                        childAmount--;
                    }
                }
            }
            if (x - gridSize > 0)
            {
                child[2] = true;
                childAmount++;
                if (grids[i].checkSlope
                    && Mathf.Abs(point.loc.y - grids[i].points[x - gridSize].loc.y) > grids[i].slopeMax)
                {
                    for (int z = 0; z < child.Length; z++)
                    {
                        child[z] = false;
                        childAmount--;
                    }
                }
            }
            if (x + gridSize < grids[i].points.Length)
            {
                child[3] = true;
                childAmount++;
                if (grids[i].checkSlope
                    && Mathf.Abs(point.loc.y - grids[i].points[x + gridSize].loc.y) > grids[i].slopeMax)
                {
                    for (int z = 0; z < child.Length; z++)
                    {
                        child[z] = false;
                        childAmount--;
                    }
                }
            }
            if (grids[i].octileConnection)
            {
                if (x - gridSize - 1 > 0 && x % gridSize != 0)
                {
                    child[4] = true;
                    childAmount++;
                    if (grids[i].checkSlope
                        && Mathf.Abs(point.loc.y - grids[i].points[x - gridSize - 1].loc.y) > grids[i].slopeMax)
                    {
                        for (int z = 0; z < child.Length; z++)
                        {
                            child[z] = false;
                            childAmount--;
                        }
                    }
                }
                if (x - gridSize + 1 > 0 && (x + 1) % gridSize != 0)
                {
                    child[5] = true;
                    childAmount++;
                    if (grids[i].checkSlope
                        && Mathf.Abs(point.loc.y - grids[i].points[x - gridSize + 1].loc.y) > grids[i].slopeMax)
                    {
                        for (int z = 0; z < child.Length; z++)
                        {
                            child[z] = false;
                            childAmount--;
                        }
                    }
                }
                if (x - 1 + gridSize < grids[i].points.Length && x % gridSize != 0)
                {
                    child[6] = true;
                    childAmount++;
                    if (grids[i].checkSlope
                        && Mathf.Abs(point.loc.y - grids[i].points[x - 1 + gridSize].loc.y) > grids[i].slopeMax)
                    {
                        for (int z = 0; z < child.Length; z++)
                        {
                            child[z] = false;
                            childAmount--;
                        }
                    }
                }
                if (x + gridSize + 1 < grids[i].points.Length && (x + 1) % gridSize != 0)
                {
                    child[7] = true;
                    childAmount++;
                    if (grids[i].checkSlope
                        && Mathf.Abs(point.loc.y - grids[i].points[x + gridSize + 1].loc.y) > grids[i].slopeMax)
                    {
                        for (int z = 0; z < child.Length; z++)
                        {
                            child[z] = false;
                            childAmount--;
                        }
                    }
                }
            }
            int y = 0;
            // Then we add those children to the point
            if (childAmount > 0)
            {
                point.children = new int[childAmount];
                if (child[0])
                {
                    point.children[y] = x - 1;
                    y++;
                }
                if (child[1])
                {
                    point.children[y] = x + 1;
                    y++;
                }
                if (child[2])
                {
                    point.children[y] = x - gridSize;
                    y++;
                }
                if (child[3])
                {
                    point.children[y] = x + gridSize;
                    y++;
                }
                // Octile Generation
                if (!grids[i].octileConnection)
                {
                    continue;
                }
                if (child[4])
                {
                    point.children[y] = x - gridSize - 1;
                    y++;
                }
                if (child[5])
                {
                    point.children[y] = x - gridSize + 1;
                    y++;
                }
                if (child[6])
                {
                    point.children[y] = x - 1 + gridSize;
                    y++;
                }
                if (child[7])
                {
                    point.children[y] = x + gridSize + 1;
                }
            }
            else
            {
                point.children = new int[0];
            }
        }
    }

    // Sets gridpoints y location for grids[i] equal to closest downward collision point within the proper layer
    public void DisplacePoints(int i)
    {
        for (var x = 0; x < grids[i].points.Length; x++)
        {
            GridPoint point = grids[i].points[x];
            RaycastHit hit;
            Physics.Raycast(point.loc, Vector3.down, out hit, 10000, grids[i].displaceLayer);
            if (hit.collider)
            {
                point.loc = new Vector3(point.loc.x, hit.point.y, point.loc.z);
            }
        }
    }

    public void CheckState(int i)
    {
        // TODO have grid automate open/closed state
        for (var x = 0; x < grids[i].points.Length; x++)
        {
            GridPoint point = grids[i].points[x];
            Collider[] coll = Physics.OverlapSphere(point.loc, grids[i].nodeDist / 2.25f, grids[i].checkLayer);
            // Changed the requirement to just one collision
            if (coll.Length >= 1)
            {
                point.state = 2;
            }
        }
        for (var x = 0; x < grids[i].points.Length; x++)
        {
            GridPoint point = grids[i].points[x];
            RaycastHit hit;
            Physics.Raycast(
                new Vector3(point.loc.x, point.loc.y + 100, point.loc.z),
                Vector3.down,
                out hit,
                10000,
                grids[i].checkLayer);
            if (hit.collider)
            {
                point.state = 2;
            }
        }
    }


    public int DetermineLoc(Vector3 loc, int gridI)
    {
        float xLoc = loc.x - grids[gridI].startLoc.x;
        float yLoc = loc.z - grids[gridI].startLoc.z;
        int x = Mathf.RoundToInt(xLoc / grids[gridI].nodeDist);
        int y = Mathf.RoundToInt(yLoc / grids[gridI].nodeDist);
        return x + y * grids[gridI].size;
    }

    public Vector3 DetermineNearestPoint(Vector3 startPoint, Vector3 point, int i)
    {
        int loc = DetermineLoc(point, i);
        if (grids[i].points[loc].state != 2)
        {
            return grids[i].points[loc].loc;
        }
        return FindNearestPoint(startPoint, loc, i, Mathf.Infinity);
    }

    public Vector3 FindNearestPoint(Vector3 startPoint, int point, int i, float dist)
    {
        bool found = false;
        List<int> children = new List<int>();
        int childAmount = 1;
        children.Add(point);
        Vector3 finalLoc = Vector3.zero;
        //int finalIndex = 0;
        int z = 0;
        bool[] checkList = new bool[grids[i].points.Length];
        while (!found || childAmount > 0)
        {
            int[] pointChildren = grids[i].points[children[0]].children;
            for (int x = 0; x < pointChildren.Length; x++)
            {
                int child = pointChildren[x];
                z++;
                if (grids[i].points[child].state != 2)
                {
                    if (!found)
                    {
                        found = true;
                        Vector3 nLoc = grids[i].points[child].loc;
                        float curDist = (startPoint - nLoc).sqrMagnitude;
                        dist = curDist;
                        finalLoc = nLoc;
                        //finalIndex = pointChildren[x];
                    }
                    else
                    {
                        Vector3 nLoc = grids[i].points[child].loc;
                        float curDist = (startPoint - nLoc).sqrMagnitude;
                        if (!(curDist < dist))
                        {
                            continue;
                        }
                        dist = curDist;
                        //finalIndex = pointChildren[x];
                        finalLoc = nLoc;
                    }
                }
                else
                {
                    if (found || checkList[child])
                    {
                        continue;
                    }
                    children.Add(child);
                    checkList[child] = true;
                    childAmount++;
                }
            }
            children.RemoveAt(0);
            childAmount--;
        }
        return finalLoc;
    }
}
