using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Input : MonoBehaviour
{
    public Navigation Navigation;

    public LayerMask GroundLayer;

    private List<Unit> m_selectedUnits = new List<Unit>();

    private void Update()
    {
        CheckSelection();
        CheckCommand();
    }

    private void CheckSelection()
    {
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Unit unitController = hit.transform.gameObject.GetComponent<Unit>();

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
        if (UnityEngine.Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GroundLayer) && m_selectedUnits.Any())
            {
                List<Vector3> targets = null;
                if (m_selectedUnits.Count < 2)
                {
                    targets = Formations.Point(m_selectedUnits, hit.point);
                }
                else
                {
                    targets = Formations.SquareGrid(m_selectedUnits, hit.point, 2.0f);
                }

                if (targets != null)
                {
                    SetTargets(m_selectedUnits.ToList(), targets);
                }
            }
        }
    }

    private void SetTargets(List<Unit> units, List<Vector3> targets)
    {
        Vector3 current = new Vector3();
        units.ForEach(u => current += u.gameObject.transform.position);
        current /= units.Count;

        foreach (var t in targets.OrderByDescending(t => (t - current).sqrMagnitude))
        {
            Unit closest = units.OrderBy(u => (t - u.gameObject.transform.position).sqrMagnitude).FirstOrDefault();
            if (closest != null)
            {
                Node start = Navigation.GetClosestNode(closest.gameObject.transform.position);
                Node end = Navigation.GetClosestNode(t);
                List<Node> path = Navigation.FindPath(start, end);

                closest.SetPath(path);
                units.Remove(closest);
            }
        }
    }
}
