using UnityEngine;

public class UnitController : MonoBehaviour
{
    private bool m_isSelected;
    
    private void Update()
    {
        CheckForSelection();
        CheckForMoveCommand();
    }

    private void CheckForSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.gameObject == gameObject)
            {
                m_isSelected = !m_isSelected;
                GetComponent<Renderer>().material.color = (m_isSelected ? Color.yellow : Color.white);
            }
        }
    }

    private void CheckForMoveCommand()
    {
        if (m_isSelected && Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.gameObject != gameObject)
            {
                transform.position = hit.point + new Vector3(0, GetComponent<Collider>().bounds.extents.y, 0);
            }
        }
    }
}
