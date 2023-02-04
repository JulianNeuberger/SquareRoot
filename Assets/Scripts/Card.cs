using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card")]
public class Card : ScriptableObject
{
    public Sprite sprite;

    public TerrainType[] validTerrain;

    public bool hasSocketAtCardTopSide;
    public bool hasSocketAtCardRightSide;
    public bool hasSocketAtCardBottomSide;
    public bool hasSocketAtCardLeftSide;

    public Card[] attachableCardTypes;
    
    public float gatherMultiplierOnResource;
    public float gatherMultiplierNextToResource;

    public TerrainType[] gatherableResources;

    public float waterUpkeep = 0f;
    public float nitrateUpkeep = 0f;

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
