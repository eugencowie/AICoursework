﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Input : MonoBehaviour
{
    public enum Formation { Point, SquareGrid, WeirdLine }

    public Navigation Navigation;

    public LayerMask GroundLayer;

    public Formation FormationType;

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
                else switch (FormationType)
                {
                    case Formation.Point:
                        targets = Formations.Point(m_selectedUnits, hit.point);
                        break;

                    case Formation.WeirdLine:
                        targets = Formations.WeirdLine(m_selectedUnits, hit.point, 2.0f);
                        break;

                    default:
                    case Formation.SquareGrid:
                        targets = Formations.SquareGrid(m_selectedUnits, hit.point, 2.0f);
                        break;
                }

                if (targets != null)
                {
                    Formations.SetTargets(Navigation, m_selectedUnits.ToList(), targets);
                }
            }
        }
    }
}
