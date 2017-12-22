//#define DEBUG_FORMATIONS

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Formations
{
    #region Assembling a Formation: Point

    /// <summary>
    /// Assembling a Formation: Point
    /// </summary>
    public static List<Vector3> Point(List<Unit> units, Vector3 target)
    {
        // Return a list of targets by taking the list of units and selecting the
        // same target for each unit in the list (select is used only for brevity)
        return units.Select(_ => target).ToList();
    }

    #endregion

    #region Assembling a Formation: Square Grid

    /// <summary>
    /// Assembling a Formation: Square Grid
    /// </summary>
    private static List<Vector3> SquareGridImpl(List<Unit> units, Vector3 target, float spacing)
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

    #endregion

    #region Assembling a Formation: Weird Line

    /// <summary>
    /// Assembling a Formation: Weird Line
    /// </summary>
    public static List<Vector3> WeirdLine(List<Unit> units, Vector3 target, float spacing)
    {
        // Get the size by taking the square root of the area of the grid and rounding up
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(units.Count));

        // Calculate the amount to offset the grid
        float offsetSize = gridSize * spacing;
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

    #endregion

    #region Group Movement in Formation

    /// <summary>
    /// Group Movement in Formation
    /// </summary>
    public static void SetTargets(Navigation navigation, List<Unit> units, List<Vector3> targets)
    {
        // Get the sum of all the units' positions 
        Vector3 sum = units
            .Select(u => u.gameObject.transform.position)
            .Aggregate((a, b) => a + b);

        // Find the center of the units by dividing the sum by the unit count
        Vector3 center = sum / units.Count;

        // Order the targets from furthest from the units to closest to the units
        foreach (var t in targets.OrderByDescending(t => (t - center).sqrMagnitude))
        {
            // Get the unit which is closest to the target
            Unit closestUnit = units.
                OrderBy(u => (t - u.gameObject.transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (closestUnit != null)
            {
                // Get the position of the closest unit
                Vector3 closest = closestUnit.gameObject.transform.position;

                // Find a path from the unit to the target
                Node start = navigation.GetClosestNode(closest);
                Node end = navigation.GetClosestNode(t);
                List<Node> path = navigation.FindPath(start, end);
                closestUnit.SetPath(path);

                // Remove the unit from the list
                units.Remove(closestUnit);
            }
        }
    }

    #endregion

    public static List<Vector3> SquareGrid(List<Unit> units, Vector3 target, float spacing)
    {
        List<Vector3> result = null;

#if DEBUG_FORMATIONS
        var watch = System.Diagnostics.Stopwatch.StartNew();
#endif

        result = SquareGridImpl(units, target, spacing);

#if DEBUG_FORMATIONS
        watch.Stop();
        if (watch.Elapsed.TotalMilliseconds > 0)
        {
            Debug.Log("SquareGrid: " + watch.Elapsed.TotalMilliseconds + "ms");
            using (System.IO.StreamWriter ss = System.IO.File.AppendText("squaregrid.csv"))
                ss.WriteLine(watch.Elapsed.TotalMilliseconds.ToString());
        }
#endif

        return result;
    }
}
