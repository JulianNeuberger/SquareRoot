using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private CardGrid _cardGrid;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public Dictionary<string, float> GatherAllResources()
    {
        var gatheredResources = new Dictionary<string, float>();

        var allActiveCardViewsGridPositions = _cardGrid.GetAllGridPositionsWithActiveCardViews();
        foreach(var gridPosition in allActiveCardViewsGridPositions)
        {
            var gatheredResourcesOfCardView = GatherResourcesForCardViewAtPosition(gridPosition.Item1, gridPosition.Item2);
            foreach(var resource in gatheredResourcesOfCardView.Keys)
            {
                if(!gatheredResources.ContainsKey(resource))
                {
                    gatheredResources.Add(resource, gatheredResourcesOfCardView[resource]);
                }
                else
                {
                    gatheredResources[resource] = gatheredResources[resource] + gatheredResourcesOfCardView[resource];
                }
            }
        }

        return gatheredResources;
    }

    private Dictionary<string, float> GatherResourcesForCardViewAtPosition(int x, int y)
    {
        var gatheredResources = new Dictionary<string, float>();

        var gridCell = _cardGrid.GetGridCell(x, y);
        var gatherMultiplier = gridCell.GetActiveCardView().GetCard().gatherMultiplierOnResource;
        foreach (var resource in gridCell.GetTerrain().Resources)
        {
            gatheredResources.Add(resource, gatherMultiplier);
        }    
        
        
        var neighborGridCells = _cardGrid.GetNeighbors(x, y);
        var gatherMultiplierNeighbors = gridCell.GetActiveCardView().GetCard().gatherMultiplierNextToResource;
        foreach (var neighbor in neighborGridCells)
        {
            foreach(var resource in neighbor.GetTerrain().Resources)
            {
                if (!gatheredResources.ContainsKey(resource))
                {
                    gatheredResources.Add(resource, gatherMultiplierNeighbors);
                }
                else
                {
                    gatheredResources[resource] = gatheredResources[resource] + gatherMultiplierNeighbors;
                }
            }
        }

        return gatheredResources;
    }
}
