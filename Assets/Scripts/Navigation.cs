﻿using System.Collections.Generic;
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
                    //GameObject newNode = Instantiate(sphere, transform);
                    //newNode.transform.position = position;
                    //newNode.SetActive(true);

                    nodes.Add(new Node(position));
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
