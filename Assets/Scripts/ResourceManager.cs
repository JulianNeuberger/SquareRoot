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

    public Dictionary<TerrainType, float> GatherAllResources()
    {
        var gatheredResources = new Dictionary<TerrainType, float>();

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

    private Dictionary<TerrainType, float> GatherResourcesForCardViewAtPosition(int x, int y)
    {
        var gatheredResources = new Dictionary<TerrainType, float>();

        var gridCell = _cardGrid.GetGridCell(x, y);
        var gatherMultiplier = gridCell.GetActiveCardView().GetCard().gatherMultiplierOnResource;

        var terrain = gridCell.GetTerrain();
        if (terrain.IsGatherable)
        {
            gatheredResources.Add(terrain, gatherMultiplier);
        }    
        
        var neighborGridCells = _cardGrid.GetNeighbors(x, y);
        var gatherMultiplierNeighbors = gridCell.GetActiveCardView().GetCard().gatherMultiplierNextToResource;
        foreach (var neighbor in neighborGridCells)
        {
            var neighborTerrain = neighbor.GetTerrain();
            if(neighborTerrain.IsGatherable)
            {
                if (!gatheredResources.ContainsKey(neighborTerrain))
                {
                    gatheredResources.Add(neighborTerrain, gatherMultiplierNeighbors);
                }
                else
                {
                    gatheredResources[neighborTerrain] = gatheredResources[neighborTerrain] + gatherMultiplierNeighbors;
                }
            }
        }

        return gatheredResources;
    }
}
