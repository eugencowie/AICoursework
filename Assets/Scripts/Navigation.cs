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
    public float NodeSpacing = 1;

    private List<Collider> m_colliders = new List<Collider>();
    private List<Node> m_nodes = new List<Node>();

    private void Start()
    {
        m_colliders = GetColliders(ColliderContainer);
        m_nodes = GenerateNodes(m_colliders, Sphere, transform, Size, NodeSpacing);
        ConnectNodes(m_nodes, NodeSpacing);
    }

    private void Update()
    {
        DrawDebugLines();
    }

    private static List<Collider> GetColliders(GameObject container)
    {
        return container.transform.Cast<Transform>().Select(t => t.gameObject.GetComponent<Collider>()).Where(c => c != null).ToList();
    }

    private static List<Node> GenerateNodes(List<Collider> colliders, GameObject sphere, Transform transform, Vector2 size, float nodeSpacing)
    {
        List<Node> nodes = new List<Node>();

        // Calculate the offset required to center the grid
        Vector2 globalOffset = -(size / 2);

        for (float y = 0; y < size.y + 0.01f; y += nodeSpacing)
        {
            for (float x = 0; x < size.x + 0.01f; x += nodeSpacing)
            {
                Vector2 gridPosition = new Vector2(x, y) + globalOffset;
                Vector3 position = transform.position + new Vector3(gridPosition.x, 0, gridPosition.y);

                if (!colliders.Any(c => c.bounds.Contains(position)))
                {
                    GameObject newNode = Instantiate(sphere, transform);
                    newNode.transform.position = position;
                    newNode.SetActive(true);

                    nodes.Add(new Node(newNode.transform));
                }
            }
        }

        return nodes;
    }
    
    private static void ConnectNodes(List<Node> nodes, float nodeSpacing)
    {
        foreach (Node node in nodes)
        {
            var nearNodes = nodes.Where(n => n != node && (n.Position - node.Position).magnitude <= (nodeSpacing * 1.75f));
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
