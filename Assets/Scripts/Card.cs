using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Card")]
public class Card : ScriptableObject
{
    public Sprite sprite;
    public Sprite unhealthySprite;
    public Sprite deadSprite;
    public int visionRange = 1;
    
    public int initialHealth = 1;

    [Header("Constraints")]
    public TerrainType[] validTerrain;
    public Card[] attachableCardTypes;

    [Header("Sockets")]
    public bool hasSocketAtCardTopSide;
    public bool hasSocketAtCardRightSide;
    public bool hasSocketAtCardBottomSide;
    public bool hasSocketAtCardLeftSide;


    [Header("Gathering")]
    public float gatherMultiplierOnResource;
    public float gatherMultiplierNextToResource;

    public TerrainType[] gatherableResources;

    [Header("Upkeep")]
    public float waterUpkeep = 0f;
    public float nitrateUpkeep = 0f;
    public float sugarUpkeep;
    
    [Header("Cost")]
    public int nitrateCost;
    public int sugarCost;
    public int waterCost;

    public bool HasSocketAtWorldSideId(int worldSideId, int cardRotation)
    {
        //Debug.Log($"Card with name {this.name} executing HasSocketAtWorldSideId with worldSideId {worldSideId} and rotation {cardRotation}.");
        
        var cardSideId = (worldSideId - cardRotation + 4) % 4;
        
        //Debug.Log($"cardSideId: {cardSideId}");

        switch(cardSideId)
        {
            case 0: return hasSocketAtCardTopSide;
            case 1: return hasSocketAtCardRightSide;
            case 2: return hasSocketAtCardBottomSide;
            case 3: return hasSocketAtCardLeftSide;

        }
        return false;
    }

    public bool CanPlaceOnTerrain(TerrainType terrain)
    {
        if (validTerrain.Contains(terrain))
        {
            return true;
        }
        return false;
    }

    public bool CanAttachCardType(Card cardType)
    {
        if (attachableCardTypes.Contains(cardType))
        {
            return true;
        }
        return false;
    }

    public bool CanGatherResource(TerrainType resource)
    {
        if(gatherableResources.Contains(resource))
        {
            return true;
        }
        return false;
    }
}
