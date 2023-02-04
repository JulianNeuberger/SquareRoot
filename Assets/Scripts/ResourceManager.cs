using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private CardGrid _cardGrid;

    private void Awake()
    {
        _cardGrid = GetComponent<CardGrid>();
    }

    public Dictionary<TerrainType, float> GatherAllResources()
    {
        var gatheredResources = new Dictionary<TerrainType, float>();

        var allActiveCardViewsGridPositions = _cardGrid.GetAllGridPositionsWithActiveCardViews();
        foreach(var gridPosition in allActiveCardViewsGridPositions)
        {
            var gatheredResourcesOfCardView = GatherResourcesForCardViewAtPosition(gridPosition);
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

    private Dictionary<TerrainType, float> GatherResourcesForCardViewAtPosition(Vector2Int pos)
    {
        var gatheredResources = new Dictionary<TerrainType, float>();

        var gridCell = _cardGrid.GetGridCell(pos);
        var gatherMultiplier = gridCell.GetActiveCardView().GetCard().gatherMultiplierOnResource;

        var terrain = gridCell.GetTerrain();
        if (terrain.IsGatherable && gridCell.GetActiveCardView().GetCard().CanGatherResource(terrain))
        {
            gatheredResources.Add(terrain, gatherMultiplier);
        }    
        
        var neighborGridCells = _cardGrid.GetNeighbors(pos);
        var gatherMultiplierNeighbors = gridCell.GetActiveCardView().GetCard().gatherMultiplierNextToResource;
        foreach (var neighbor in neighborGridCells)
        {
            terrain = neighbor.GetTerrain();
            if(terrain.IsGatherable && gridCell.GetActiveCardView().GetCard().CanGatherResource(terrain))
            {
                if (!gatheredResources.ContainsKey(terrain))
                {
                    gatheredResources.Add(terrain, gatherMultiplierNeighbors);
                }
                else
                {
                    gatheredResources[terrain] = gatheredResources[terrain] + gatherMultiplierNeighbors;
                }
            }
        }

        return gatheredResources;
    }
}
