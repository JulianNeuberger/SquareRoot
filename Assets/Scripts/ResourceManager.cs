using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ResourceManager : MonoBehaviour
{
    public CardGrid cardGrid;

    public Card leaveCard;

    public TerrainType waterTerrain;
    public TerrainType nitrateTerrain;

    public WaterDisplay waterDisplay;
    public NitrateDisplay nitrateDisplay;

    private int waterAmount = 0;
    private int nitrateAmount = 0;

    private int waterIncome = 0;
    private int nitrateIncome = 0;

    private int waterUpkeep = 0;
    private int nitrateUpkeep = 0;


    public int getWaterAmount()
    {
        return waterAmount;
    }

    public int getNitrateAmount()
    {
        return nitrateAmount;
    }

    public int getWaterIncome()
    {
        return waterIncome;
    }

    public int getNitrateIncome()
    {
        return nitrateIncome;
    }

    public int getWaterUpkeep()
    {
        return waterUpkeep;
    }

    public int getNitrateUpkeep()
    {
        return nitrateUpkeep;
    }


    public void UpdateResourceIncome()
    {
        var resourceIncome = GatherAllResources();

        waterIncome = (int)resourceIncome.GetValueOrDefault(waterTerrain, 0);
        waterDisplay.UpdateWaterIncome(waterIncome);

        nitrateIncome = (int)resourceIncome.GetValueOrDefault(nitrateTerrain, 0);
        nitrateDisplay.UpdateNitrateIncome(nitrateIncome);
    }


    public void ReceiveResourceIncome()
    {
        waterAmount += waterIncome;
        waterDisplay.UpdateWaterAmount(waterAmount);

        nitrateAmount += nitrateIncome;
        nitrateDisplay.UpdateNitrateAmount(nitrateAmount);
    }


    public Dictionary<TerrainType, float> GatherAllResources()
    {
        var gatheredResources = new Dictionary<TerrainType, float>();

        var allActiveCardViewsGridPositions = cardGrid.GetAllGridPositionsWithActiveCardViews();
        Debug.Log($"GatherAllResources: Received {allActiveCardViewsGridPositions.Count} active card views throughout the grid");

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
        Debug.Log($"GatherResourcesForCardViewAtPosition {pos}, cardType is {cardGrid.GetGridCell(pos).GetActiveCardView().GetCard().name}");

        var gatheredResources = new Dictionary<TerrainType, float>();

        var gridCell = cardGrid.GetGridCell(pos);
        var gatherMultiplier = gridCell.GetActiveCardView().GetCard().gatherMultiplierOnResource;
        var terrain = gridCell.GetTerrain();
        Debug.Log($"{pos} On cell: gatherMultiplier is {gatherMultiplier}, terrain is {terrain}");

        if (terrain.IsGatherable && gridCell.GetActiveCardView().GetCard().CanGatherResource(terrain))
        {
            gatheredResources.Add(terrain, gatherMultiplier);
            Debug.Log($"{pos} On cell: Added {gatherMultiplier} of {terrain}");
        }    
        
        var neighborGridCells = cardGrid.GetNeighbors(pos);

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


    public void UpdateUpkeep()
    {
        var allActiveCardViewsGridPositions = cardGrid.GetAllGridPositionsWithActiveCardViews();
        Debug.Log($"UpdateUpkeep: Received {allActiveCardViewsGridPositions.Count} active card views throughout the grid");

        float totalWaterUpkeep = 0;
        float totalNitrateUpkeep = 0;

        foreach (var gridPosition in allActiveCardViewsGridPositions)
        {
            var card = cardGrid.GetGridCell(gridPosition).GetActiveCardView().GetCard();
            totalWaterUpkeep += card.waterUpkeep;
            totalNitrateUpkeep += card.nitrateUpkeep;
        }

        waterUpkeep = (int)totalWaterUpkeep;
        waterDisplay.UpdateWaterUpkeep(waterUpkeep);

        nitrateUpkeep = (int)totalNitrateUpkeep;
        nitrateDisplay.UpdateNitrateUpkeep(nitrateUpkeep);
    }


    public void PayUpkeep()
    {
        waterAmount -= waterUpkeep;
        if(waterAmount < 0)
        {
            Debug.Log("Water shortage!");
            waterAmount = 0;
            waterDisplay.UpdateWaterShortageStatus(true);
            DecreaseLeaveHealth();
        }
        else
        {
            waterDisplay.UpdateWaterShortageStatus(false);
        }

        waterDisplay.UpdateWaterAmount(waterAmount);

        nitrateAmount -= nitrateUpkeep;
        if(nitrateAmount < 0)
        {
            nitrateAmount = 0;
            nitrateDisplay.UpdateNitrateShortageStatus(true);
            //TODO: Do something to have consequences (?)
        }
        else
        {
            nitrateDisplay.UpdateNitrateShortageStatus(false);
        }

        nitrateDisplay.UpdateNitrateAmount(nitrateAmount);
    }

    public void DecreaseLeaveHealth()
    {
        Debug.Log("Trying to decrease health of a leaf");

        var allActiveCardViewsGridPositions = cardGrid.GetAllGridPositionsWithActiveCardViews();
        var orderedActiveCardViewsGridPositions = allActiveCardViewsGridPositions.OrderByDescending(pos => pos.y).ThenBy(pos => pos.x);

        var leavesAlivePositions = orderedActiveCardViewsGridPositions
            .Where(pos => cardGrid.GetGridCell(pos).GetActiveCardView().GetCard() == leaveCard)
            .Where(pos => cardGrid.GetGridCell(pos).GetActiveCardView().GetCurrentHealth() > 0);

        if(!leavesAlivePositions.Any())
        {
            Debug.Log("Did not find any leaf alive, can not decrease health!");
            //TODO: TRIGGER GAME LOSE?
            return;
        }

        var firstLeafAlivePosition = leavesAlivePositions.First();
        Debug.Log($"Decreasing health of leaf at position {firstLeafAlivePosition}");
        cardGrid.GetGridCell(firstLeafAlivePosition).GetActiveCardView().DecreaseHealth();
    }


    [CustomEditor(typeof(ResourceManager))]
    public class ResourceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Update Resource Income"))
            {
                var resourceManager = (ResourceManager) target;
                resourceManager.UpdateResourceIncome();
            }

            if (GUILayout.Button("Receive Resource Income"))
            {
                var resourceManager = (ResourceManager)target;
                resourceManager.ReceiveResourceIncome();
            }

            if (GUILayout.Button("Update Upkeep"))
            {
                var resourceManager = (ResourceManager)target;
                resourceManager.UpdateUpkeep();
            }

            if (GUILayout.Button("Pay Upkeep"))
            {
                var resourceManager = (ResourceManager)target;
                resourceManager.PayUpkeep();
            }
        }
    }
}
