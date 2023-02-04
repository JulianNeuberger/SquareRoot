using System;
using System.Collections.Generic;
using System.Linq;

public class GraphManager<T> where T: class 
{
    public List<Node<T>> Nodes { get; private set; }
    
    public GraphManager(T rootValue)
    {
        Root = new Node<T>(rootValue);
        Nodes = new List<Node<T>>();
        Nodes.Add(Root);
    }
    
    public Node<T> Root { get; private set; }

    public Node<T> AddNode(T value)
    {
        var newNode = new Node<T>(value);
        Nodes.Add(newNode);
        return newNode;
    }

    public void Connect(T value, T other)
    {
        var firstNode = GetNodeByValue(value);
        var otherNode = GetNodeByValue(other);

        if (firstNode == null) throw new ArgumentException($"Value {value} is not in graph");
        if (otherNode == null) throw new ArgumentException($"Other {other} is not in graph");
        
        firstNode.MakeConnection(otherNode);
    }

    public Node<T> GetNodeByValue(T value)
    {
        return Nodes.FirstOrDefault(n => n.Value == value);
    }

    public bool IsConnectedToRoot(T value)
    {
        return AreConnected(Root.Value, value);
    }

    public bool AreConnected(T value, T other)
    {
        var start = GetNodeByValue(value);
        var target = GetNodeByValue(other);
        var found = DepthFirstSearch(start, new List<Node<T>>(), (n, _) => n == target);
        return found;
    }

    private bool DepthFirstSearch(Node<T> node, List<Node<T>> visited, Func<Node<T>, List<Node<T>>, bool> predicate)
    {
        foreach (var neighbour in node.neighbors)
        {
            if (visited.Contains(neighbour)) continue;

            var shouldStop = predicate.Invoke(neighbour, visited);
            if (shouldStop) return true;
            
            visited.Add(neighbour);
            DepthFirstSearch(neighbour, visited, predicate);
        }

        return false;
    }
}
