using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(Seek))]
public class UnitController : MonoBehaviour
{
    private Renderer m_renderer;
    private Seek m_seek;

    private bool m_isSelected;

    private void Start()
    {
        m_renderer = GetComponent<Renderer>();
        m_seek = GetComponent<Seek>();

        m_isSelected = false;
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
                m_renderer.material.color = (m_isSelected ? Color.yellow : Color.white);
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
                m_seek.Target.transform.SetParent(gameObject.transform.parent);
                m_seek.Target.transform.position = hit.point;
                m_seek.Target.gameObject.SetActive(true);
            }
        }
    }
}
