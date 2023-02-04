using System;
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
        other.neighbors.Add(this);
    }

    public void RemoveConnection(Node<T> other)
    {
        if (!neighbors.Contains(other)) return;
        neighbors.Remove(other);
        if (!other.neighbors.Contains(this)) return;
        other.neighbors.Remove(this);
    }

    public void Isolate()
    {
        if (neighbors.Contains(this)) throw new ArgumentException("WTF?");
        foreach (var neighbor in this.neighbors)
        {
            neighbor.neighbors.Remove(this);
        }

        neighbors.Clear();
    }
}