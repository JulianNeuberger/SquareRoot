using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public CardGrid cardGrid;

    public Card leaveCard;

    public TerrainType waterTerrain;
    public TerrainType nitrateTerrain;

    public WaterDisplay waterDisplay;
    public NitrateDisplay nitrateDisplay;
    public SugarDisplay sugarDisplay;

    [SerializeField] private int waterAmount = 5;
    [SerializeField] private int nitrateAmount = 1;
    [SerializeField] private int sugarAmount = 1;

    

    public int WaterIncome { get; private set; }
    public int NitrateIncome { get; private set; }

    public int WaterUpkeep { get; private set; }
    public int NitrateUpkeep { get; private set; }
    public int sugarUpkeep { get; private set; }

    private void Start()
    {
        UpdateAmountsDisplay();
    }

    private void Update()
    {
        if(Input.GetKeyDown("w"))
        {
            waterAmount++;
            UpdateAmountsDisplay();
        }
        if (Input.GetKeyDown("n"))
        {
            nitrateAmount++;
            UpdateAmountsDisplay();
        }
        if (Input.GetKeyDown("s"))
        {
            sugarAmount++;
            UpdateAmountsDisplay();
        }
    }

    public void PayWater(int amount)
    {
        if (amount < 0f) throw new ArgumentException("Trying to pay a negative amount!");
        if (waterAmount - amount < 0f)
            throw new ArgumentException($"Cant afford {amount} water, only have {waterAmount}");
        waterAmount -= amount;
    }
    
    public void PaySugar(int amount)
    {
        if (amount < 0f) throw new ArgumentException("Trying to pay a negative amount!");
        if (waterAmount - amount < 0f)
            throw new ArgumentException($"Cant afford {amount} sugar, only have {sugarAmount}");
        sugarAmount-= amount;
    }
    
    public void PayNitrate(int amount)
    {
        if (amount < 0f) throw new ArgumentException("Trying to pay a negative amount!");
        if (waterAmount - amount < 0f)
            throw new ArgumentException($"Cant afford {amount} nitrate, only have {nitrateAmount}");
        nitrateAmount -= amount;
    }

    public void PayForCard(Card card)
    {
        PayNitrate(card.nitrateCost);
        PaySugar(card.sugarCost);
        PayWater(card.waterCost);
        UpdateAmountsDisplay();
    }

    public bool CanPayForCard(Card card)
    {
        if (waterAmount - card.waterCost < 0f) return false;
        if (sugarAmount - card.sugarCost < 0f) return false;
        if (nitrateAmount - card.nitrateCost < 0f) return false;
        return true;
    }

    public bool ExchangeResourcesAtLeaf()
    {
        if (waterAmount < 1) return false;
        if (nitrateAmount < 1) return false;
        
        waterAmount--;
        nitrateAmount--;
        sugarAmount++;
        
        UpdateAmountsDisplay();
        
        return true;
    }

    public void UpdateResourceIncome()
    {
        var resourceIncome = GatherAllResources();

        WaterIncome = (int)resourceIncome.GetValueOrDefault(waterTerrain, 0);
        waterDisplay.UpdateWaterIncome(WaterIncome);

        NitrateIncome = (int)resourceIncome.GetValueOrDefault(nitrateTerrain, 0);
        nitrateDisplay.UpdateNitrateIncome(NitrateIncome);
    }


    public void ReceiveResourceIncome()
    {
        waterAmount += WaterIncome;
        nitrateAmount += NitrateIncome;

        UpdateAmountsDisplay();
    }


    private Dictionary<TerrainType, float> GatherAllResources()
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
        float totalSugarUpkeep = 0;

        foreach (var gridPosition in allActiveCardViewsGridPositions)
        {
            var cardView = cardGrid.GetGridCell(gridPosition).GetActiveCardView();
            
            //skip dead plant parts
            if(cardView.GetCurrentHealth() != 0)
            {
                var card = cardView.GetCard();
                totalWaterUpkeep += card.waterUpkeep;
                totalNitrateUpkeep += card.nitrateUpkeep;
                totalSugarUpkeep += card.sugarUpkeep;
            }
        }

        WaterUpkeep = (int)totalWaterUpkeep;
        waterDisplay.UpdateWaterUpkeep(WaterUpkeep);

        NitrateUpkeep = (int)totalNitrateUpkeep;
        nitrateDisplay.UpdateNitrateUpkeep(NitrateUpkeep);

        sugarUpkeep = (int)totalSugarUpkeep;
        sugarDisplay.UpdateSugarUpkeep(sugarUpkeep);
    }


    public void PayUpkeep()
    {
        waterAmount -= WaterUpkeep;
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
        
        nitrateAmount -= NitrateUpkeep;
        if (nitrateAmount < 0)
        {
            nitrateAmount = 0;
            nitrateDisplay.UpdateNitrateShortageStatus(true);
            //TODO: Do something to have consequences (?)
        }
        else
        {
            nitrateDisplay.UpdateNitrateShortageStatus(false);
        }
        
        sugarAmount -= sugarUpkeep;
        if (sugarAmount < 0)
        {
            sugarAmount = 0;
            sugarDisplay.UpdateSugarShortageStatus(true);
            //TODO: Do something to have consequences (?)
        }
        else
        {
            sugarDisplay.UpdateSugarShortageStatus(false);
        }

        UpdateAmountsDisplay();
    }

    public void DecreaseLeaveHealth()
    {
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

        UpdateUpkeep();
    }

    private void UpdateAmountsDisplay()
    {
        waterDisplay.UpdateWaterAmount(waterAmount);
        nitrateDisplay.UpdateNitrateAmount(nitrateAmount);
        sugarDisplay.UpdateSugarAmount(sugarAmount);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ResourceManager))]
public class ResourceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Update Resource Income"))
        {
            var resourceManager = (ResourceManager)target;
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
#endif
