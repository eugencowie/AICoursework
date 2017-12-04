using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public Transform Transform;
    public List<Connection> Connections;
    public Node Parent;
    public bool Visited;

    public Node(Transform transform)
    {
        Transform = transform;
        Connections = new List<Connection>();
        Parent = null;
        Visited = false;
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
    
    private List<Node> m_nodes = new List<Node>();

    private void Start()
    {
        m_nodes = ConnectNodes(GenerateNodes(GetColliders(ColliderContainer), Sphere, transform, Size, NodeSpacing), NodeSpacing);
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

    private static List<Node> ConnectNodes(List<Node> nodes, float nodeSpacing)
    {
        List<Node> result = nodes.ToList();

        foreach (Node node in result)
        {
            var nearNodes = result.Where(n => n != node && (n.Position - node.Position).magnitude <= (nodeSpacing * 1.75f));
            var nearConnections = nearNodes.Select(n => new Connection(n, (n.Position - node.Position).magnitude));
            var validConnections = nearConnections.Where(c => !Physics.Linecast(node.Position, c.Node.Position) && !Physics.Linecast(c.Node.Position, node.Position));

            node.Connections.AddRange(validConnections);
        }

        return result;
    }

    private void Update()
    {
        DrawDebugLines();
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

    public static Node GetClosestNode(List<Node> nodes, Vector3 point)
    {
        return nodes.OrderBy(n => (point - n.Position).sqrMagnitude).First();
    }

    public static List<Node> FindPath(List<Node> nodes, Node start, Node end)
    {
        foreach (var n in nodes)
        {
            n.Parent = null;
            n.Visited = false;
        }

        Queue<Node> queue = new Queue<Node>();

        start.Visited = true;
        queue.Enqueue(start);

        while (queue.Any())
        {
            Node current = queue.Dequeue();

            if (current == end)
            {
                return ReturnPath(end);
            }

            foreach (Connection connection in current.Connections)
            {
                if (!connection.Node.Visited)
                {
                    connection.Node.Visited = true;
                    connection.Node.Parent = current;
                    queue.Enqueue(connection.Node);
                }
            }
        }

        return new Queue<Node>(new Node[] { start });
    }

    private static List<Node> ReturnPath(Node node)
    {
        List<Node> foundPath = new List<Node>();
        if (node != null)
        {
            foundPath.Add(node);

            while (node.Parent != null)
            {
                foundPath.Add(node.Parent);
                node = node.Parent;
            }

            // Reverse the path so the start node is at index 0
            foundPath.Reverse();
        }
        return foundPath;
    }
}
