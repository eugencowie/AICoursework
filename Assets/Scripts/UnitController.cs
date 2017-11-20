using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(Seek))]
public class UnitController : MonoBehaviour
{
    private Renderer m_renderer;
    private Seek m_seek;

    private bool m_selected;

    public bool Selected
    {
        get { return m_selected; }
        set {
            m_selected = value;
            m_renderer.material.color = (value ? Color.yellow : Color.white);
        }
    }

    private void Start()
    {
        m_renderer = GetComponent<Renderer>();
        m_seek = GetComponent<Seek>();

        m_selected = false;
    }

    public void ToggleSelected()
    {
        Selected = !Selected;
    }

    public void SetTarget(Vector3 target)
    {
        m_seek.Target.transform.SetParent(gameObject.transform.parent);
        m_seek.Target.transform.position = target;
        m_seek.Target.gameObject.SetActive(true);
    }
}
