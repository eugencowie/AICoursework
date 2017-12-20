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
        return container.transform.Cast<Transform>()
            .Select(t => t.gameObject.GetComponent<Collider>())
            .Where(c => c != null && c.gameObject.GetComponent<Unit>() == null)
            .ToList();
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

    public static float Heuristic(Node current, Node target)
    {
        // Return the Euclidean distance from the current node to the target node
        return (target.Position - current.Position).magnitude;
    }

    public static List<Node> TraversePath(Node current)
    {
        List<Node> path = new List<Node>();
        while (current.Parent != null)
        {
            path.Insert(0, current);
            current = current.Parent;
        }
        return path;
    }

    public static List<Node> FindPath(List<Node> nodes, Node start, Node end)
    {
        nodes.ForEach(n => n.Reset());   // Reset all nodes

        List<Node> closedList = new List<Node>(); // Nodes already evaluated
        List<Node> openList = new List<Node>();   // Nodes discovered but not yet evaluated

        start.G = 0;                     // Initial cost at the start
        start.H = Heuristic(start, end); // Heuristic from start to end
        openList.Add(start);             // Add start node to open list to be evaluated

        while (openList.Count > 0)
        {
            Node current = openList.OrderBy(n => n.F).First(); // Node with lowest total cost

            openList.Remove(current); // Remove from open list as it is now being evaluated
            closedList.Add(current);  // Add to closed list as it is now being evaluated

            if (current == end)               // If we have reached the end node
                return TraversePath(current); // Return the path that we took to get here

            foreach (var connection in current.Connections)
            {
                if (closedList.Contains(connection.Node)) // If connected node already evaluated
                    continue;                             // Skip connection
                
                if (!openList.Contains(connection.Node)) // If connected node not on the open list
                    openList.Add(connection.Node);       // Add connected node to open list
                
                float g = current.G + connection.Cost;   // Cost to get to the connected node

                if (g >= connection.Node.G) // If calculated cost is higher than cost of the node
                    continue;               // Skip connection, not a better path to the node

                connection.Node.Parent = current;                    // Set parent to current node
                connection.Node.G = g;                               // Set cost = calculated cost
                connection.Node.H = Heuristic(connection.Node, end); // Heuristic from node to end
            }
        }

        // If we have not found a path and there are no more
        // nodes on the open list, just return the start node
        return new List<Node>(new Node[] { start });
    }

    public Node GetClosestNode(Vector3 point) => GetClosestNode(m_nodes, point);
    public List<Node> FindPath(Node start, Node end) => FindPath(m_nodes, start, end);
}
