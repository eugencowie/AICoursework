using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private List<GameObject> m_selectedUnits = new List<GameObject>();

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
                GameObject hitObject = hit.transform.gameObject;
                UnitController unitController = hitObject.GetComponent<UnitController>();

                if (unitController != null)
                {
                    unitController.ToggleSelected();
                    if (unitController.Selected)
                    {
                        m_selectedUnits.Add(hitObject);
                    }
                    else
                    {
                        m_selectedUnits.Remove(hitObject);
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
            if (Physics.Raycast(ray, out hit) && m_selectedUnits.Any())
            {
                foreach (var unit in m_selectedUnits)
                {
                    UnitController unitController = unit.GetComponent<UnitController>();
                    if (unitController != null)
                    {
                        unitController.SetTarget(hit.point);
                    }
                    unitController.Selected = false;
                }
                m_selectedUnits.Clear();
            }
        }
    }
}
