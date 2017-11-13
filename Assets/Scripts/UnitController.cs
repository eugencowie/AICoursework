using UnityEngine;

[RequireComponent(typeof(Seek))]
public class UnitController : MonoBehaviour
{
    private Seek m_seek;
    private bool m_isSelected;

    private void Start()
    {
        m_seek = GetComponent<Seek>();
    }

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
                m_seek.Target = hit.point + new Vector3(0, GetComponent<Collider>().bounds.extents.y, 0);
            }
        }
    }
}
