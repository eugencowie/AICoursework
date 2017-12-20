using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Formations
{
    public static List<Vector3> Point(List<Unit> units, Vector3 target)
    {
        // Return a list of targets by taking the list of units and selecting the
        // same target for each unit in the list (select is used only for brevity)
        return units.Select(_ => target).ToList();
    }

    public static List<Vector3> SquareGrid(List<Unit> units, Vector3 target, float spacing)
    {
        // Get the size by taking the square root of the area of the grid and rounding up
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(units.Count));

        // Calculate the amount to offset the grid in order to centre it
        float offsetSize = ((gridSize - 1) * spacing) / 2;
        Vector3 offset = new Vector3(offsetSize, offsetSize);

        // Set the start and end points for the grid
        Vector2 start = -offset;
        Vector2 end = offset;

        List<Vector3> targets = new List<Vector3>();

        // Add a new target for each point in the grid
        for (float z = start.y; z <= end.y; z += spacing)
            for (float x = start.x; x <= end.x; x += spacing)
                targets.Add(target + new Vector3(x, 0, z));

        return targets;
    }

    public static List<Vector3> WeirdLine(List<Unit> units, Vector3 target, float distance)
    {
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(units.Count));

        Vector3 start = new Vector3();
        start.x = start.z = -(distance * gridSize);
        Vector3 end = -start;

        List<Vector3> targets = new List<Vector3>();

        for (float z = start.z; z <= end.z; z += distance)
        {
            for (float x = start.x; x <= end.x; x += distance)
            {
                targets.Add(target + new Vector3(x, 0, z));
            }
        }

        return targets;
    }
}
