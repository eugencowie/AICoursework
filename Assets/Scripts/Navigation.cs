using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public Vector3 Position;
    public List<Connection> Connections;

    public Node Parent;
    public float G;
    public float H;

    public Node(Vector3 position)
    {
        Position = position;
        Connections = new List<Connection>();
        Reset();
    }

    public void Reset()
    {
        Parent = null;
        G = Mathf.Infinity;
        H = Mathf.Infinity;
    }

    public float F => G + H;
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
    public LayerMask IgnoreLayer;
    public GameObject ColliderContainer;
    public GameObject Sphere;

    public Vector3 Size = new Vector3(55, 0, 55);
    public float NodeSpacing = 0.5f;
    
    private List<Node> m_nodes = new List<Node>();

    private void Start()
    {
        m_nodes = ConnectNodes(GenerateNodes(GetColliders(ColliderContainer), Sphere, transform, Size, NodeSpacing), NodeSpacing * 1.5f, IgnoreLayer);
    }
    
    private static List<Collider> GetColliders(GameObject container)
    {
        return container.transform.Cast<Transform>().Select(t => t.gameObject.GetComponent<Collider>()).Where(c => c != null && c.gameObject.GetComponent<Unit>() == null).ToList();
    }

    private static List<Node> GenerateNodes(List<Collider> colliders, GameObject sphere, Transform transform, Vector3 size, float nodeSpacing)
    {
        List<Node> validNodes = new List<Node>();

        // Calculate the position to start the grid (the grid should be centered)
        Vector3 start = transform.position - (size / 2);

        // Calculate the position to end the grid
        Vector3 end = start + size;

        for (Vector3 pos = start; pos.z <= end.z; pos.z += nodeSpacing)
        {
            for (pos.y = start.y; pos.y <= end.y; pos.y += nodeSpacing)
            {
                for (pos.x = start.x; pos.x <= end.x; pos.x += nodeSpacing)
                {
                    // If not intersecting any object, add the node to the list of valid nodes
                    if (!colliders.Any(c => c.bounds.Contains(pos)))
                    {
                        validNodes.Add(new Node(pos));
                    }
                }
            }
        }

        return validNodes;
    }

    private static List<Node> ConnectNodes(List<Node> nodeInput, float radius, LayerMask ignoreLayer)
    {
        List<Node> nodes = nodeInput.ToList();

        foreach (Node current in nodes)
        {
            // Get a list of nodes which are within a certain radius of this node
            var nearNodes = nodes
                .Where(n => (n.Position - current.Position).magnitude <= radius)
                .Where(n => n != current);

            // Use raycasting to check if there is a valid connection to each nearby node
            var validConnections = nearNodes
                .Where(n => !Physics.Linecast(current.Position, n.Position, ~ignoreLayer))
                .Where(n => !Physics.Linecast(n.Position, current.Position, ~ignoreLayer));

            // Create a connection object for each valid connection to another node (the cost
            // of the connection is set as the distance between the nodes)
            var nearConnections = validConnections
                .Select(n => new Connection(n, (n.Position - current.Position).magnitude));

            current.Connections.AddRange(nearConnections);
        }

        return nodes;
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
        // Reset all nodes
        nodes.ForEach(n => n.Reset());

        // Cost from start to start is zero
        start.G = 0;

        // Heuristic
        start.H = (end.Position - start.Position).magnitude;

        // Nodes already evaluated
        List<Node> closedList = new List<Node>();

        // Discovered nodes not yet evaluated
        List<Node> openList = new List<Node>();
        openList.Add(start);

        while (openList.Count > 0)
        {
            Node current = openList.OrderBy(n => n.F).First();

            if (current == end)
            {
                List<Node> path = new List<Node>();
                while (current.Parent != null)
                {
                    path.Add(current);
                    current = current.Parent;
                }
                path.Reverse();
                return path;
            }

            openList.Remove(current);
            closedList.Add(current);
            
            foreach (var connection in current.Connections)
            {
                // Ignore if connection has already been evaluated
                if (closedList.Contains(connection.Node))
                {
                    continue;
                }

                if (!openList.Contains(connection.Node))
                {
                    openList.Add(connection.Node);
                }

                float g = current.G + connection.Cost;

                // This is not a better path
                if (g >= connection.Node.G)
                {
                    continue;
                }

                connection.Node.Parent = current;
                connection.Node.G = g;
                connection.Node.H = (end.Position - connection.Node.Position).magnitude;
            }
        }

        return new List<Node>(new Node[] { start });
    }

    public Node GetClosestNode(Vector3 point) => GetClosestNode(m_nodes, point);
    public List<Node> FindPath(Node start, Node end) => FindPath(m_nodes, start, end);
}
