using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public Transform Transform;
    public List<Connection> Connections;

    public Node(Transform transform)
    {
        Transform = transform;
        Connections = new List<Connection>();
    }

    public Vector3 Position => Transform.position;
}

public class Connection
{
    public Node Node;
    public float Cost;

    public Connection(Node node, float cost)
    {
        Node = node;
        Cost = cost;
    }
}

public class Navigation : MonoBehaviour
{
    public GameObject ColliderContainer;
    public GameObject Sphere;

    public Vector2 Size = new Vector2(60, 60);
    public float NodeSpacing = 2;

    private List<Collider> m_colliders = new List<Collider>();
    private List<Node> m_nodes = new List<Node>();

    private void Start()
    {
        m_colliders = GetColliders(ColliderContainer);
        GenerateNodes();
        ConnectNodes();
    }

    private void Update()
    {
        DrawDebugLines();
    }

    private static List<Collider> GetColliders(GameObject container)
    {
        List<Collider> colliders = new List<Collider>();

        IEnumerable e = container.transform;

        foreach (Transform transform in e)
        {
            Collider collider = transform.gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                colliders.Add(collider);
            }
        }

        return colliders;
    }

    private void GenerateNodes()
    {
        // Calculate the offset required to center the grid
        Vector2 globalOffset = -(Size / 2);

        for (float y = 0; y < Size.y + 0.01f; y += NodeSpacing)
        {
            for (float x = 0; x < Size.x + 0.01f; x += NodeSpacing)
            {
                Vector2 gridPosition = new Vector2(x, y) + globalOffset;
                Vector3 position = transform.position + new Vector3(gridPosition.x, 0, gridPosition.y);

                if (!m_colliders.Any(c => c.bounds.Contains(position)))
                {
                    GameObject newNode = Instantiate(Sphere, transform);
                    newNode.transform.position = position;
                    newNode.SetActive(true);

                    m_nodes.Add(new Node(newNode.transform));
                }
            }
        }
    }
    
    private void ConnectNodes()
    {
        foreach (Node node in m_nodes)
        {
            var nearNodes = m_nodes.Where(n => n != node && (n.Position - node.Position).magnitude <= (NodeSpacing * 1.75f));
            var nearConnections = nearNodes.Select(n => new Connection(n, (n.Position - node.Position).magnitude));
            var validConnections = nearConnections.Where(c => !Physics.Linecast(node.Position, c.Node.Position) && !Physics.Linecast(c.Node.Position, node.Position));

            node.Connections.AddRange(validConnections);
        }
    }

    private void DrawDebugLines()
    {
        foreach (var node in m_nodes)
        {
            foreach (var connection in node.Connections)
            {
                Debug.DrawLine(node.Position, connection.Node.Position, Color.green);
            }
        }
    }
}
