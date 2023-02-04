using System.Collections.Generic;

public class Node<T>
{
    public readonly List<Node<T>> neighbors = new();
    public T Value { get; private set; }
    
    public Node(T value)
    {
        Value = value;
    }
    
    public void MakeConnection(Node<T> other)
    {
        neighbors.Add(other);
        other.neighbors.Add(other);
    }
}
