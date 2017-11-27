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
                if (m_selectedUnits.Count < 2)
                {
                    Point(m_selectedUnits.ToList(), hit.point);
                }
                else
                {
                    SquareGrid(m_selectedUnits.ToList(), hit.point, 2.0f);
                }
            }
        }
    }

    private static void Point(List<UnitController> units, Vector3 target)
    {
        units.ForEach(u => u.SetTarget(target));
    }

    private static void SquareGrid(List<UnitController> units, Vector3 target, float distance)
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
}
