using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    private Node rootNode;


    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    private void SetRoot(Node node)
    {
        rootNode = node;
    }


    public bool IsConnectedToRoot(Node node)
    {
        var nodesConnectedToRoot = DepthFirstSearch(rootNode);
        return nodesConnectedToRoot.Contains(node);
    }

    public bool AreConnected(Node nodeOne, Node nodeTwo)
    {
        var nodesConnectedToNodeOne = DepthFirstSearch(nodeOne);
        return nodesConnectedToNodeOne.Contains(nodeTwo);
    }

    private HashSet<Node> DepthFirstSearch(Node startNode)
    {
        var visited = new HashSet<Node>();

        var stack = new Stack<Node>();
        stack.Push(startNode);

        while(stack.Count > 0)
        {
            var currentNode = stack.Pop();

            if(visited.Contains(currentNode))
            {
                continue;
            }

            visited.Add(currentNode);
            foreach(var neighbor in currentNode.neighbors)
            {
                if(!visited.Contains(neighbor))
                {
                    stack.Push(neighbor);
                }
            }
        }

        return visited;
    } 


}
