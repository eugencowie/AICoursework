using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seek))]
public class Unit : MonoBehaviour
{
    private Seek seek;
    private bool selected;

    private List<Node> path;
    private int pathIndex;

    private void Start()
    {
        seek = GetComponent<Seek>();
        seek.onArrival = OnArrival;

        selected = false;

        path = new List<Node>();
        pathIndex = 0;
    }

    private void OnArrival()
    {
        pathIndex++;

        if (pathIndex < path.Count)
        {
            SetTarget(path[pathIndex].Position);
        }
    }

    public void SetPath(List<Node> newPath)
    {
        path = newPath;
        pathIndex = 0;
        
        if (pathIndex < path.Count)
        {
            SetTarget(path[pathIndex].Position);
        }
    }

    private void SetTarget(Vector3 target)
    {
        seek.target.transform.SetParent(gameObject.transform.parent);
        seek.target.transform.position = target;
        seek.target.gameObject.SetActive(true);
    }

    public bool Selected
    {
        get { return selected; }
        set { gameObject.SetColor((selected = value) ? Color.yellow : Color.white); }
    }

    public void ToggleSelected()
    {
        Selected = !Selected;
    }
}
