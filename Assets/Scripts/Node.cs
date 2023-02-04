using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public GameObject card; // TODO: Other way round, link to the nodes within the card?

    public List<Node> neighbors = new List<Node>();

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void AddNeighbor(Node newNeighbor)
    {
        neighbors.Add(newNeighbor);
        newNeighbor.neighbors.Add(this);
    }
}
