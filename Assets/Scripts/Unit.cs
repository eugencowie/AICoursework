using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seek))]
public class Unit : MonoBehaviour
{
    private Seek m_seek;

    private bool m_selected;

    private List<Node> m_path = new List<Node>();
    int m_pathIndex = 0;

    public bool Selected
    {
        get { return m_selected; }
        set { gameObject.SetColor((m_selected = value) ? Color.yellow : Color.white); }
    }

    private void Start()
    {
        m_seek = GetComponent<Seek>();
        m_seek.onArrival = OnArrival;
        m_selected = false;
    }
    
    public void ToggleSelected()
    {
        Selected = !Selected;
    }

    public void SetPath(List<Node> path)
    {
        m_path = path;
        m_pathIndex = 0;
        
        if (m_path.Count > 0)
        {
            SetTarget(m_path[m_pathIndex].Position);
        }
    }

    private void SetTarget(Vector3 target)
    {
        m_seek.target.transform.SetParent(gameObject.transform.parent);
        m_seek.target.transform.position = target;
        m_seek.target.gameObject.SetActive(true);
    }

    private void OnArrival()
    {
        m_pathIndex++;

        if (m_pathIndex < m_path.Count)
        {
            SetTarget(m_path[m_pathIndex].Position);
        }
    }
}
