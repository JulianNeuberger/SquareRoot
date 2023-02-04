using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResourceManager : MonoBehaviour
{
    public CardGrid _cardGrid;

    public Dictionary<TerrainType, float> GatherAllResources()
    {
        var gatheredResources = new Dictionary<TerrainType, float>();

        var allActiveCardViewsGridPositions = _cardGrid.GetAllGridPositionsWithActiveCardViews();

        Debug.Log($"Received {allActiveCardViewsGridPositions.Count} active card views throughout the grid");

        foreach(var gridPosition in allActiveCardViewsGridPositions)
        {
            var gatheredResourcesOfCardView = GatherResourcesForCardViewAtPosition(gridPosition);

            Debug.Log($"===== Gathered resources for gridPosition {gridPosition}: =====");
            foreach (var pair in gatheredResourcesOfCardView)
            {
                Debug.Log($"{pair.Key}: {pair.Value}");
            }
            Debug.Log($"===== ===== ===== =====");

            foreach (var resource in gatheredResourcesOfCardView.Keys)
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

        Debug.Log($"===== Gathered resources overall: =====");
        foreach (var pair in gatheredResources)
        {
            Debug.Log($"{pair.Key}: {pair.Value}");
        }
        Debug.Log($"===== ===== ===== =====");

        return gatheredResources;
    }

    private Dictionary<TerrainType, float> GatherResourcesForCardViewAtPosition(Vector2Int pos)
    {
        Debug.Log($"GatherResourcesForCardViewAtPosition {pos}, cardType is {_cardGrid.GetGridCell(pos).GetActiveCardView().GetCard().name}");

        var gatheredResources = new Dictionary<TerrainType, float>();

        var gridCell = _cardGrid.GetGridCell(pos);
        var gatherMultiplier = gridCell.GetActiveCardView().GetCard().gatherMultiplierOnResource;
        var terrain = gridCell.GetTerrain();
        Debug.Log($"{pos} On cell: gatherMultiplier is {gatherMultiplier}, terrain is {terrain}");

        if (terrain.IsGatherable && gridCell.GetActiveCardView().GetCard().CanGatherResource(terrain))
        {
            gatheredResources.Add(terrain, gatherMultiplier);
            Debug.Log($"{pos} On cell: Added {gatherMultiplier} of {terrain}");
        }    
        
        var neighborGridCells = _cardGrid.GetNeighbors(pos);

        Debug.Log($"Got {neighborGridCells.Count} neighbors to gather from");
        var gatherMultiplierNeighbors = gridCell.GetActiveCardView().GetCard().gatherMultiplierNextToResource;
        foreach (var neighbor in neighborGridCells)
        {
            terrain = neighbor.GetTerrain();
            Debug.Log($"{pos} Neighbor {neighbor.GetGridPosition()}: gatherMultiplier is {gatherMultiplierNeighbors}, terrain is {terrain}");

            if (terrain.IsGatherable && gridCell.GetActiveCardView().GetCard().CanGatherResource(terrain))
            {
                if (!gatheredResources.ContainsKey(terrain))
                {
                    gatheredResources.Add(terrain, gatherMultiplierNeighbors);
                    Debug.Log($"{pos} Neighbor: Added {gatherMultiplier} of {terrain} (new resource)");
                }
                else
                {
                    gatheredResources[terrain] = gatheredResources[terrain] + gatherMultiplierNeighbors;
                    Debug.Log($"{pos} Neighbor: Added {gatherMultiplier} of {terrain} (existing, total amount is now {gatheredResources[terrain]})");
                }
            }
        }

        return gatheredResources;
    }

    [CustomEditor(typeof(ResourceManager))]
    public class ResourceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Gather Resources"))
            {
                var resourceManager = (ResourceManager) target;
                resourceManager.GatherAllResources();
            }
        }
    }
}
