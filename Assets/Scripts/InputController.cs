using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public LayerMask GroundLayer;

    private List<UnitController> m_selectedUnits = new List<UnitController>();

    private void Update()
    {
        CheckSelection();
        CheckCommand();
    }

    private void CheckSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                UnitController unitController = hit.transform.gameObject.GetComponent<UnitController>();

                if (unitController != null)
                {
                    unitController.ToggleSelected();

                    if (unitController.Selected)
                    {
                        m_selectedUnits.Add(unitController);
                    }
                    else
                    {
                        m_selectedUnits.Remove(unitController);
                    }
                }
            }
        }
    }

    private void CheckCommand()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GroundLayer) && m_selectedUnits.Any())
            {
                List<Vector3> targets = null;
                if (m_selectedUnits.Count < 2)
                {
                    targets = Point(m_selectedUnits, hit.point);
                }
                else
                {
                    targets = SquareGrid(m_selectedUnits, hit.point, 2.0f);
                }

                if (targets != null)
                {
                    SetTargets(m_selectedUnits.ToList(), targets);
                }
            }
        }
    }

    private static void SetTargets(List<UnitController> units, List<Vector3> targets)
    {
        Vector3 current = new Vector3();
        units.ForEach(u => current += u.gameObject.transform.position);
        current /= units.Count;

        foreach (var t in targets.OrderByDescending(t => (t - current).sqrMagnitude))
        {
            UnitController closest = units.OrderBy(u => (t - u.gameObject.transform.position).sqrMagnitude).FirstOrDefault();
            if (closest != null)
            {
                closest.SetTarget(t);
                units.Remove(closest);
            }
        }
    }

    private static List<Vector3> Point(List<UnitController> units, Vector3 target)
    {
        List<Vector3> targets = new List<Vector3>();
        units.ForEach(u => targets.Add(target));
        return targets;
    }
    
    private static List<Vector3> SquareGrid(List<UnitController> units, Vector3 target, float distance)
    {
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(units.Count));

        Vector3 start = new Vector3();
        start.x = start.z = -(distance / gridSize);
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
